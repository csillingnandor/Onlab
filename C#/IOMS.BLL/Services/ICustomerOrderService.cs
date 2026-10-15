using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default);

    // Függő (Pending) rendelés vevőjének, raktárának és tételeinek cseréje; null, ha nincs ilyen rendelés,
    // ConflictException, ha már nem függő. A megmaradó termékek egységára változatlan, az újak az aktuális árat kapják.
    Task<CustomerOrderData?> ModifyAsync(int id, CreateCustomerOrderData data, CancellationToken ct = default);

    // Státuszváltások; mindegyik null-t ad, ha nincs ilyen rendelés, és ConflictException-t,
    // ha a rendelés nem a megfelelő állapotban van.

    // Pending -> Shipped; a kiszolgáló raktár készlete csökken (ConflictException, ha nincs elég)
    Task<CustomerOrderData?> ShipAsync(int id, CancellationToken ct = default);

    // Shipped -> Delivered; csak státusz
    Task<CustomerOrderData?> DeliverAsync(int id, CancellationToken ct = default);

    // Pending -> Cancelled; csak státusz (kiszállított rendelés nem mondható le)
    Task<CustomerOrderData?> CancelAsync(int id, CancellationToken ct = default);
}
