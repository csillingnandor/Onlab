namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/ProductConfiguration.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
    public ICollection<CustomerOrderItem> OrderItems { get; set; } = new List<CustomerOrderItem>();

    // Raktáronkénti készlet; a termék összkészlete ezek összege
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
