using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class SupplierOrderService : ISupplierOrderService
{
    private readonly ISupplierOrderRepository _supplierOrders;

    public SupplierOrderService(ISupplierOrderRepository supplierOrders)
    {
        _supplierOrders = supplierOrders;
    }

    public Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return _supplierOrders.GetAllAsync(ct);
    }

    public Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _supplierOrders.GetByIdAsync(id, ct);
    }
}
