using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ISupplierOrderRepository
{
    // Legújabb rendelés elöl
    Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default);
}
