namespace IOMS.DAL.Repositories;

public interface ICustomerRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
