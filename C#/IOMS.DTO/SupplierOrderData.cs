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
