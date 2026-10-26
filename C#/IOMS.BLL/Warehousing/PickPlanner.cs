namespace IOMS.BLL.Warehousing;

// Egy termék készlete egy tárolóhelyen, a hely elérési (folyosó) cellájával; null, ha a hely nem érhető el
public record LocationCandidate(int LocationId, int ProductId, int Quantity, GridPoint? AccessPoint);

// Egy felvétel: ennyi darab erről a helyről (LocationId = null: a depón lévő, még el nem helyezett készletből)
public record PlannedPick(int? LocationId, int ProductId, int Quantity);

public record PickPlan(IReadOnlyList<PlannedPick> Picks, IReadOnlyDictionary<int, int> Shortages);

// Melyik tárolóhelyről mennyit vegyünk fel egy rendeléshez. Stratégia:
// - elsősorban a polcokról: a depón lévő áru még átvétel / betárolás alatt áll, csak tartalék;
// - termékenként a már kiválasztott megállókon lévő helyek előnyben (nem kell érte külön kitérő),
//   azon belül a depóhoz legközelebbiek, amíg a mennyiség össze nem jön;
// - ami a polcokról nem jön össze, a depóról; ami onnan sem, az hiány.
// A kevés helyen elérhető termékek kerülnek sorra először, hogy a megállóikhoz igazodhassanak a többiek.
public static class PickPlanner
{
    public static PickPlan Plan(
        IReadOnlyDictionary<int, int> quantityByProduct,
        IReadOnlyList<LocationCandidate> candidates,
        IReadOnlyDictionary<int, int> dockQuantityByProduct,
        PathTree fromDepot)
    {
        var picks = new List<PlannedPick>();
        var shortages = new Dictionary<int, int>();
        var chosenStops = new HashSet<GridPoint>();

        var reachable = candidates
            .Where(c => c.Quantity > 0 && c.AccessPoint is { } p && fromDepot.DistanceTo(p) is not null)
            .GroupBy(c => c.ProductId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var products = quantityByProduct
            .Where(kv => kv.Value > 0)
            .OrderBy(kv => reachable.TryGetValue(kv.Key, out var list) ? list.Count : 0)
            .ThenBy(kv => kv.Key);

        foreach (var (productId, quantity) in products)
        {
            var remaining = quantity;
            var options = reachable.GetValueOrDefault(productId, [])
                .OrderBy(c => chosenStops.Contains(c.AccessPoint!.Value) ? 0 : 1)
                .ThenBy(c => fromDepot.DistanceTo(c.AccessPoint!.Value))
                .ThenBy(c => c.LocationId);

            foreach (var option in options)
            {
                if (remaining == 0)
                    break;
                var take = Math.Min(remaining, option.Quantity);
                picks.Add(new PlannedPick(option.LocationId, productId, take));
                chosenStops.Add(option.AccessPoint!.Value);
                remaining -= take;
            }

            var fromDock = Math.Min(remaining, Math.Max(0, dockQuantityByProduct.GetValueOrDefault(productId)));
            if (fromDock > 0)
            {
                picks.Add(new PlannedPick(null, productId, fromDock));
                remaining -= fromDock;
            }

            if (remaining > 0)
                shortages[productId] = remaining;
        }

        return new PickPlan(picks, shortages);
    }
}
