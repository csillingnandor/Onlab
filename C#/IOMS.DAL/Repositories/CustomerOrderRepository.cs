using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class CustomerOrderRepository : ICustomerOrderRepository
{
    private readonly AppDbContext _context;

    public CustomerOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerOrder>> GetAllAsync(CancellationToken ct = default)
    {
        return await WithDetails()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);
    }

    public async Task<CustomerOrder?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await WithDetails()
            .SingleOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task AddAsync(CustomerOrder order, CancellationToken ct = default)
    {
        _context.CustomerOrders.Add(order);
        await _context.SaveChangesAsync(ct);
    }

    private IQueryable<CustomerOrder> WithDetails()
    {
        return _context.CustomerOrders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items.OrderBy(i => i.Id))
                .ThenInclude(i => i.Product);
    }
}
