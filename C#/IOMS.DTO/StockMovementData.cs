namespace IOMS.DTO;

// Egy termék készletváltozása (készlettörténet)
public class StockMovementData
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    // Előjeles változás: + beérkezés / nyitókészlet, − kiszállítás
    public int QuantityChange { get; set; }
    public StockMovementType Type { get; set; }

    // A mozgást kiváltó rendelés (Receipt: beszállítói, Shipment: vevői; Opening-nál egyik sem)
    public int? SupplierOrderId { get; set; }
    public int? CustomerOrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Kézi korrekció indoka
    public string? Note { get; set; }
}

// Az API-n kiadott mozgástípus; szándékosan független a DAL entitás enumjától.
public enum StockMovementType
{
    Opening,
    Receipt,
    Shipment,
    Adjustment,
}
