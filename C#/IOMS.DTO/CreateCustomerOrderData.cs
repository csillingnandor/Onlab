namespace IOMS.DTO;

// Validáció: IOMS.BLL.Validation.CreateCustomerOrderDataValidator
public class CreateCustomerOrderData
{
    public int CustomerId { get; set; }
    public List<CreateCustomerOrderItemData> Items { get; set; } = new List<CreateCustomerOrderItemData>();
}

public class CreateCustomerOrderItemData
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
