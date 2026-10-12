using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.DAL;
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
}
