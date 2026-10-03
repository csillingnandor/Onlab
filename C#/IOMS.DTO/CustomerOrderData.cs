namespace IOMS.DTO;

public class CustomerOrderData
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CustomerOrderItemData> Items { get; set; } = new List<CustomerOrderItemData>();
}
