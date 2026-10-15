namespace IOMS.BLL.Auth;

// appsettings "Seed:Users" elemei: induláskor létrejönnek, ha még nincs ilyen nevű felhasználó
public class SeedUserOptions
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    
}
