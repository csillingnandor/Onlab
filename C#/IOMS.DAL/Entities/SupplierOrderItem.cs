namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/SupplierOrderItemConfiguration.cs
public class SupplierOrderItem
{
    public int Id { get; set; }

    public int SupplierOrderId { get; set; }
    public SupplierOrder SupplierOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    // A beszállítónak fizetett egységár
    public decimal UnitCost { get; set; }
}
