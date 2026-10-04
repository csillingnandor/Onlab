using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default);
}
