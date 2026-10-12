using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class CustomerOrderItemService : ICustomerOrderItemService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CustomerOrderItemService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CustomerOrderItemData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .ProjectTo<CustomerOrderItemData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerOrderItemData>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Where(i => i.CustomerOrderId == orderId)
            .OrderBy(i => i.Id)
            .ProjectTo<CustomerOrderItemData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
