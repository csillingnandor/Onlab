namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.CreateSupplierOrderDataValidator
public class CreateSupplierOrderData
{
    public int SupplierId { get; set; }

    // Célraktár, ahová a rendelés beérkezik
    public int WarehouseId { get; set; }

    public List<CreateSupplierOrderItemData> Items { get; set; } = new List<CreateSupplierOrderItemData>();
}

public class CreateSupplierOrderItemData
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // A beszállítóval egyeztetett egységár (nem a termék eladási ára)
    public decimal UnitCost { get; set; }
}
