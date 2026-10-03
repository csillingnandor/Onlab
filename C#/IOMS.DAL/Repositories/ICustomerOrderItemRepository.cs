using IOMS.DAL.Entities;

namespace IOMS.DAL.Repositories;

public interface ICustomerOrderItemRepository
{
    // A visszaadott tételekben a Product be van töltve.
    Task<IReadOnlyList<CustomerOrderItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CustomerOrderItem>> GetByOrderAsync(int orderId, CancellationToken ct = default);
}
