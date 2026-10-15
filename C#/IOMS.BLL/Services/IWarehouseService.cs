using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IWarehouseService
{
    Task<IReadOnlyList<WarehouseData>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WarehouseData> CreateAsync(SaveWarehouseData data, CancellationToken ct = default);

    // null, ha nincs ilyen raktár
    Task<WarehouseData?> ModifyAsync(int id, SaveWarehouseData data, CancellationToken ct = default);

    // false, ha nincs ilyen raktár; ConflictException, ha készlet, rendelés vagy készletmozgás hivatkozik rá
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
