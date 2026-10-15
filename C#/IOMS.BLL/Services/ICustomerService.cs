using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CustomerData> CreateAsync(CreateCustomerData data, CancellationToken ct = default);

    // A módosítás ugyanazokat a mezőket kapja, mint a felvétel; null, ha nincs ilyen vevő
    Task<CustomerData?> ModifyAsync(int id, CreateCustomerData data, CancellationToken ct = default);

    // false, ha nincs ilyen vevő; ConflictException, ha vannak rendelései
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
