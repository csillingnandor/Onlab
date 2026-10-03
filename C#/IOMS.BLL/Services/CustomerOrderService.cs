using IOMS.BLL.Exceptions;
using IOMS.BLL.Mapping;
using IOMS.DAL.Entities;
using IOMS.DAL.Repositories;
using IOMS.DTO;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly ICustomerOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly IProductRepository _products;

    public CustomerOrderService(
        ICustomerOrderRepository orders,
        ICustomerRepository customers,
        IProductRepository products)
    {
        _orders = orders;
        _customers = customers;
        _products = products;
    }

    public async Task<IReadOnlyList<CustomerOrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orders.GetAllAsync(ct);
        return orders.Select(o => o.ToDto()).ToList();
    }

    public async Task<CustomerOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        return order?.ToDto();
    }

    public async Task<CustomerOrderDto> CreateAsync(CreateCustomerOrderDto dto, CancellationToken ct = default)
    {
        if (!await _customers.ExistsAsync(dto.CustomerId, ct))
            throw new BusinessValidationException(nameof(dto.CustomerId), $"Nincs {dto.CustomerId} azonosítójú vevő.");

        // Ugyanaz a termék többször is szerepelhet a kérésben, ezeket összevonjuk egy tétellé.
        var lines = dto.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToList();

        var productIds = lines.Select(l => l.ProductId).ToList();
        var prices = await _products.GetPricesAsync(productIds, ct);

        var missing = productIds.Where(id => !prices.ContainsKey(id)).ToList();
        if (missing.Count > 0)
            throw new BusinessValidationException(nameof(dto.Items), $"Ismeretlen termék azonosító(k): {string.Join(", ", missing)}");

        var order = new CustomerOrder
        {
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = EntityOrderStatus.Pending,
            Items = lines
                .Select(l => new CustomerOrderItem
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    // Az aktuális árat rögzítjük, hogy későbbi árváltozás ne írja át a rendelést.
                    UnitPrice = prices[l.ProductId],
                })
                .ToList(),
        };

        await _orders.AddAsync(order, ct);

        // Újratöltés, hogy a vevő- és terméknevek is benne legyenek a válaszban.
        return (await GetByIdAsync(order.Id, ct))!;
    }
}
