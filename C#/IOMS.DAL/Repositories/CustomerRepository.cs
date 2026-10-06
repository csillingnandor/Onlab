using IOMS.DAL.Entities;
using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(DataProjections.Customer)
            .ToListAsync(ct);
    }

    public async Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(DataProjections.Customer)
            .SingleOrDefaultAsync(ct);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _context.Customers.AnyAsync(c => c.Id == id, ct);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return _context.Customers.AnyAsync(c => c.Email == email, ct);
    }

    public async Task<CustomerData> CreateAsync(CreateCustomerData data, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            Name = data.Name,
            Email = data.Email,
            Phone = data.Phone,
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);

        return (await GetByIdAsync(customer.Id, ct))!;
    }
}
