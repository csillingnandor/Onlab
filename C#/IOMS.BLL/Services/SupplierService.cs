using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SupplierService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ProjectTo<SupplierData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<SupplierData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .ProjectTo<SupplierData>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<SupplierData> CreateAsync(SaveSupplierData data, CancellationToken ct = default)
    {
        var supplier = new Supplier();
        Apply(supplier, data);
        await EnsureNameIsFreeAsync(supplier.Name, null, ct);

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(ct);
        return (await GetByIdAsync(supplier.Id, ct))!;
    }

    public async Task<SupplierData?> ModifyAsync(int id, SaveSupplierData data, CancellationToken ct = default)
    {
        var supplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.Id == id, ct);
        if (supplier is null)
            return null;

        Apply(supplier, data);
        await EnsureNameIsFreeAsync(supplier.Name, id, ct);

        await _context.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == id)
            .Select(s => new { s.Name, OrderCount = s.Orders.Count() })
            .SingleOrDefaultAsync(ct);
        if (supplier is null)
            return false;

        if (supplier.OrderCount > 0)
            throw new ConflictException(nameof(id),
                $"{supplier.Name} nem törölhető, mert {supplier.OrderCount} beszerzési rendelése van.");

        await _context.Suppliers.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
        return true;
    }

    // Felesleges szóközök nélkül; az üres e-mail / telefon null
    private static void Apply(Supplier supplier, SaveSupplierData data)
    {
        supplier.Name = data.Name.Trim();
        supplier.Email = string.IsNullOrWhiteSpace(data.Email) ? null : data.Email.Trim();
        supplier.Phone = string.IsNullOrWhiteSpace(data.Phone) ? null : data.Phone.Trim();
    }

    // Két azonos nevű beszállító összekeverhető lenne a rendelés űrlapon
    private async Task EnsureNameIsFreeAsync(string name, int? ownId, CancellationToken ct)
    {
        if (await _context.Suppliers.AnyAsync(s => s.Name == name && s.Id != ownId, ct))
            throw new BusinessValidationException(nameof(SaveSupplierData.Name), $"Már létezik beszállító ezzel a névvel: {name}");
    }
}
