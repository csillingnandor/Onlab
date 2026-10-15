using AutoMapper;
using AutoMapper.QueryableExtensions;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
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

    public async Task<WarehouseData> CreateAsync(SaveWarehouseData data, CancellationToken ct = default)
    {
        var warehouse = new Warehouse();
        Apply(warehouse, data);
        await EnsureNameIsFreeAsync(warehouse.Name, null, ct);

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync(ct);
        return (await GetByIdAsync(warehouse.Id, ct))!;
    }

    public async Task<WarehouseData?> ModifyAsync(int id, SaveWarehouseData data, CancellationToken ct = default)
    {
        var warehouse = await _context.Warehouses.SingleOrDefaultAsync(w => w.Id == id, ct);
        if (warehouse is null)
            return null;

        Apply(warehouse, data);
        await EnsureNameIsFreeAsync(warehouse.Name, id, ct);

        await _context.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    // A készlet, a rendelések és a készletnapló nem veszhet el, ezért csak érintetlen raktár törölhető
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var warehouse = await _context.Warehouses
            .Where(w => w.Id == id)
            .Select(w => new { w.Name, HasStock = w.Inventories.Any(i => i.Quantity > 0) })
            .SingleOrDefaultAsync(ct);
        if (warehouse is null)
            return false;

        if (warehouse.HasStock)
            throw new ConflictException(nameof(id), $"{warehouse.Name} nem törölhető, mert készlet van benne.");

        var referenced = await _context.CustomerOrders.AnyAsync(o => o.WarehouseId == id, ct)
            || await _context.SupplierOrders.AnyAsync(o => o.WarehouseId == id, ct)
            || await _context.StockMovements.AnyAsync(m => m.WarehouseId == id, ct);
        if (referenced)
            throw new ConflictException(nameof(id),
                $"{warehouse.Name} nem törölhető, mert rendelés vagy készletmozgás hivatkozik rá.");

        // Az esetleges 0 mennyiségű készletsorok sem akadályozhatják a törlést
        await _context.Inventories.Where(i => i.WarehouseId == id).ExecuteDeleteAsync(ct);
        await _context.Warehouses.Where(w => w.Id == id).ExecuteDeleteAsync(ct);
        return true;
    }

    private static void Apply(Warehouse warehouse, SaveWarehouseData data)
    {
        warehouse.Name = data.Name.Trim();
        warehouse.Address = data.Address.Trim();
        warehouse.Capacity = data.Capacity;
        warehouse.Latitude = data.Latitude;
        warehouse.Longitude = data.Longitude;
    }

    private async Task EnsureNameIsFreeAsync(string name, int? ownId, CancellationToken ct)
    {
        if (await _context.Warehouses.AnyAsync(w => w.Name == name && w.Id != ownId, ct))
            throw new BusinessValidationException(nameof(SaveWarehouseData.Name), $"Már létezik raktár ezzel a névvel: {name}");
    }
}
