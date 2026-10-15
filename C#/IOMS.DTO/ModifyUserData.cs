namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.ModifyUserDataValidator
// A bejelentkezési név nem módosítható.
public class ModifyUserData
{
    public string DisplayName { get; set; } = string.Empty;

    // Nem kötelező; értesítésekhez
    public string? Email { get; set; }

    // Ha meg van adva, a felhasználó jelszava erre cserélődik
    public string? NewPassword { get; set; }

    public List<string> Roles { get; set; } = [];
}
