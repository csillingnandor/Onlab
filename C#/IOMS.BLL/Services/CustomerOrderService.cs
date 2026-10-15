using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.BLL.Exceptions;
using IOMS.BLL.Notifications;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;
using StockMovementType = IOMS.DAL.Entities.StockMovementType;

namespace IOMS.BLL.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notifications;

    public CustomerOrderService(AppDbContext context, IMapper mapper, INotificationService notifications)
    {
        _context = context;
        _mapper = mapper;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ProjectTo<CustomerOrderData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .ProjectTo<CustomerOrderData>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(ct);
    }

    // Új, Pending státuszú rendelés a termékek aktuális árával; az azonos termékű sorokat összevonja.
    public async Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default)
    {
        await ValidateReferencesAsync(data, ct);

        // Ugyanaz a termék többször is szerepelhet a kérésben, ezeket összevonjuk egy tétellé.
        var merged = MergeLines(data);

        var productIds = merged.Select(l => l.ProductId).ToList();
        var prices = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);

        var order = new CustomerOrder
        {
            CustomerId = data.CustomerId,
            WarehouseId = data.WarehouseId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = merged
                .Select(l => new CustomerOrderItem
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    // Az aktuális árat rögzítjük, hogy későbbi árváltozás ne írja át a rendelést.
                    UnitPrice = prices[l.ProductId],
                })
                .ToList(),
        };

        _context.CustomerOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a vevő- és terméknevek is benne legyenek.
        var created = (await GetByIdAsync(order.Id, ct))!;
        await _notifications.CustomerOrderCreatedAsync(created, ct);
        return created;
    }

    public async Task<CustomerOrderData?> ModifyAsync(int id, CreateCustomerOrderData data, CancellationToken ct = default)
    {
        var status = await _context.CustomerOrders
            .Where(o => o.Id == id)
            .Select(o => (OrderStatus?)o.Status)
            .SingleOrDefaultAsync(ct);
        if (status is null)
            return null;
        if (status != OrderStatus.Pending)
            throw StatusConflict(id, status.Value, "nem módosítható");

        await ValidateReferencesAsync(data, ct);
        var merged = MergeLines(data);

        // A már rendelt termékek az eredeti egységárukat tartják meg, az újak az aktuális árat kapják
        var existingPrices = await _context.CustomerOrderItems
            .Where(i => i.CustomerOrderId == id)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, UnitPrice = g.Max(i => i.UnitPrice) })
            .ToDictionaryAsync(x => x.ProductId, x => x.UnitPrice, ct);
        var newIds = merged.Select(l => l.ProductId).Where(pid => !existingPrices.ContainsKey(pid)).ToList();
        var currentPrices = await _context.Products
            .Where(p => newIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        // Feltételes módosítás: ha közben kiszállították / lemondták, nem írjuk át
        var updated = await _context.CustomerOrders
            .Where(o => o.Id == id && o.Status == OrderStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(o => o.CustomerId, data.CustomerId)
                .SetProperty(o => o.WarehouseId, data.WarehouseId), ct);
        if (updated == 0)
            throw new ConflictException(nameof(id), $"A {id} azonosítójú rendelés állapota időközben megváltozott.");

        await _context.CustomerOrderItems.Where(i => i.CustomerOrderId == id).ExecuteDeleteAsync(ct);
        _context.CustomerOrderItems.AddRange(merged.Select(l => new CustomerOrderItem
        {
            CustomerOrderId = id,
            ProductId = l.ProductId,
            Quantity = l.Quantity,
            UnitPrice = existingPrices.TryGetValue(l.ProductId, out var price) ? price : currentPrices[l.ProductId],
        }));
        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    private async Task ValidateReferencesAsync(CreateCustomerOrderData data, CancellationToken ct)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == data.CustomerId, ct))
            throw new BusinessValidationException(nameof(data.CustomerId), $"Nincs {data.CustomerId} azonosítójú vevő.");

        if (!await _context.Warehouses.AnyAsync(w => w.Id == data.WarehouseId, ct))
            throw new BusinessValidationException(nameof(data.WarehouseId), $"Nincs {data.WarehouseId} azonosítójú raktár.");

        var missing = await _context.Products.GetMissingIdsAsync(data.Items.Select(i => i.ProductId), ct);
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(data.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");
    }

    private static List<(int ProductId, int Quantity)> MergeLines(CreateCustomerOrderData data) =>
        data.Items
            .GroupBy(l => l.ProductId)
            .Select(g => (g.Key, g.Sum(l => l.Quantity)))
            .ToList();

    // Kiszállítás: Pending -> Shipped. A kiszolgáló raktár készlete csökken, termékenként egy Shipment mozgás
    // kerül a naplóba. Ha bármelyik termékből nincs elég, semmi sem változik. Minden egy tranzakcióban történik.
    public async Task<CustomerOrderData?> ShipAsync(int id, CancellationToken ct = default)
    {
        var order = await _context.CustomerOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new { o.Status, o.WarehouseId, WarehouseName = o.Warehouse.Name })
            .SingleOrDefaultAsync(ct);

        if (order is null)
            return null;

        if (order.Status != OrderStatus.Pending)
            throw StatusConflict(id, order.Status, "nem szállítható ki");

        var lines = await _context.CustomerOrderItems
            .Where(i => i.CustomerOrderId == id)
            .GroupBy(i => new { i.ProductId, i.Product.Name })
            .Select(g => new { g.Key.ProductId, ProductName = g.Key.Name, Quantity = g.Sum(i => i.Quantity) })
            .ToListAsync(ct);

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        // Feltételes, atomi státuszváltás: két egyidejű kérésből csak az egyik jut tovább
        var shipped = await _context.CustomerOrders
            .Where(o => o.Id == id && o.Status == OrderStatus.Pending)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, OrderStatus.Shipped), ct);

        if (shipped == 0)
            throw new ConflictException(nameof(id), $"A {id} azonosítójú rendelés állapota időközben megváltozott.");

        // Atomi, feltételes levonás (csak ha van elég); ami nem sikerül, az hiányként kerül az üzenetbe.
        // A hibát a tranzakció visszagörgetése követi, így a már levont tételek is visszaállnak.
        var shortages = new List<string>();
        foreach (var line in lines)
        {
            var updated = await _context.Inventories
                .Where(i => i.ProductId == line.ProductId && i.WarehouseId == order.WarehouseId && i.Quantity >= line.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity - line.Quantity), ct);

            if (updated == 0)
            {
                var available = await _context.Inventories
                    .Where(i => i.ProductId == line.ProductId && i.WarehouseId == order.WarehouseId)
                    .Select(i => (int?)i.Quantity)
                    .SingleOrDefaultAsync(ct) ?? 0;
                shortages.Add($"{line.ProductName} (kell {line.Quantity} db, van {available} db)");
            }
        }

        if (shortages.Count > 0)
            throw new ConflictException(nameof(id),
                $"Nincs elég készlet ({order.WarehouseName}): {string.Join(", ", shortages)}.");

        // A 0-ra fogyott sorok törlése, hogy a raktáronkénti bontásban csak a tényleges készlet jelenjen meg
        var productIds = lines.Select(l => l.ProductId).ToList();
        await _context.Inventories
            .Where(i => i.WarehouseId == order.WarehouseId && productIds.Contains(i.ProductId) && i.Quantity == 0)
            .ExecuteDeleteAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var line in lines)
        {
            _context.StockMovements.Add(new StockMovement
            {
                ProductId = line.ProductId,
                WarehouseId = order.WarehouseId,
                QuantityChange = -line.Quantity,
                Type = StockMovementType.Shipment,
                CustomerOrderId = id,
                CreatedAt = now,
            });
        }

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        var result = (await GetByIdAsync(id, ct))!;
        await _notifications.CustomerOrderStatusChangedAsync(result, ct);
        await _notifications.StockDecreasedAsync(lines.ToDictionary(l => l.ProductId, l => l.Quantity), ct);
        return result;
    }

    // Kézbesítés: Shipped -> Delivered. Csak státuszváltás, a készlet a kiszállításkor már csökkent;
    // ettől kezdve számít a rendelés eladásnak a statisztikákban.
    public Task<CustomerOrderData?> DeliverAsync(int id, CancellationToken ct = default)
    {
        return ChangeStatusAsync(id, OrderStatus.Shipped, OrderStatus.Delivered, "nem kézbesíthető", ct);
    }

    // Lemondás: csak a még ki nem szállított (Pending) rendelés mondható le, így készletmozgás nincs.
    public Task<CustomerOrderData?> CancelAsync(int id, CancellationToken ct = default)
    {
        return ChangeStatusAsync(id, OrderStatus.Pending, OrderStatus.Cancelled, "nem mondható le", ct);
    }

    // Feltételes, atomi státuszváltás from -> to; null, ha nincs ilyen rendelés, 409, ha nem from állapotú.
    private async Task<CustomerOrderData?> ChangeStatusAsync(
        int id, OrderStatus from, OrderStatus to, string notAllowed, CancellationToken ct)
    {
        var changed = await _context.CustomerOrders
            .Where(o => o.Id == id && o.Status == from)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, to), ct);

        if (changed == 0)
        {
            var current = await _context.CustomerOrders
                .Where(o => o.Id == id)
                .Select(o => (OrderStatus?)o.Status)
                .SingleOrDefaultAsync(ct);

            if (current is null)
                return null;

            throw StatusConflict(id, current.Value, notAllowed);
        }

        var result = await GetByIdAsync(id, ct);
        if (result is not null)
            await _notifications.CustomerOrderStatusChangedAsync(result, ct);
        return result;
    }

    private static ConflictException StatusConflict(int id, OrderStatus current, string notAllowed)
    {
        var state = current switch
        {
            OrderStatus.Pending => "függőben van",
            OrderStatus.Shipped => "már ki van szállítva",
            OrderStatus.Delivered => "már kézbesítve van",
            OrderStatus.Cancelled => "le van mondva",
            _ => current.ToString(),
        };
        return new ConflictException(nameof(id), $"A {id} azonosítójú rendelés {state}, ezért {notAllowed}.");
    }
}
