namespace IOMS.DAL.QueryResults;

// Lekérdezés eredménye (projekció), nem entitás: nincs mögötte tábla.
public class DailySalesResult
{
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
