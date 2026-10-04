namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/WarehouseConfiguration.cs
public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // null, ha nincs megadva hely
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
