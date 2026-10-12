namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.CreateCustomerOrderDataValidator
public class CreateCustomerOrderData
{
    public int CustomerId { get; set; }

    // Kiszolgáló raktár, ahonnan a rendelést kiszállítják
    public int WarehouseId { get; set; }

    public List<CreateCustomerOrderItemData> Items { get; set; } = new List<CreateCustomerOrderItemData>();
}

public class CreateCustomerOrderItemData
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
