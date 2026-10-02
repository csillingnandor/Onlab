using System.ComponentModel.DataAnnotations;

namespace IOMS.DTO;

public record CreateProductDto(
    [Required, MaxLength(100)] string Name,
    [Required, MaxLength(50)] string SKU,
    [MaxLength(50)] string? Category,
    [Range(0, int.MaxValue)] int StockQuantity,
    [Range(0, int.MaxValue)] int MinStockLevel,
    [Range(0, 999_999_999)] decimal Price);
