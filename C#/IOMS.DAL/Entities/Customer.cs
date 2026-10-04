namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/CustomerConfiguration.cs
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public ICollection<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
}
