using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    // Név szerint rendezve, a rendelésekből számított mezőkkel
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

    public async Task<CustomerData> CreateAsync(CreateCustomerData data, CancellationToken ct = default)
    {
        // Felesleges szóközök nélkül mentünk; az üres telefonszám null.
        var customer = new Customer
        {
            Name = data.Name.Trim(),
            Email = data.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(data.Phone) ? null : data.Phone.Trim(),
        };

        // Az e-mail egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _context.Customers.AnyAsync(c => c.Email == customer.Email, ct))
            throw new BusinessValidationException(nameof(data.Email), $"Már létezik vevő ezzel az e-mail címmel: {customer.Email}");

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);

        return (await GetByIdAsync(customer.Id, ct))!;
    }
}
