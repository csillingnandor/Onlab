
namespace IOMS.DAL.Entities;

public class CustomerOrder
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // A kiszolgáló raktár: kiszállításkor ennek a raktárnak a készlete csökken
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateTime OrderDate { get; set; } 

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public ICollection<CustomerOrderItem> Items { get; set; } = new List<CustomerOrderItem>();
}
