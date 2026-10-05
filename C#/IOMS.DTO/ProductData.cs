namespace IOMS.DTO;

public class ProductData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OrderedLast7Days { get; set; }
    public decimal? DaysOfCover { get; set; }
}
