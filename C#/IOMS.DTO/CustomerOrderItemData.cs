namespace IOMS.DTO;

public class CustomerOrderItemData
{
    public int Id { get; set; }
    public int CustomerOrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
}
