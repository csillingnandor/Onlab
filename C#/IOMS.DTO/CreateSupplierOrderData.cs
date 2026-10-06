namespace IOMS.DTO;

// Validáció: IOMS.BLL.Validation.CreateSupplierOrderDataValidator
public class CreateSupplierOrderData
{
    public int SupplierId { get; set; }
    public List<CreateSupplierOrderItemData> Items { get; set; } = new List<CreateSupplierOrderItemData>();
}

public class CreateSupplierOrderItemData
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // A beszállítóval egyeztetett egységár (nem a termék eladási ára)
    public decimal UnitCost { get; set; }
}
