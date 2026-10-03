using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ICustomerOrderItemRepository
{
    Task<IReadOnlyList<CustomerOrderItemData>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CustomerOrderItemData>> GetByOrderAsync(int orderId, CancellationToken ct = default);
}
