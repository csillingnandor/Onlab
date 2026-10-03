using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface IWarehouseRepository
{
    Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
