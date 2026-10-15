namespace IOMS.DAL.Entities;

public enum StockMovementType
{
    // Nyitókészlet (a kezdeti adatok betöltésekor)
    Opening,

    // Beszállítói rendelés beérkezése (+)
    Receipt,

    // Vevői rendelés kiszállítása (−)
    Shipment,

    // Kézi készletkorrekció (leltáreltérés, sérülés...; + vagy −), megjegyzéssel
    Adjustment,
}
