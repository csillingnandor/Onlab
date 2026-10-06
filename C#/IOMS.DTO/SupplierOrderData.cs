namespace IOMS.DTO;

public class SupplierOrderData
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalCost { get; set; }
    public List<SupplierOrderItemData> Items { get; set; } = new List<SupplierOrderItemData>();
}

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
