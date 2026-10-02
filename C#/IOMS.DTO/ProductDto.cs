namespace IOMS.DTO;

public record ProductDto(
    int Id,
    string Name,
    string SKU,
    string Category,
    int StockQuantity,
    int MinStockLevel,
    decimal Price,
    string Status);
