using System.ComponentModel.DataAnnotations.Schema;

namespace IOMS.DAL.Entities;

public class SupplierOrderItem
{
    public int Id { get; set; }

    public int SupplierOrderId { get; set; }
    public SupplierOrder SupplierOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    // What we paid the supplier per unit
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }
}
