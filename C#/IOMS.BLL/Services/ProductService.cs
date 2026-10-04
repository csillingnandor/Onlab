using FluentValidation;
using IOMS.BLL.Exceptions;
using IOMS.BLL.Validation;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class ProductService : IProductService
{
    private const int DefaultStatisticsDays = 30;
    private const int MaxStatisticsDays = 366;

    private readonly IProductRepository _products;
    private readonly ICustomerOrderRepository _orders;
    private readonly IValidator<CreateProductData> _createValidator;

    public ProductService(
        IProductRepository products,
        ICustomerOrderRepository orders,
        IValidator<CreateProductData> createValidator)
    {
        _products = products;
        _orders = orders;
        _createValidator = createValidator;
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
        // Előbb a formai szabályok, hogy hibás kérés ne menjen az adatbázisig
        await _createValidator.EnsureValidAsync(data, ct);

        // Az SKU egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _products.SkuExistsAsync(data.SKU, ct))
            throw new BusinessValidationException(nameof(data.SKU), $"Már létezik termék ezzel az SKU-val: {data.SKU}");

        return await _products.CreateAsync(data, ct);
    }

    public Task<ProductSaleStatisticsData?> GetSaleStatisticsAsync(
        int id, DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var start = from ?? end.AddDays(-(DefaultStatisticsDays - 1));

        if (start > end)
            throw new BusinessValidationException(nameof(from), "A kezdő dátum nem lehet későbbi a záró dátumnál.");

        // Napi bontás van a válaszban, ezért korlátozzuk a hosszát.
        if (end.DayNumber - start.DayNumber + 1 > MaxStatisticsDays)
            throw new BusinessValidationException(nameof(to), $"Az időszak legfeljebb {MaxStatisticsDays} nap lehet.");

        return _orders.GetProductSaleStatisticsAsync(id, start, end, ct);
    }
}
