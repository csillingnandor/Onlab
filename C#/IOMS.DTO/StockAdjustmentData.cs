namespace IOMS.DTO;

// Kézi készletkorrekció egy termékre és raktárra; validáció: IOMS.API.Validation.StockAdjustmentDataValidator
// A ténylegesen megszámolt mennyiséget kell megadni, a különbség kerül a naplóba.
public class StockAdjustmentData
{
    public int WarehouseId { get; set; }
    public int CountedQuantity { get; set; }
    public string? Note { get; set; }
}
