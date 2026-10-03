using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
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

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _context.Warehouses.AnyAsync(w => w.Id == id, ct);
    }
}
