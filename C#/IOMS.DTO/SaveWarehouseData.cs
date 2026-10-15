namespace IOMS.DTO;

// Raktár felvétele és módosítása; validáció: IOMS.API.Validation.SaveWarehouseDataValidator
public class SaveWarehouseData
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // Mindkettő vagy egyik sem (térképes megjelenítéshez)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
