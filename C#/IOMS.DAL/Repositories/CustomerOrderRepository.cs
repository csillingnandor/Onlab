using IOMS.DAL.Entities;
using IOMS.DAL.Mapping;
using IOMS.DAL.QueryResults;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.DAL.Repositories;

public class CustomerOrderRepository : ICustomerOrderRepository
{
    private readonly AppDbContext _context;

    public CustomerOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Select(DataProjections.CustomerOrder)
            .ToListAsync(ct);
    }

    public async Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(DataProjections.CustomerOrder)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default)
    {
        // Ugyanaz a termék többször is szerepelhet a kérésben, ezeket összevonjuk egy tétellé.
        var merged = data.Items
            .GroupBy(l => l.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(l => l.Quantity) })
            .ToList();

        var productIds = merged.Select(l => l.ProductId).ToList();
        var prices = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);

        var order = new CustomerOrder
        {
            CustomerId = data.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = merged
                .Select(l => new CustomerOrderItem
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    // Az aktuális árat rögzítjük, hogy későbbi árváltozás ne írja át a rendelést.
                    UnitPrice = prices.TryGetValue(l.ProductId, out var price)
                        ? price
                        : throw new InvalidOperationException($"Ismeretlen termék: {l.ProductId}"),
                })
                .ToList(),
        };

        _context.CustomerOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a vevő- és terméknevek is benne legyenek.
        return (await GetByIdAsync(order.Id, ct))!;
    }

    public async Task<IReadOnlyList<DailySalesResult>> GetDailySalesForProductAsync(
        int productId, DateTime from, DateTime toExclusive, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Where(i => i.ProductId == productId
                     && i.CustomerOrder.Status == OrderStatus.Delivered
                     && i.CustomerOrder.OrderDate >= from
                     && i.CustomerOrder.OrderDate < toExclusive)
            .GroupBy(i => i.CustomerOrder.OrderDate.Date)
            .Select(g => new DailySalesResult
            {
                Date = g.Key,
                Quantity = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice),
                // Egy rendelésben egy termék csak egy tételként szerepel, így tétel = rendelés.
                OrderCount = g.Count(),
            })
            .OrderBy(r => r.Date)
            .ToListAsync(ct);
    }
}
