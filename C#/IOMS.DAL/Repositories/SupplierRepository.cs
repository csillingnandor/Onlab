using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(DataProjections.Supplier)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _context.Suppliers.AnyAsync(s => s.Id == id, ct);
    }
}
