using IOMS.DAL.Entities;
using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.DAL.Repositories;

public class SupplierOrderRepository : ISupplierOrderRepository
{
    private readonly AppDbContext _context;

    public SupplierOrderRepository(AppDbContext context)
    {
        _context = context;
    }

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

    public async Task<SupplierOrderData> CreateAsync(CreateSupplierOrderData data, CancellationToken ct = default)
    {
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
