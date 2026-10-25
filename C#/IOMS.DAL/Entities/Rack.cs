namespace IOMS.DAL.Entities;

// Adatbázis-leképezés: Configurations/RackConfiguration.cs
// Egy polcsor a raktár alaprajzán. A rács egysége 1 m; a polc az (X, Y) cellától indul, és Length rekeszt
// foglal el az Orientation irányában. Minden rekesz Levels szintes, rekeszenként és szintenként egy tárolóhely.
public class Rack
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    // Raktáron belül egyedi, pl. "A"; a tárolóhelyek kódjának eleje
    public string Code { get; set; } = string.Empty;

    public int X { get; set; }
    public int Y { get; set; }
    public int Length { get; set; }
    public int Levels { get; set; }
    public RackOrientation Orientation { get; set; }

    public ICollection<StorageLocation> Locations { get; set; } = new List<StorageLocation>();
}
