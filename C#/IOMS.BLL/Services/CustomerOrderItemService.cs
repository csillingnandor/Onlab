using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class CustomerOrderItemService : ICustomerOrderItemService
{
    private readonly AppDbContext _context;

    public CustomerOrderItemService(AppDbContext context)
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
