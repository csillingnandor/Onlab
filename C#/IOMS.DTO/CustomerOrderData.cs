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

public class CustomerOrderItemData
{
    public int Id { get; set; }
    public int CustomerOrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
}
