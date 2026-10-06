using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ISupplierOrderRepository
{
    // Legújabb rendelés elöl
    Task<IReadOnlyList<SupplierOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierOrderData?> GetByIdAsync(int id, CancellationToken ct = default);

    // Új, Pending státuszú beszerzési rendelés; az azonos termékű sorokat összevonja (súlyozott átlagárral).
    // A beszállító és a termékek létezését a hívónak kell előtte ellenőriznie.
    Task<SupplierOrderData> CreateAsync(CreateSupplierOrderData data, CancellationToken ct = default);
}
