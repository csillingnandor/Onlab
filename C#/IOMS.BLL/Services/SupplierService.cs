using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    // Név szerint rendezve
    public async Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(DataProjections.Supplier)
            .ToListAsync(ct);
    }
}
