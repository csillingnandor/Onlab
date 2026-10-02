using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly AppDbContext _context;

    public CustomerOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerOrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Select(DtoProjections.CustomerOrder)
            .ToListAsync(ct);
    }

    public async Task<CustomerOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.CustomerOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(DtoProjections.CustomerOrder)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<CustomerOrderDto> CreateAsync(CreateCustomerOrderDto dto, CancellationToken ct = default)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId, ct))
            throw new BusinessValidationException(nameof(dto.CustomerId), $"Nincs {dto.CustomerId} azonosítójú vevő.");

        // Ugyanaz a termék többször is szerepelhet a kérésben, ezeket összevonjuk egy tétellé.
        var lines = dto.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToList();

        var productIds = lines.Select(l => l.ProductId).ToList();
        var prices = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);

        var missing = productIds.Where(id => !prices.ContainsKey(id)).ToList();
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(dto.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

        var order = new CustomerOrder
        {
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = EntityOrderStatus.Pending,
            Items = lines
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

        return (await GetByIdAsync(order.Id, ct))!;
    }
}
