using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default)
    {
        return _context.Products.AnyAsync(p => p.SKU == sku, ct);
    }

    public Task<Dictionary<int, decimal>> GetPricesAsync(IEnumerable<int> productIds, CancellationToken ct = default)
    {
        var ids = productIds.ToList();
        return _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);
    }
}
