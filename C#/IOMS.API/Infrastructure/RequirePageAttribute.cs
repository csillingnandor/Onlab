using IOMS.BLL.Auth;
using Microsoft.AspNetCore.Authorization;

namespace IOMS.API.Infrastructure;

// A végpont csak bejelentkezve, és a felsorolt oldalak (AppPages) legalább egyikéhez való jogosultsággal érhető el.
// Pl. a termékek listája több oldalnak is kell (rendeléskor termékválasztó), a módosítás csak a Termékek oldalnak.
// Osztályon és metóduson is megadva mindkettőnek teljesülnie kell.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePageAttribute : AuthorizeAttribute, IAuthorizationRequirementData
{
    public IReadOnlyList<string> Pages { get; }

    public RequirePageAttribute(params string[] pages)
    {
        Pages = pages;
    }

    public IEnumerable<IAuthorizationRequirement> GetRequirements() => [new PermissionRequirement(AppClaimTypes.Page, Pages)];
}

// A végpont egy oldalon végezhető művelethez kötött (AppActions), pl. rendelés lemondása.
// A művelet-jogosultság csak az oldalával együtt kerül a tokenbe, így az oldalt külön nem kell megadni.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireActionAttribute : AuthorizeAttribute, IAuthorizationRequirementData
{
    public string Action { get; }

    public RequireActionAttribute(string action)
    {
        Action = action;
    }

    public IEnumerable<IAuthorizationRequirement> GetRequirements() => [new PermissionRequirement(AppClaimTypes.Action, [Action])];
}

// Teljesül, ha a felhasználónak van a megadott típusú claim-je a felsorolt értékek valamelyikével
public class PermissionRequirement : IAuthorizationRequirement
{
    public string ClaimType { get; }
    public IReadOnlyList<string> Values { get; }

    public PermissionRequirement(string claimType, IReadOnlyList<string> values)
    {
        ClaimType = claimType;
        Values = values;
    }
}

// Az oldal- és művelet-jogosultságok a token "page" / "action" claim-jei (bejelentkezéskor a szerepkörökből számolva)
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Claims.Any(c => c.Type == requirement.ClaimType && requirement.Values.Contains(c.Value)))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
