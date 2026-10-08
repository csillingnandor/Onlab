using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using OrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

public class ProductService : IProductService
{
    private const int DefaultStatisticsDays = 30;
    private const int MaxStatisticsDays = 366;

    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(DataProjections.Product(since))
            .ToListAsync(ct);

        foreach (var product in products)
            ApplyStockInfo(product);

        return products;
    }

    public async Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(DataProjections.Product(since))
            .SingleOrDefaultAsync(ct);

        if (product is not null)
            ApplyStockInfo(product);

        return product;
    }

    public async Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default)
    {
        // Az SKU egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _context.Products.AnyAsync(p => p.SKU == data.SKU, ct))
            throw new BusinessValidationException(nameof(data.SKU), $"Már létezik termék ezzel az SKU-val: {data.SKU}");

        var product = new Product
        {
            Name = data.Name,
            SKU = data.SKU,
            Category = data.Category ?? string.Empty,
            MinStockLevel = data.MinStockLevel,
            Price = data.Price,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        return (await GetByIdAsync(product.Id, ct))!;
    }

    public async Task<ProductSaleStatisticsData?> GetSaleStatisticsAsync(
        int id, DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var start = from ?? end.AddDays(-(DefaultStatisticsDays - 1));

        if (start > end)
            throw new BusinessValidationException(nameof(from), "A kezdő dátum nem lehet későbbi a záró dátumnál.");

        // Napi bontás van a válaszban, ezért korlátozzuk a hosszát.
        if (end.DayNumber - start.DayNumber + 1 > MaxStatisticsDays)
            throw new BusinessValidationException(nameof(to), $"Az időszak legfeljebb {MaxStatisticsDays} nap lehet.");

        var productName = await _context.Products
            .Where(p => p.Id == id)
            .Select(p => p.Name)
            .SingleOrDefaultAsync(ct);

        if (productName is null)
            return null;

        // A zárt [start, end] napokból félig nyitott [start, end+1) időintervallum, hogy a záró nap egésze benne legyen.
        var sales = await GetDailySalesAsync(
            id,
            start.ToDateTime(TimeOnly.MinValue),
            end.AddDays(1).ToDateTime(TimeOnly.MinValue),
            ct);

        var salesByDay = sales.ToDictionary(s => s.Date);

        // Az eladás nélküli napok is szerepeljenek (0 értékkel), hogy a diagram folytonos legyen.
        var daily = new List<DailyProductSaleData>();
        for (var day = start; day <= end; day = day.AddDays(1))
        {
            salesByDay.TryGetValue(day, out var s);
            daily.Add(new DailyProductSaleData
            {
                Date = day,
                Quantity = s?.Quantity ?? 0,
                Revenue = s?.Revenue ?? 0,
                OrderCount = s?.OrderCount ?? 0,
            });
        }

        var totalQuantity = daily.Sum(d => d.Quantity);
        var totalRevenue = daily.Sum(d => d.Revenue);

        return new ProductSaleStatisticsData
        {
            ProductId = id,
            ProductName = productName,
            From = start,
            To = end,
            TotalQuantity = totalQuantity,
            TotalRevenue = totalRevenue,
            AverageUnitPrice = totalQuantity > 0 ? totalRevenue / totalQuantity : null,
            Daily = daily,
        };
    }

    // Csak a kiszállított (Delivered) rendelések számítanak eladásnak; az intervallum [from, toExclusive).
    private async Task<IReadOnlyList<DailyProductSaleData>> GetDailySalesAsync(
        int productId, DateTime from, DateTime toExclusive, CancellationToken ct = default)
    {
        return await _context.CustomerOrderItems
            .AsNoTracking()
            .Where(i => i.ProductId == productId
                     && i.CustomerOrder.Status == OrderStatus.Delivered
                     && i.CustomerOrder.OrderDate >= from
                     && i.CustomerOrder.OrderDate < toExclusive)
            .GroupBy(i => i.CustomerOrder.OrderDate.Date)
            .Select(g => new DailyProductSaleData
            {
                Date = DateOnly.FromDateTime(g.Key),
                Quantity = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice),
                // Egy rendelésben egy termék csak egy tételként szerepel, így tétel = rendelés.
                OrderCount = g.Count(),
            })
            .OrderBy(r => r.Date)
            .ToListAsync(ct);
    }

    private static void ApplyStockInfo(ProductData product)
    {
        // Hány napig elég a készlet az utolsó 7 nap kereslete alapján
        product.DaysOfCover = product.StockQuantity > 0 && product.OrderedLast7Days > 0
            ? Math.Round(product.StockQuantity * 7m / product.OrderedLast7Days, 1)
            : null;

        if (product.StockQuantity == 0)
            product.Status = "Out of Stock";
        else if (product.StockQuantity < product.OrderedLast7Days || product.StockQuantity <= product.MinStockLevel)
            product.Status = "Low Stock";
        else
            product.Status = "In Stock";
    }

    public async Task<ProductData?> ModifyProductAsync(ModifyProductData data, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Where(p => p.Id == data.Id)
            .SingleOrDefaultAsync(ct);
        if (product is null)
            return null;
        // Az SKU egyedi index; a saját SKU-ja megtartható, más termékét nem veheti át.
        if (await _context.Products.AnyAsync(p => p.SKU == data.SKU && p.Id != data.Id, ct))
            throw new BusinessValidationException(nameof(data.SKU), $"Már létezik termék ezzel az SKU-val: {data.SKU}");
        product.Name = data.Name;
        product.SKU = data.SKU;
        product.Category = data.Category ?? string.Empty;
        product.MinStockLevel = data.MinStockLevel;
        product.Price = data.Price;
        await _context.SaveChangesAsync(ct);

        // Újratöltés, hogy a Status és a DaysOfCover az új minimális készlettel legyen kiszámolva.
        return await GetByIdAsync(product.Id, ct);
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Where(p => p.Id == id)
            .SingleOrDefaultAsync(ct);
        if (product is null)
            return false;
        // A rendeléstételek felé Restrict a kapcsolat, ezért előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        var hasOrders = await _context.CustomerOrderItems.AnyAsync(i => i.ProductId == id, ct)
                     || await _context.SupplierOrderItems.AnyAsync(i => i.ProductId == id, ct);
        if (hasOrders)
            throw new ConflictException(nameof(id), $"A termékhez már tartozik rendelés, ezért nem törölhető: {id}");
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
