namespace IOMS.DTO;

public class SupplierData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    // Számított: a beszállító rendeléseinek száma (amíg van, nem törölhető)
    public int OrderCount { get; set; }
}
