using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default);
}
