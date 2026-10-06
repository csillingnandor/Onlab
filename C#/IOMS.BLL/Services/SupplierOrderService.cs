using FluentValidation;
using IOMS.BLL.Exceptions;
using IOMS.BLL.Validation;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class SupplierOrderService : ISupplierOrderService
{
    private readonly ISupplierOrderRepository _supplierOrders;
    private readonly ISupplierRepository _suppliers;
    private readonly IProductRepository _products;
    private readonly IValidator<CreateSupplierOrderData> _createValidator;

    public SupplierOrderService(
        ISupplierOrderRepository supplierOrders,
        ISupplierRepository suppliers,
        IProductRepository products,
        IValidator<CreateSupplierOrderData> createValidator)
    {
        _supplierOrders = supplierOrders;
        _suppliers = suppliers;
        _products = products;
        _createValidator = createValidator;
    }

    public Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default)
    {
        return _supplierOrders.GetAllAsync(ct);
    }

    public Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _supplierOrders.GetByIdAsync(id, ct);
    }

    public async Task<SupplierOrderData> CreateAsync(CreateSupplierOrderData data, CancellationToken ct = default)
    {
        // Előbb a formai szabályok, hogy hibás kérés ne menjen az adatbázisig
        await _createValidator.EnsureValidAsync(data, ct);

        if (!await _suppliers.ExistsAsync(data.SupplierId, ct))
            throw new BusinessValidationException(nameof(data.SupplierId), $"Nincs {data.SupplierId} azonosítójú beszállító.");

        var missing = await _products.GetMissingIdsAsync(data.Items.Select(i => i.ProductId), ct);
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(data.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

        return await _supplierOrders.CreateAsync(data, ct);
    }
}
