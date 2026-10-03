namespace IOMS.DTO;

public class DailyProductSaleData
{
    public DateOnly Date { get; set; }
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
