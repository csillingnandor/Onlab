namespace IOMS.DTO;

// Validáció: IOMS.BLL.Validation.CreateProductDataValidator
public class CreateProductData
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
}
