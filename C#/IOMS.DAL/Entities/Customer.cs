using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IOMS.DAL.Entities;

[Index(nameof(Email), IsUnique = true)]
public class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(254)] 
    [EmailAddress]   
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; } 

    public ICollection<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
}
