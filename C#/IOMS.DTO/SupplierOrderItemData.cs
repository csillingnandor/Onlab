namespace IOMS.DTO;

public class SupplierOrderItemData
{
    public int Id { get; set; }
    public int SupplierOrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    // A beszállítónak fizetett egységár
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; }
}
