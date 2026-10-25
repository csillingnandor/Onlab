namespace IOMS.DTO;

// Merre fut a polc a raktár rácsán; szándékosan független a DAL entitás enumjától
public enum RackOrientation
{
    Horizontal,
    Vertical
}

// Egy raktár belső elrendezése a 3D nézethez. A rács egysége 1 m, az origó az alaprajz bal felső sarka,
// X jobbra, Y "lefelé" (a bejárat felé) nő.
public class WarehouseLayoutData
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    // false: a raktárnak nincs megadva elrendezése (a méretek ilyenkor 0-k, a lista üres)
    public bool HasLayout { get; set; }
    public int Width { get; set; }
    public int Depth { get; set; }
    public int DockX { get; set; }
    public int DockY { get; set; }

    // Kód szerint
    public List<RackData> Racks { get; set; } = [];

    // A raktár készletéből az, ami még nincs tárolóhelyhez rendelve (a depón van)
    public List<LocationStockData> UnplacedStock { get; set; } = [];
}

public class RackData
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Length { get; set; }
    public int Levels { get; set; }
    public RackOrientation Orientation { get; set; }

    // Rekesz, azon belül szint szerint
    public List<StorageLocationData> Locations { get; set; } = [];
}

public class StorageLocationData
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Bay { get; set; }
    public int Level { get; set; }
    public int MaxQuantity { get; set; }

    // A helyen lévő összes darab (a Stocks összege)
    public int Quantity { get; set; }
    public List<LocationStockData> Stocks { get; set; } = [];
}

public class LocationStockData
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
