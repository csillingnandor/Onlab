using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _context.Customers.AnyAsync(c => c.Id == id, ct);
    }
}
