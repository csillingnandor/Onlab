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
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = merged,
        };

        _context.SupplierOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a beszállító- és terméknevek is benne legyenek.
        return (await GetByIdAsync(order.Id, ct))!;
    }
}
