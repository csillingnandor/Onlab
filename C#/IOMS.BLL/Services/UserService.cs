using IOMS.BLL.Auth;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

// A szerepkör-változások a felhasználó következő bejelentkezésekor lépnek életbe (a jogosultságok a tokenben vannak).
public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<UserData>> GetAllAsync(CancellationToken ct = default)
    {
        return await QueryUsers().OrderBy(u => u.UserName).ToListAsync(ct);
    }

    public async Task<UserData?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await QueryUsers().SingleOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<UserData> CreateAsync(CreateUserData data, CancellationToken ct = default)
    {
        var roles = await ValidateRolesAsync(data.Roles, ct);
        var user = new AppUser
        {
            UserName = data.UserName.Trim(),
            DisplayName = data.DisplayName.Trim(),
            Email = NormalizeEmail(data.Email),
        };

        // A UserManager ugyanezt a (scoped) kontextust használja, így egy tranzakcióba kerül a felhasználó és a szerepkörei
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        EnsureSucceeded(await _userManager.CreateAsync(user, data.Password));
        if (roles.Count > 0)
            EnsureSucceeded(await _userManager.AddToRolesAsync(user, roles));
        await transaction.CommitAsync(ct);

        return (await GetByIdAsync(user.Id, ct))!;
    }

    public async Task<UserData?> ModifyAsync(string id, ModifyUserData data, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return null;

        var newRoles = await ValidateRolesAsync(data.Roles, ct);
        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains(AppRoles.Admin) && !newRoles.Contains(AppRoles.Admin))
            await EnsureNotLastAdminAsync(ct);

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        user.DisplayName = data.DisplayName.Trim();
        user.Email = NormalizeEmail(data.Email);
        EnsureSucceeded(await _userManager.UpdateAsync(user));

        var removed = currentRoles.Except(newRoles).ToList();
        var added = newRoles.Except(currentRoles).ToList();
        if (removed.Count > 0)
            EnsureSucceeded(await _userManager.RemoveFromRolesAsync(user, removed));
        if (added.Count > 0)
            EnsureSucceeded(await _userManager.AddToRolesAsync(user, added));

        if (!string.IsNullOrEmpty(data.NewPassword))
        {
            // Előbb ellenőrizzük, hogy érvénytelen új jelszónál ne maradjon jelszó nélkül a fiók
            foreach (var validator in _userManager.PasswordValidators)
                EnsureSucceeded(await validator.ValidateAsync(_userManager, user, data.NewPassword), nameof(data.NewPassword));

            EnsureSucceeded(await _userManager.RemovePasswordAsync(user));
            EnsureSucceeded(await _userManager.AddPasswordAsync(user, data.NewPassword), nameof(data.NewPassword));
        }

        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(string id, string currentUserId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return false;

        if (user.Id == currentUserId)
            throw new ConflictException(string.Empty, "Saját magadat nem törölheted.");

        if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
            await EnsureNotLastAdminAsync(ct);

        EnsureSucceeded(await _userManager.DeleteAsync(user));
        return true;
    }

    private IQueryable<UserData> QueryUsers()
    {
        return _context.Users
            .AsNoTracking()
            .Select(u => new UserData
            {
                Id = u.Id,
                UserName = u.UserName!,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                    .OrderBy(name => name)
                    .ToList(),
            });
    }

    private static string? NormalizeEmail(string? email) => string.IsNullOrWhiteSpace(email) ? null : email.Trim();

    // Duplikátumok nélkül; nem létező szerepkörnél 400
    private async Task<List<string>> ValidateRolesAsync(IEnumerable<string> roles, CancellationToken ct)
    {
        var requested = roles.Distinct().ToList();
        var existing = await _context.Roles
            .Where(r => requested.Contains(r.Name!))
            .Select(r => r.Name!)
            .ToListAsync(ct);

        var unknown = requested.Except(existing).ToList();
        if (unknown.Count > 0)
            throw new BusinessValidationException(nameof(CreateUserData.Roles), $"Nem létező szerepkör: {string.Join(", ", unknown)}");

        return requested;
    }

    // Mindig maradjon legalább egy adminisztrátor, különben senki sem tudná kezelni a jogosultságokat
    private async Task EnsureNotLastAdminAsync(CancellationToken ct)
    {
        var adminCount = (await _userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;
        if (adminCount <= 1)
            throw new ConflictException(nameof(CreateUserData.Roles), "Legalább egy adminisztrátornak maradnia kell.");
    }

    // Az Identity hibáit mezőnként csoportosítva adjuk vissza (a jelszóhibák a jelszó mezőhöz kerülnek)
    private static void EnsureSucceeded(IdentityResult result, string passwordField = nameof(CreateUserData.Password))
    {
        if (result.Succeeded)
            return;

        var errors = result.Errors
            .GroupBy(e => e.Code switch
            {
                var code when code.StartsWith("Password") => passwordField,
                var code when code.Contains("UserName") => nameof(CreateUserData.UserName),
                _ => string.Empty,
            })
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        throw new BusinessValidationException(errors);
    }
}
