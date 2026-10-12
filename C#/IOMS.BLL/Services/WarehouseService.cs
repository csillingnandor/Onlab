using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

public class WarehouseService : IWarehouseService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public WarehouseService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .ProjectTo<WarehouseData>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(w => w.Id == id)
            .ProjectTo<WarehouseData>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(ct);
    }
}
