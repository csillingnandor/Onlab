using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _suppliers;

    public SupplierService(ISupplierRepository suppliers)
    {
        _suppliers = suppliers;
    }

    public Task<IReadOnlyList<SupplierData>> GetAllAsync(CancellationToken ct = default)
    {
        return _suppliers.GetAllAsync(ct);
    }
}
