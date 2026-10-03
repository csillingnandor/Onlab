using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default);
    Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default);

    // A megadott azonosítók közül azok, amelyekhez nincs termék.
    Task<IReadOnlyList<int>> GetMissingIdsAsync(IEnumerable<int> productIds, CancellationToken ct = default);

    // Az SKU egyediségét a hívónak kell előtte ellenőriznie.
    Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default);
}
