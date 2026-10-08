using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly AppDbContext _context;

    public CustomerOrderService(AppDbContext context)
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

    // Új, Pending státuszú rendelés a termékek aktuális árával; az azonos termékű sorokat összevonja.
    public async Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == data.CustomerId, ct))
            throw new BusinessValidationException(nameof(data.CustomerId), $"Nincs {data.CustomerId} azonosítójú vevő.");

        var missing = await _context.Products.GetMissingIdsAsync(data.Items.Select(i => i.ProductId), ct);
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(data.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

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
                    UnitPrice = prices[l.ProductId],
                })
                .ToList(),
        };

        _context.CustomerOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a vevő- és terméknevek is benne legyenek.
        return (await GetByIdAsync(order.Id, ct))!;
    }
}
