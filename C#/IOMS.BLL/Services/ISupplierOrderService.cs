using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ISupplierOrderService
{
    Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default);
}
