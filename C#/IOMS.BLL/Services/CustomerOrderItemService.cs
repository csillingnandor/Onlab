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

    public async Task<IReadOnlyList<CustomerOrderItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Select(DtoProjections.CustomerOrderItem)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Where(i => i.CustomerOrderId == orderId)
            .Select(DtoProjections.CustomerOrderItem)
            .ToListAsync(ct);
    }
}
