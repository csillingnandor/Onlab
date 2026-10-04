namespace IOMS.DTO;

public class CustomerData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Számított mezők a rendelésekből
    public int OrderCount { get; set; }

    // Csak a kiszállított (Delivered) rendelések számítanak eladásnak
    public decimal TotalSpent { get; set; }

    // null, ha még nem rendelt
    public DateTime? LastOrderDate { get; set; }
}
