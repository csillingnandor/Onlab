namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.CreateUserDataValidator
public class CreateUserData
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    
}
