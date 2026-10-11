
namespace IOMS.DAL.Entities;

public class SupplierOrder
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    // A célraktár: beérkezéskor ennek a raktárnak a készlete nő
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public ICollection<SupplierOrderItem> Items { get; set; } = new List<SupplierOrderItem>();
}
