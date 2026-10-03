using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class CustomerOrderItemRepository : ICustomerOrderItemRepository
{
    private readonly AppDbContext _context;

    public CustomerOrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerOrderItemData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(DataProjections.CustomerOrderItem)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerOrderItemData>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Where(i => i.CustomerOrderId == orderId)
            .OrderBy(i => i.Id)
            .Select(DataProjections.CustomerOrderItem)
            .ToListAsync(ct);
    }
}
