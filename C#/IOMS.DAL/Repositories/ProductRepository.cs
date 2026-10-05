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
        var since = DateTime.UtcNow.AddDays(-7);
        var productData = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(DataProjections.Product(since))
            .ToListAsync(ct);
        foreach (var p in productData)
        {
            ApplyStockInfo(p);
        }
        return productData;
    }

    private void ApplyStockInfo(ProductData productData)
    {
        SetDaysOfCover(productData);
        SetStatus(productData);
    }
    private void SetDaysOfCover(ProductData productData)
    {
        if (productData.StockQuantity > 0 && productData.OrderedLast7Days > 0)
        {
            productData.DaysOfCover = Math.Round(productData.StockQuantity * 7m / productData.OrderedLast7Days, 1);
        }
        else
        {
            productData.DaysOfCover = null;
        }
    }
    private void SetStatus(ProductData productData)
    {
        if (productData.StockQuantity == 0)
        {
            productData.Status = "Out of Stock";
        }
        else if (productData.StockQuantity < productData.OrderedLast7Days || productData.StockQuantity <= productData.MinStockLevel)
        {
            productData.Status = "Low Stock";
        }
        else
        {
            productData.Status = "In Stock";
        }

    }

    public async Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var product =  await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(DataProjections.Product(since))
            .SingleOrDefaultAsync(ct);
        if (product is not null)
        {
            ApplyStockInfo(product);
        }
        return product;
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
