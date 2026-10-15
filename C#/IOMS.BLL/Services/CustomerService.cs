using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CustomerService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Név szerint rendezve, a rendelésekből számított mezőkkel
    public async Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ProjectTo<CustomerData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .ProjectTo<CustomerData>(_mapper.ConfigurationProvider)
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

    public async Task<CustomerData?> ModifyAsync(int id, CreateCustomerData data, CancellationToken ct = default)
    {
        var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Id == id, ct);
        if (customer is null)
            return null;

        var email = data.Email.Trim();
        if (await _context.Customers.AnyAsync(c => c.Email == email && c.Id != id, ct))
            throw new BusinessValidationException(nameof(data.Email), $"Már létezik vevő ezzel az e-mail címmel: {email}");

        customer.Name = data.Name.Trim();
        customer.Email = email;
        customer.Phone = string.IsNullOrWhiteSpace(data.Phone) ? null : data.Phone.Trim();
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    // A rendelések (és így az eladási statisztikák) nem veszhetnek el, ezért rendeléssel rendelkező vevő nem törölhető
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == id)
            .Select(c => new { c.Name, OrderCount = c.Orders.Count() })
            .SingleOrDefaultAsync(ct);
        if (customer is null)
            return false;

        if (customer.OrderCount > 0)
            throw new ConflictException(nameof(id),
                $"{customer.Name} nem törölhető, mert {customer.OrderCount} rendelése van.");

        await _context.Customers.Where(c => c.Id == id).ExecuteDeleteAsync(ct);
        return true;
    }
}
