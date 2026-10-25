namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/StorageLocationConfiguration.cs
// Egy tárolóhely: a polc Bay-edik rekeszének Level-edik szintje (mindkettő 1-től számozva)
public class StorageLocation
{
    public int Id { get; set; }

    public int RackId { get; set; }
    public Rack Rack { get; set; } = null!;

    public int Bay { get; set; }
    public int Level { get; set; }

    // Pl. "A-03-2": polc, rekesz, szint
    public string Code { get; set; } = string.Empty;

    // Ennyi darab fér el a helyen (termékfüggetlen, egyszerűsített kapacitás)
    public int MaxQuantity { get; set; }

    public ICollection<LocationStock> Stocks { get; set; } = new List<LocationStock>();
}
