using System.ComponentModel.DataAnnotations;

namespace IOMS.DTO;

public class CreateProductData
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Category { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue)]
    public int MinStockLevel { get; set; }

    [Range(0, 999_999_999)]
    public decimal Price { get; set; }
}
