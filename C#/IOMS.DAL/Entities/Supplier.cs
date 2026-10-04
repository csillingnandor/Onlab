namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/SupplierConfiguration.cs
public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public ICollection<SupplierOrder> Orders { get; set; } = new List<SupplierOrder>();
}
