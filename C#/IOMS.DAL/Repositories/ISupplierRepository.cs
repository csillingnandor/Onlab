using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ISupplierRepository
{
    // Név szerint rendezve
    Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
