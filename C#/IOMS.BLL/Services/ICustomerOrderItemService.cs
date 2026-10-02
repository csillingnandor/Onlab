using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerOrderItemService
{
    Task<IReadOnlyList<CustomerOrderItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CustomerOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default);
}
