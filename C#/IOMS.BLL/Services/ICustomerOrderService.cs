using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderDto>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CustomerOrderDto> CreateAsync(CreateCustomerOrderDto dto, CancellationToken ct = default);
}
