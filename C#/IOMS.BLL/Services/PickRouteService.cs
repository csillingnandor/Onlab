using IOMS.BLL.Exceptions;
using IOMS.BLL.Warehousing;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using EntityRackOrientation = IOMS.DAL.Entities.RackOrientation;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

// A raktári felvétel adatbázis-oldala: betölti az elrendezést és a helyenkénti készletet, a tervezést
// a Warehousing osztályai végzik (PickPlanner: honnan, RoutePlanner: milyen sorrendben).
public class PickRouteService : IPickRouteService
{
    private readonly AppDbContext _context;

    public PickRouteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PickRouteData?> GetForOrderAsync(int orderId, CancellationToken ct = default)
    {
        var order = await _context.CustomerOrders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new { o.Status, o.WarehouseId, WarehouseName = o.Warehouse.Name })
            .SingleOrDefaultAsync(ct);
        if (order is null)
            return null;

        if (order.Status != OrderStatus.Pending)
            throw new ConflictException(nameof(orderId), "Felvételi útvonal csak függő (még ki nem szállított) rendeléshez tervezhető.");

        var quantities = await OrderQuantitiesAsync(orderId, ct);
        var context = await LoadAsync(order.WarehouseId, quantities.Keys, ct)
            ?? throw new BusinessValidationException(nameof(orderId), $"{order.WarehouseName}: nincs megadva belső elrendezés.");

        var plan = PickPlanner.Plan(quantities, context.Candidates, context.DockQuantities, context.FromDepot);

        // Megállónként a felvételek; a megálló a hely elérési cellája
        var locationPicks = plan.Picks.Where(p => p.LocationId is not null).ToList();
        var stops = locationPicks.Select(p => context.AccessPoints[p.LocationId!.Value]).Distinct().ToList();
        var route = RoutePlanner.Plan(context.Grid, context.Depot, stops);

        var productIds = quantities.Keys.ToList();
        var names = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name, ct);

        PickLineData Line(int? locationId, int productId, int quantity) => new()
        {
            LocationId = locationId,
            LocationCode = locationId is { } id ? context.LocationCodes[id] : null,
            ProductId = productId,
            ProductName = names.GetValueOrDefault(productId, $"#{productId}"),
            Quantity = quantity,
        };

        return new PickRouteData
        {
            OrderId = orderId,
            WarehouseId = order.WarehouseId,
            Distance = route.Distance,
            Path = [.. route.Path.Select(p => new GridPointData { X = p.X, Y = p.Y })],
            Stops = [.. route.Order.Select((stopIndex, sequence) =>
            {
                var point = stops[stopIndex];
                return new PickStopData
                {
                    Sequence = sequence + 1,
                    X = point.X,
                    Y = point.Y,
                    Picks = [.. locationPicks
                        .Where(p => context.AccessPoints[p.LocationId!.Value] == point)
                        .OrderBy(p => context.LocationCodes[p.LocationId!.Value])
                        .Select(p => Line(p.LocationId, p.ProductId, p.Quantity))],
                };
            })],
            FromDock = [.. plan.Picks.Where(p => p.LocationId is null).Select(p => Line(null, p.ProductId, p.Quantity))],
            Shortages = [.. plan.Shortages.Select(s => Line(null, s.Key, s.Value))],
        };
    }

    public async Task<PickPlan?> PlanPicksAsync(int warehouseId, IReadOnlyDictionary<int, int> quantityByProduct, CancellationToken ct = default)
    {
        var context = await LoadAsync(warehouseId, quantityByProduct.Keys, ct);
        return context is null
            ? null
            : PickPlanner.Plan(quantityByProduct, context.Candidates, context.DockQuantities, context.FromDepot);
    }

    private async Task<Dictionary<int, int>> OrderQuantitiesAsync(int orderId, CancellationToken ct) =>
        await _context.CustomerOrderItems
            .Where(i => i.CustomerOrderId == orderId)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Quantity, ct);

    private sealed record PlanningContext(
        WarehouseGrid Grid,
        GridPoint Depot,
        PathTree FromDepot,
        List<LocationCandidate> Candidates,
        Dictionary<int, int> DockQuantities,
        Dictionary<int, GridPoint> AccessPoints,
        Dictionary<int, string> LocationCodes);

    // Az elrendezés és a szóba jöhető termékek helyenkénti készlete; null, ha a raktárnak nincs elrendezése
    private async Task<PlanningContext?> LoadAsync(int warehouseId, IEnumerable<int> productIds, CancellationToken ct)
    {
        var ids = productIds.ToList();

        var layout = await _context.Warehouses
            .AsNoTracking()
            .Where(w => w.Id == warehouseId)
            .Select(w => new { w.LayoutWidth, w.LayoutDepth, w.DockX, w.DockY })
            .SingleOrDefaultAsync(ct);
        if (layout?.LayoutWidth is not { } width || layout.LayoutDepth is not { } depth)
            return null;

        var racks = await _context.Racks
            .AsNoTracking()
            .Where(r => r.WarehouseId == warehouseId)
            .Select(r => new { r.Id, r.X, r.Y, r.Length, r.Orientation })
            .ToListAsync(ct);
        var shapes = racks.ToDictionary(
            r => r.Id,
            r => new RackShape(r.X, r.Y, r.Length, r.Orientation == EntityRackOrientation.Horizontal));

        var grid = new WarehouseGrid(width, depth, shapes.Values);
        var depot = new GridPoint(layout.DockX ?? 0, layout.DockY ?? 0);
        var fromDepot = grid.ShortestPaths(depot);

        var stocks = await _context.LocationStocks
            .AsNoTracking()
            .Where(s => s.Location.Rack.WarehouseId == warehouseId && ids.Contains(s.ProductId))
            .Select(s => new { s.LocationId, s.ProductId, s.Quantity, s.Location.RackId, s.Location.Bay, s.Location.Code })
            .ToListAsync(ct);

        var accessPoints = new Dictionary<int, GridPoint>();
        var codes = new Dictionary<int, string>();
        var candidates = new List<LocationCandidate>();
        foreach (var s in stocks)
        {
            var access = grid.AccessPoint(shapes[s.RackId], s.Bay);
            if (access is { } point)
                accessPoints[s.LocationId] = point;
            codes[s.LocationId] = s.Code;
            candidates.Add(new LocationCandidate(s.LocationId, s.ProductId, s.Quantity, access));
        }

        // A depón az van, ami a raktári készletből nincs helyhez rendelve
        var inventory = await _context.Inventories
            .Where(i => i.WarehouseId == warehouseId && ids.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId, i => i.Quantity, ct);
        var dock = ids.ToDictionary(
            id => id,
            id => Math.Max(0, inventory.GetValueOrDefault(id) - stocks.Where(s => s.ProductId == id).Sum(s => s.Quantity)));

        return new PlanningContext(grid, depot, fromDepot, candidates, dock, accessPoints, codes);
    }
}
