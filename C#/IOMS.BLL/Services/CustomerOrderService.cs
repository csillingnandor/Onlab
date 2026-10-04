using FluentValidation;
using IOMS.BLL.Exceptions;
using IOMS.BLL.Validation;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly ICustomerOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly IProductRepository _products;
    private readonly IValidator<CreateCustomerOrderData> _createValidator;

    public CustomerOrderService(
        ICustomerOrderRepository orders,
        ICustomerRepository customers,
        IProductRepository products,
        IValidator<CreateCustomerOrderData> createValidator)
    {
        _orders = orders;
        _customers = customers;
        _products = products;
        _createValidator = createValidator;
    }

    public Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return _orders.GetAllAsync(ct);
    }

    public Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _orders.GetByIdAsync(id, ct);
    }

    public async Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default)
    {
        // Előbb a formai szabályok, hogy hibás kérés ne menjen az adatbázisig
        await _createValidator.EnsureValidAsync(data, ct);

        if (!await _customers.ExistsAsync(data.CustomerId, ct))
            throw new BusinessValidationException(nameof(data.CustomerId), $"Nincs {data.CustomerId} azonosítójú vevő.");

        var missing = await _products.GetMissingIdsAsync(data.Items.Select(i => i.ProductId), ct);
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(data.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

        return await _orders.CreateAsync(data, ct);
    }
}
