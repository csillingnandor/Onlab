using IOMS.DAL.Entities;
using IOMS.DAL.Mapping;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(DataProjections.Product)
            .ToListAsync(ct);
    }

    public async Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(DataProjections.Product)
            .SingleOrDefaultAsync(ct);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default)
    {
        return _context.Products.AnyAsync(p => p.SKU == sku, ct);
    }

    public async Task<IReadOnlyList<int>> GetMissingIdsAsync(IEnumerable<int> productIds, CancellationToken ct = default)
    {
        var ids = productIds.Distinct().ToList();
        var existing = await _context.Products
            .Where(p => ids.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        return ids.Except(existing).ToList();
    }

    public async Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default)
    {
        var product = new Product
        {
            Name = data.Name,
            SKU = data.SKU,
            Category = data.Category ?? string.Empty,
            StockQuantity = data.StockQuantity,
            MinStockLevel = data.MinStockLevel,
            Price = data.Price,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        return (await GetByIdAsync(product.Id, ct))!;
    }
}
