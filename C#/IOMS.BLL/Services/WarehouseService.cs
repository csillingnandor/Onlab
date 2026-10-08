using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class WarehouseService : IWarehouseService
{
    private readonly AppDbContext _context;

    public WarehouseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .Select(DataProjections.Warehouse)
            .ToListAsync(ct);
    }

    public async Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(w => w.Id == id)
            .Select(DataProjections.Warehouse)
            .SingleOrDefaultAsync(ct);
    }
}
