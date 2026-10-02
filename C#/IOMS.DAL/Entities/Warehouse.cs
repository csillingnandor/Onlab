using System.ComponentModel.DataAnnotations;

namespace IOMS.DAL.Entities;

public class Warehouse
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
