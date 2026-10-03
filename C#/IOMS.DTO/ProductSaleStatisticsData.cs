namespace IOMS.DTO;

public class ProductSaleStatisticsData
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    // Zárt intervallum: a To napja is benne van.
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }

    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }

    // null, ha az időszakban nem volt eladás.
    public decimal? AverageUnitPrice { get; set; }

    public List<DailyProductSaleData> Daily { get; set; } = new List<DailyProductSaleData>();
}
