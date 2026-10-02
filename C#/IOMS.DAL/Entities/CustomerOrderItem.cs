using System.ComponentModel.DataAnnotations.Schema;

namespace IOMS.DAL.Entities;

public class CustomerOrderItem
{
    public int Id { get; set; }

    public int CustomerOrderId { get; set; }
    public CustomerOrder CustomerOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
}
