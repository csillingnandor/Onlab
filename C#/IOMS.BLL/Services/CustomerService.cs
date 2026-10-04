using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;

    public CustomerService(ICustomerRepository customers)
    {
        _customers = customers;
    }

    public Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default)
    {
        return _customers.GetAllAsync(ct);
    }

    public Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _customers.GetByIdAsync(id, ct);
    }
}
