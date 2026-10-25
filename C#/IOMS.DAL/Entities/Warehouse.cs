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

    // Belső elrendezés: az alaprajz mérete rácscellában (1 cella = 1 m) és a depó (ki- / beszállítási pont) cellája.
    // null, ha a raktárnak nincs megadva elrendezése (ilyenkor polcai sincsenek).
    public int? LayoutWidth { get; set; }
    public int? LayoutDepth { get; set; }
    public int? DockX { get; set; }
    public int? DockY { get; set; }

    // A raktárban tárolt termékek készlete
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public ICollection<Rack> Racks { get; set; } = new List<Rack>();
}
