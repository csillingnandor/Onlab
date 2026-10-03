using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouses;

    public WarehouseService(IWarehouseRepository warehouses)
    {
        _warehouses = warehouses;
    }

    public Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default)
    {
        return _warehouses.GetAllAsync(ct);
    }

    public Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _warehouses.GetByIdAsync(id, ct);
    }
}
