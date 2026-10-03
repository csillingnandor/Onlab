using IOMS.BLL.Exceptions;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default)
    {
        return _products.GetAllAsync(ct);
    }

    public Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _products.GetByIdAsync(id, ct);
    }

    public async Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default)
    {
        // Az SKU egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _products.SkuExistsAsync(data.SKU, ct))
            throw new BusinessValidationException(nameof(data.SKU), $"Már létezik termék ezzel az SKU-val: {data.SKU}");

        return await _products.CreateAsync(data, ct);
    }
}
