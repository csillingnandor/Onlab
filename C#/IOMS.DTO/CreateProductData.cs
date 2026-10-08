namespace IOMS.DTO;

// Validáció: IOMS.API.Validation.CreateProductDataValidator
// Új termék 0 készlettel jön létre; a raktáronkénti készletet a beszállítói rendelések beérkezése növeli.
public class CreateProductData
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
}
