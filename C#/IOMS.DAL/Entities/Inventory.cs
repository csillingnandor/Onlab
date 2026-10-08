namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/InventoryConfiguration.cs
// Egy termék készlete egy adott raktárban
public class Inventory
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int Quantity { get; set; }
}