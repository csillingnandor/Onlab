using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SupplierData> CreateAsync(SaveSupplierData data, CancellationToken ct = default);

    // null, ha nincs ilyen beszállító
    Task<SupplierData?> ModifyAsync(int id, SaveSupplierData data, CancellationToken ct = default);

    // false, ha nincs ilyen beszállító; ConflictException, ha vannak rendelései
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
