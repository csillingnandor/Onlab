namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/LocationStockConfiguration.cs
// Egy termék mennyisége egy tárolóhelyen. A raktári összkészlet továbbra is az Inventory;
// ez csak azt bontja tovább, hol van a raktáron belül. A raktárbeli összegük legfeljebb az Inventory mennyisége,
// a különbség a depón lévő, még el nem helyezett készlet.
public class LocationStock
{
    public int LocationId { get; set; }
    public StorageLocation Location { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
