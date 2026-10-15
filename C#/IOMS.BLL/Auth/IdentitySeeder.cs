using System.Security.Claims;
using IOMS.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IOMS.BLL.Auth;

// Induláskor: hiányzó alap szerepkörök és a konfigurált felhasználók létrehozása.
// A meglévőkhöz nem nyúl (a Felhasználók oldalon tett módosítások megmaradnak), kivéve az adminisztrátor oldalait és műveleteit,
// amelyek mindig az összesre bővülnek (így egy újonnan felvett oldal vagy művelet is azonnal elérhető neki).
// (A jelszó-hash minden generáláskor más lenne, ezért ezek nem HasData-val, hanem futásidőben kerülnek be.)
public static class IdentitySeeder
{
    public static async Task SeedIdentityAsync(this IServiceProvider services, IEnumerable<SeedUserOptions> users)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (var (roleName, defaults) in AppRoles.Defaults)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            var existingClaims = new List<Claim>();

            if (role is null)
            {
                role = new IdentityRole(roleName);
                await EnsureSucceeded(roleManager.CreateAsync(role), roleName);
            }
            else if (roleName == AppRoles.Admin)
            {
                existingClaims = [.. await roleManager.GetClaimsAsync(role)];
            }
            else
            {
                continue;
            }

            var claims = defaults.Pages.Select(page => new Claim(AppPages.ClaimType, page))
                .Concat(defaults.Actions.Select(action => new Claim(AppActions.ClaimType, action)));
            foreach (var claim in claims.Where(c => !existingClaims.Any(e => e.Type == c.Type && e.Value == c.Value)))
                await EnsureSucceeded(roleManager.AddClaimAsync(role, claim), roleName);
        }

        foreach (var seedUser in users)
        {
            if (await userManager.FindByNameAsync(seedUser.UserName) is not null)
                continue;

            var user = new AppUser { UserName = seedUser.UserName, DisplayName = seedUser.DisplayName };
            await EnsureSucceeded(userManager.CreateAsync(user, seedUser.Password), seedUser.UserName);
            if (seedUser.Roles.Count > 0)
                await EnsureSucceeded(userManager.AddToRolesAsync(user, seedUser.Roles), seedUser.UserName);
        }
    }

    private static async Task EnsureSucceeded(Task<IdentityResult> operation, string subject)
    {
        var result = await operation;
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Kezdeti adatok létrehozása sikertelen ({subject}): {string.Join(" ", result.Errors.Select(e => e.Description))}");
    }
}
