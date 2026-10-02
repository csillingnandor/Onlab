using System.ComponentModel.DataAnnotations;

namespace IOMS.DAL.Entities;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(254)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    public ICollection<SupplierOrder> Orders { get; set; } = new List<SupplierOrder>();
}
