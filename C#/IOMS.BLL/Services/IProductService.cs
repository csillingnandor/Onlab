using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default);
    Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default);
}
