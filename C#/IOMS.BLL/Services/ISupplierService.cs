using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default);
}
