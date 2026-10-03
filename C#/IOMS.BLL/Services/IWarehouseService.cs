using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IWarehouseService
{
    Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default);
}
