namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/ProductConfiguration.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
}
