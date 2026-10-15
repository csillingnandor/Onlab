using System.Security.Claims;
using System.Text;
using IOMS.BLL.Auth;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IOMS.BLL.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Hibás felhasználónév vagy jelszó.";

    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly JwtOptions _jwtOptions;

    public AuthService(UserManager<AppUser> userManager, AppDbContext context, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResultData> LoginAsync(LoginData data, CancellationToken ct = default)
    {
        // Nem létező névnél is ugyanaz az üzenet, hogy ne lehessen felhasználóneveket kifürkészni
        var user = await _userManager.FindByNameAsync(data.UserName.Trim())
            ?? throw new AuthenticationFailedException(InvalidCredentialsMessage);

        if (await _userManager.IsLockedOutAsync(user))
            throw new AuthenticationFailedException("Túl sok sikertelen próbálkozás, a fiók néhány percre zárolva van.");

        if (!await _userManager.CheckPasswordAsync(user, data.Password))
        {
            // Az Identity lockout beállításai szerint (alapból 5 hiba után 5 perc zárolás)
            await _userManager.AccessFailedAsync(user);
            throw new AuthenticationFailedException(InvalidCredentialsMessage);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var pages = await GetRoleClaimValuesAsync(roles, AppPages.ClaimType, ct);
        var actions = await GetRoleClaimValuesAsync(roles, AppActions.ClaimType, ct);

        var currentUser = new CurrentUserData
        {
            Id = user.Id,
            UserName = user.UserName!,
            DisplayName = user.DisplayName,
            Roles = [.. roles.Order()],
            Pages = [.. AppPages.All.Where(pages.Contains)],
            // Csak az elérhető oldalak műveletei számítanak
            Actions = [.. AppActions.All.Where(a => actions.Contains(a) && pages.Contains(AppActions.PageOf(a)))],
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes);
        return new LoginResultData
        {
            Token = CreateToken(currentUser, expiresAt),
            ExpiresAt = expiresAt,
            User = currentUser,
        };
    }

    public CurrentUserData GetCurrentUser(ClaimsPrincipal principal)
    {
        return new CurrentUserData
        {
            Id = principal.FindFirstValue(AppClaimTypes.UserId) ?? string.Empty,
            UserName = principal.FindFirstValue(AppClaimTypes.UserName) ?? string.Empty,
            DisplayName = principal.FindFirstValue(AppClaimTypes.DisplayName) ?? string.Empty,
            Roles = [.. principal.FindAll(AppClaimTypes.Role).Select(c => c.Value)],
            Pages = [.. principal.FindAll(AppClaimTypes.Page).Select(c => c.Value)],
            Actions = [.. principal.FindAll(AppClaimTypes.Action).Select(c => c.Value)],
        };
    }

    // A szerepkörök adott típusú claim-jeinek uniója (oldalak vagy műveletek)
    private async Task<List<string>> GetRoleClaimValuesAsync(IList<string> roles, string claimType, CancellationToken ct)
    {
        return await _context.RoleClaims
            .Where(rc => rc.ClaimType == claimType)
            .Join(_context.Roles, rc => rc.RoleId, r => r.Id, (rc, r) => new { r.Name, rc.ClaimValue })
            .Where(x => roles.Contains(x.Name!))
            .Select(x => x.ClaimValue!)
            .Distinct()
            .ToListAsync(ct);
    }

    private string CreateToken(CurrentUserData user, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(AppClaimTypes.UserId, user.Id),
            new(AppClaimTypes.UserName, user.UserName),
            new(AppClaimTypes.DisplayName, user.DisplayName),
        };
        claims.AddRange(user.Roles.Select(r => new Claim(AppClaimTypes.Role, r)));
        claims.AddRange(user.Pages.Select(p => new Claim(AppClaimTypes.Page, p)));
        claims.AddRange(user.Actions.Select(a => new Claim(AppClaimTypes.Action, a)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        });
    }
}
