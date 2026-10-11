using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

public class SupplierOrderService : ISupplierOrderService
{
    private readonly AppDbContext _context;

    public SupplierOrderService(AppDbContext context)
    {
        _context = context;
    }

    // Legújabb rendelés elöl
    public async Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.SupplierOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Select(DataProjections.SupplierOrder)
            .ToListAsync(ct);
    }

    public async Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.SupplierOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(DataProjections.SupplierOrder)
            .SingleOrDefaultAsync(ct);
    }

    // Új, Pending státuszú beszerzési rendelés; az azonos termékű sorokat összevonja (súlyozott átlagárral).
    public async Task<SupplierOrderData> CreateAsync(CreateSupplierOrderData data, CancellationToken ct = default)
    {
        if (!await _context.Suppliers.AnyAsync(s => s.Id == data.SupplierId, ct))
            throw new BusinessValidationException(nameof(data.SupplierId), $"Nincs {data.SupplierId} azonosítójú beszállító.");

        if (!await _context.Warehouses.AnyAsync(w => w.Id == data.WarehouseId, ct))
            throw new BusinessValidationException(nameof(data.WarehouseId), $"Nincs {data.WarehouseId} azonosítójú raktár.");

        var missing = await _context.Products.GetMissingIdsAsync(data.Items.Select(i => i.ProductId), ct);
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(data.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

        // Ugyanaz a termék többször is szerepelhet a kérésben; egy tétellé vonjuk össze,
        // az egységár a sorok mennyiséggel súlyozott átlaga, így a rendelés végösszege nem változik.
        var merged = data.Items
            .GroupBy(l => l.ProductId)
            .Select(g =>
            {
                var quantity = g.Sum(l => l.Quantity);
                return new SupplierOrderItem
                {
                    ProductId = g.Key,
                    Quantity = quantity,
                    UnitCost = Math.Round(g.Sum(l => l.UnitCost * l.Quantity) / quantity, 2),
                };
            })
            .ToList();

        var order = new SupplierOrder
        {
            SupplierId = data.SupplierId,
            WarehouseId = data.WarehouseId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = merged,
        };

        _context.SupplierOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a beszállító- és terméknevek is benne legyenek.
        return (await GetByIdAsync(order.Id, ct))!;
    }

    // Beérkeztetés: Delivered státusz, a tételek mennyisége a célraktár készletéhez adódik,
    // és termékenként egy Receipt mozgás kerül a naplóba. Minden egy tranzakcióban történik.
    public async Task<SupplierOrderData?> ReceiveAsync(int id, CancellationToken ct = default)
    {
        var order = await _context.SupplierOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new { o.Status, o.WarehouseId })
            .SingleOrDefaultAsync(ct);

        if (order is null)
            return null;

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new ConflictException(nameof(id), order.Status == OrderStatus.Delivered
                ? $"A {id} azonosítójú rendelés már beérkezett."
                : $"A {id} azonosítójú rendelés le van mondva, nem érkeztethető be.");

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        // Feltételes, atomi státuszváltás: ha két kérés egyszerre érkeztetné be, csak az egyik jut tovább,
        // így a készlet nem nőhet kétszer.
        var received = await _context.SupplierOrders
            .Where(o => o.Id == id && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Shipped))
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, OrderStatus.Delivered), ct);

        if (received == 0)
            throw new ConflictException(nameof(id), $"A {id} azonosítójú rendelés állapota időközben megváltozott.");

        var lines = await _context.SupplierOrderItems
            .Where(i => i.SupplierOrderId == id)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var line in lines)
        {
            // Atomi növelés (Quantity = Quantity + n), hogy egy párhuzamos mozgás ne írja felül
            var updated = await _context.Inventories
                .Where(i => i.ProductId == line.ProductId && i.WarehouseId == order.WarehouseId)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity + line.Quantity), ct);

            // Ebben a raktárban még nem volt ilyen termék
            if (updated == 0)
            {
                _context.Inventories.Add(new Inventory
                {
                    ProductId = line.ProductId,
                    WarehouseId = order.WarehouseId,
                    Quantity = line.Quantity,
                });
            }

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = line.ProductId,
                WarehouseId = order.WarehouseId,
                QuantityChange = line.Quantity,
                Type = StockMovementType.Receipt,
                SupplierOrderId = id,
                CreatedAt = now,
            });
        }

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }
}
