namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/StockMovementConfiguration.cs
// Egy készletváltozás naplója. Az Inventory a pillanatnyi állapot, ez pedig az oka:
// egy termék egy raktárban lévő mozgásainak összege mindig egyenlő az Inventory.Quantity-vel.
// Csak hozzáadni szabad, módosítani és törölni nem.
public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    // Előjeles változás: + beérkezés, − kiszállítás
    public int QuantityChange { get; set; }

    public StockMovementType Type { get; set; }

    // A mozgást kiváltó rendelés (Receipt esetén beszállítói, Shipment esetén vevői); Opening-nál egyik sem
    public int? SupplierOrderId { get; set; }
    public SupplierOrder? SupplierOrder { get; set; }

    public int? CustomerOrderId { get; set; }
    public CustomerOrder? CustomerOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
