namespace IOMS.DTO;

public class ProductData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    // Összkészlet: a raktáronkénti mennyiségek összege
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OrderedLast7Days { get; set; }
    public decimal? DaysOfCover { get; set; }

    // Raktáronkénti bontás (csak azok a raktárak, ahol van nyilvántartott készlet), raktárnév szerint
    public List<ProductStockData> Stocks { get; set; } = [];
}

public class ProductStockData
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class ModifyProductData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int MinStockLevel { get; set; }
    public decimal Price { get; set; }
}
