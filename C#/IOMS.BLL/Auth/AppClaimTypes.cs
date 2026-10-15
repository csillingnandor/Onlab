namespace IOMS.BLL.Auth;

// A tokenben szereplő claim-ek nevei (a JWT szabványos rövid nevei, átnevezés nélkül)
public static class AppClaimTypes
{
    public const string UserId = "sub";
    public const string UserName = "name";
    public const string DisplayName = "display_name";
    public const string Role = "role";
    public const string Page = AppPages.ClaimType;
    public const string Action = AppActions.ClaimType;
}
