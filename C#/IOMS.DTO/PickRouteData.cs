namespace IOMS.DTO;

// Egy függő vevői rendelés felvételi körútja a kiszolgáló raktárban: a depóból indul, a megállókat
// a megadott sorrendben érinti, és a depóba tér vissza. Koordináták: a raktár rácsán (WarehouseLayoutData).
public class PickRouteData
{
    public int OrderId { get; set; }
    public int WarehouseId { get; set; }

    // A körút hossza méterben (rácslépésekben)
    public int Distance { get; set; }

    // A teljes útvonal cellánként, a depótól a depóig
    public List<GridPointData> Path { get; set; } = [];

    // Bejárási sorrendben
    public List<PickStopData> Stops { get; set; } = [];

    // A depón lévő, még el nem helyezett készletből (nem kell érte bemenni a raktárba)
    public List<PickLineData> FromDock { get; set; } = [];

    // Ami sehonnan nem jön össze (a kiszállítás ilyenkor nem lehetséges)
    public List<PickLineData> Shortages { get; set; } = [];
}

public class GridPointData
{
    public int X { get; set; }
    public int Y { get; set; }
}

// Egy megálló a folyosón: innen a polc egy rekeszének (akár több szintjének) termékei vehetők fel
public class PickStopData
{
    public int Sequence { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public List<PickLineData> Picks { get; set; } = [];
}

public class PickLineData
{
    // null: depóról / hiány
    public int? LocationId { get; set; }
    public string? LocationCode { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
