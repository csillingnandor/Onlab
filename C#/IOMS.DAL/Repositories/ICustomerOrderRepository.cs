using IOMS.DAL.Entities;

namespace IOMS.DAL.Repositories;

public interface ICustomerOrderRepository
{
    // A visszaadott rendelésekben a Customer és az Items (Product-tal együtt) be van töltve.
    Task<IReadOnlyList<CustomerOrder>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrder?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(CustomerOrder order, CancellationToken ct = default);
}
