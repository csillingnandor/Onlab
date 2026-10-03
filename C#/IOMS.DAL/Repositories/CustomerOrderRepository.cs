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

    public async Task<ProductSaleStatisticsData?> GetProductSaleStatisticsAsync(
        int productId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var productName = await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => p.Name)
            .SingleOrDefaultAsync(ct);

        if (productName is null)
            return null;

        // A zárt [from, to] napokból félig nyitott [from, to+1) időintervallum, hogy a záró nap egésze benne legyen.
        var sales = await GetDailySalesForProductAsync(
            productId,
            from.ToDateTime(TimeOnly.MinValue),
            to.AddDays(1).ToDateTime(TimeOnly.MinValue),
            ct);

        var salesByDay = sales.ToDictionary(s => DateOnly.FromDateTime(s.Date));

        // Az eladás nélküli napok is szerepeljenek (0 értékkel), hogy a diagram folytonos legyen.
        var daily = new List<DailyProductSaleData>();
        for (var day = from; day <= to; day = day.AddDays(1))
        {
            salesByDay.TryGetValue(day, out var s);
            daily.Add(new DailyProductSaleData
            {
                Date = day,
                Quantity = s?.Quantity ?? 0,
                Revenue = s?.Revenue ?? 0,
                OrderCount = s?.OrderCount ?? 0,
            });
        }

        var totalQuantity = daily.Sum(d => d.Quantity);
        var totalRevenue = daily.Sum(d => d.Revenue);

        return new ProductSaleStatisticsData
        {
            ProductId = productId,
            ProductName = productName,
            From = from,
            To = to,
            TotalQuantity = totalQuantity,
            TotalRevenue = totalRevenue,
            AverageUnitPrice = totalQuantity > 0 ? totalRevenue / totalQuantity : null,
            Daily = daily,
        };
    }

    // Csak a kiszállított (Delivered) rendelések számítanak eladásnak; az intervallum [from, toExclusive).
    private async Task<IReadOnlyList<DailySalesResult>> GetDailySalesForProductAsync(
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
