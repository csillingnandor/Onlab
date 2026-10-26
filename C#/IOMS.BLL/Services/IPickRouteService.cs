using IOMS.BLL.Warehousing;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IPickRouteService
{
    // Függő rendelés felvételi körútja; null, ha nincs ilyen rendelés. ConflictException, ha a rendelés nem függő,
    // BusinessValidationException, ha a kiszolgáló raktárnak nincs elrendezése.
    Task<PickRouteData?> GetForOrderAsync(int orderId, CancellationToken ct = default);

    // Melyik helyről mennyit (termék -> mennyiség); null, ha a raktárnak nincs elrendezése.
    // A kiszállítás ez alapján csökkenti a helyenkénti készletet.
    Task<PickPlan?> PlanPicksAsync(int warehouseId, IReadOnlyDictionary<int, int> quantityByProduct, CancellationToken ct = default);
}
