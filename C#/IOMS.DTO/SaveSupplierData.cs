namespace IOMS.DTO;

// Beszállító felvétele és módosítása; validáció: IOMS.API.Validation.SaveSupplierDataValidator
public class SaveSupplierData
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
