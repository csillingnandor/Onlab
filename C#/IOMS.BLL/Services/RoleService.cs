using IOMS.BLL.Auth;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

// Szerepkörök (felhasználói csoportok), az általuk elérhető oldalak és az oldalakon engedélyezett műveletek;
// mindkettő AspNetRoleClaims sor (ClaimType = "page" / "action").
public class RoleService : IRoleService
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<RoleData>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await QueryRoles().OrderBy(r => r.Name).ToListAsync(ct);
        return [.. roles.Select(SortPages)];
    }

    public async Task<RoleData?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var role = await QueryRoles().SingleOrDefaultAsync(r => r.Id == id, ct);
        return role is null ? null : SortPages(role);
    }

    public async Task<RoleData> CreateAsync(SaveRoleData data, CancellationToken ct = default)
    {
        var name = data.Name.Trim();
        await EnsureNameIsFreeAsync(name, null, ct);

        var role = new IdentityRole(name);

        // A RoleManager ugyanezt a (scoped) kontextust használja, így a szerepkör és az oldalai egy tranzakcióba kerülnek
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        EnsureSucceeded(await _roleManager.CreateAsync(role));
        await SetPermissionsAsync(role.Id, data, ct);
        await transaction.CommitAsync(ct);

        return (await GetByIdAsync(role.Id, ct))!;
    }

    public async Task<RoleData?> ModifyAsync(string id, SaveRoleData data, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return null;

        if (role.Name == AppRoles.Admin)
            throw new ConflictException(string.Empty, "Az adminisztrátor szerepkör nem módosítható, mindig minden oldalt és műveletet elér.");

        var name = data.Name.Trim();
        await EnsureNameIsFreeAsync(name, id, ct);

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        if (role.Name != name)
        {
            EnsureSucceeded(await _roleManager.SetRoleNameAsync(role, name));
            EnsureSucceeded(await _roleManager.UpdateAsync(role));
        }
        await SetPermissionsAsync(role.Id, data, ct);
        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return false;

        if (role.Name == AppRoles.Admin)
            throw new ConflictException(string.Empty, "Az adminisztrátor szerepkör nem törölhető.");

        // Mint a termékeknél: amíg hivatkoznak rá, nem törölhető (a tagok különben csendben elveszítenék a jogaikat)
        var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == id, ct);
        if (userCount > 0)
            throw new ConflictException(string.Empty, $"A szerepkör nem törölhető, mert {userCount} felhasználó tartozik hozzá.");

        EnsureSucceeded(await _roleManager.DeleteAsync(role));
        return true;
    }

    private IQueryable<RoleData> QueryRoles()
    {
        return _context.Roles
            .AsNoTracking()
            .Select(r => new RoleData
            {
                Id = r.Id,
                Name = r.Name!,
                Pages = _context.RoleClaims
                    .Where(rc => rc.RoleId == r.Id && rc.ClaimType == AppPages.ClaimType)
                    .Select(rc => rc.ClaimValue!)
                    .ToList(),
                Actions = _context.RoleClaims
                    .Where(rc => rc.RoleId == r.Id && rc.ClaimType == AppActions.ClaimType)
                    .Select(rc => rc.ClaimValue!)
                    .ToList(),
                UserCount = _context.UserRoles.Count(ur => ur.RoleId == r.Id),
                IsSystem = r.Name == AppRoles.Admin,
            });
    }

    // Az oldalak és műveletek az AppPages / AppActions (menü) sorrendjében
    private static RoleData SortPages(RoleData role)
    {
        role.Pages = [.. AppPages.All.Where(role.Pages.Contains)];
        role.Actions = [.. AppActions.All.Where(role.Actions.Contains)];
        return role;
    }

    // Az oldal- és művelet-claim-ek cseréje a megadottakra. Ki nem választott oldal művelete
    // nem kerül be (a validátor ezt már jelzi, itt csak biztosra megyünk).
    private async Task SetPermissionsAsync(string roleId, SaveRoleData data, CancellationToken ct)
    {
        await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId && (rc.ClaimType == AppPages.ClaimType || rc.ClaimType == AppActions.ClaimType))
            .ExecuteDeleteAsync(ct);

        var pages = data.Pages.Distinct().ToList();
        var actions = data.Actions.Distinct().Where(a => pages.Contains(AppActions.PageOf(a)));

        _context.RoleClaims.AddRange(
            pages.Select(page => NewClaim(roleId, AppPages.ClaimType, page))
                .Concat(actions.Select(action => NewClaim(roleId, AppActions.ClaimType, action))));
        await _context.SaveChangesAsync(ct);
    }

    private static IdentityRoleClaim<string> NewClaim(string roleId, string type, string value) =>
        new() { RoleId = roleId, ClaimType = type, ClaimValue = value };

    // A RoleManager is jelezné, de előre ellenőrizve mezőhöz köthető, érthető hibát adunk
    private async Task EnsureNameIsFreeAsync(string name, string? ownId, CancellationToken ct)
    {
        var normalized = _roleManager.NormalizeKey(name);
        if (await _context.Roles.AnyAsync(r => r.NormalizedName == normalized && r.Id != ownId, ct))
            throw new BusinessValidationException(nameof(SaveRoleData.Name), $"Már létezik szerepkör ezzel a névvel: {name}");
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new BusinessValidationException(nameof(SaveRoleData.Name), string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
