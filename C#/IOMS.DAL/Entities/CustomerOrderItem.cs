namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/CustomerOrderItemConfiguration.cs
public class CustomerOrderItem
{
    public int Id { get; set; }

    public int CustomerOrderId { get; set; }
    public CustomerOrder CustomerOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    // A rendeléskori egységár (a termék későbbi árváltozása nem írja át)
    public decimal UnitPrice { get; set; }
}
