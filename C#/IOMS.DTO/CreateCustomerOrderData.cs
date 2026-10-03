using System.ComponentModel.DataAnnotations;

namespace IOMS.DTO;

public class CreateCustomerOrderData
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [MinLength(1)]
    public List<CreateCustomerOrderItemData> Items { get; set; } = new List<CreateCustomerOrderItemData>();
}

public class CreateCustomerOrderItemData
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 10_000)]
    public int Quantity { get; set; }
}
