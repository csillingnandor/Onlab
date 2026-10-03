using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL.Entities;
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

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _products.GetAllAsync(ct);
        return products.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(id, ct);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        // Az SKU egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _products.SkuExistsAsync(dto.SKU, ct))
            throw new BusinessValidationException(nameof(dto.SKU), $"Már létezik termék ezzel az SKU-val: {dto.SKU}");

        var product = new Product
        {
            Name = dto.Name,
            SKU = dto.SKU,
            Category = dto.Category ?? string.Empty,
            StockQuantity = dto.StockQuantity,
            MinStockLevel = dto.MinStockLevel,
            Price = dto.Price,
        };

        await _products.AddAsync(product, ct);

        return product.ToDto();
    }
}
