using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class SupplierOrderRepository : ISupplierOrderRepository
{
    private readonly AppDbContext _context;

    public SupplierOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.SupplierOrders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Select(DataProjections.SupplierOrder)
            .ToListAsync(ct);
    }

    public async Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.SupplierOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(DataProjections.SupplierOrder)
            .SingleOrDefaultAsync(ct);
    }
}
