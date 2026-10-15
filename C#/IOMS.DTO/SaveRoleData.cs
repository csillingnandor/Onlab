namespace IOMS.DTO;

// Létrehozáshoz és módosításhoz is; validáció: IOMS.API.Validation.SaveRoleDataValidator
public class SaveRoleData
{
    public string Name { get; set; } = string.Empty;
    public List<string> Pages { get; set; } = [];

    // A kiválasztott oldalakon engedélyezett műveletek (pl. "customer-orders.cancel")
    public List<string> Actions { get; set; } = [];
}
