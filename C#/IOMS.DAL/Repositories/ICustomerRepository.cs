using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ICustomerRepository
{
    // Név szerint rendezve, a rendelésekből számított mezőkkel
    Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

    // Az e-mail egyediségét a hívónak kell előtte ellenőriznie.
    Task<CustomerData> CreateAsync(CreateCustomerData data, CancellationToken ct = default);
}
