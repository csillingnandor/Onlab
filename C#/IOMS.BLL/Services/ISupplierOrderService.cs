using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ISupplierOrderService
{
    Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SupplierOrderData> CreateAsync(CreateSupplierOrderData data, CancellationToken ct = default);

    // Beérkeztetés a célraktárba (készlet + napló); null, ha nincs ilyen rendelés.
    // ConflictException, ha már beérkezett vagy le van mondva.
    Task<SupplierOrderData?> ReceiveAsync(int id, CancellationToken ct = default);
}
