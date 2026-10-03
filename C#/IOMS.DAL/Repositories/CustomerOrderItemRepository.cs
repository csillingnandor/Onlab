using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class CustomerOrderItemRepository : ICustomerOrderItemRepository
{
    private readonly AppDbContext _context;

    public CustomerOrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerOrderItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Include(i => i.Product)
            .OrderBy(i => i.Id)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerOrderItem>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Where(i => i.CustomerOrderId == orderId)
            .OrderBy(i => i.Id)
            .ToListAsync(ct);
    }
}
