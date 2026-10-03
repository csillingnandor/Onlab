using IOMS.DAL.Entities;

namespace IOMS.DAL.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default);

    // Termék azonosító -> aktuális ár; a nem létező azonosítók kimaradnak a szótárból.
    Task<Dictionary<int, decimal>> GetPricesAsync(IEnumerable<int> productIds, CancellationToken ct = default);

    Task AddAsync(Product product, CancellationToken ct = default);
}
