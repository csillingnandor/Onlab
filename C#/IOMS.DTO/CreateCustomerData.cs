namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.CreateCustomerDataValidator
public class CreateCustomerData
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
