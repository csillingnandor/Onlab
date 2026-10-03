using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerOrderItemService
{
    Task<IReadOnlyList<CustomerOrderItemData>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CustomerOrderItemData>> GetByOrderAsync(int orderId, CancellationToken ct = default);
}
