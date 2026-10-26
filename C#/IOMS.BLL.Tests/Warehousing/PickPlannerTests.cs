using IOMS.BLL.Warehousing;

namespace IOMS.BLL.Tests.Warehousing;

public class PickPlannerTests
{
    // Üres, 10 × 10-es csarnok, a depó a (0, 0) cellában: a távolság a Manhattan-távolság
    private static readonly PathTree FromDepot = new WarehouseGrid(10, 10, []).ShortestPaths(new(0, 0));

    private static Dictionary<int, int> Order(params (int ProductId, int Quantity)[] lines) =>
        lines.ToDictionary(l => l.ProductId, l => l.Quantity);

    [Fact]
    public void TakesFromNearestLocation()
    {
        var plan = PickPlanner.Plan(
            Order((1, 5)),
            [new(10, 1, 20, new(8, 8)), new(11, 1, 20, new(2, 1))],
            new Dictionary<int, int>(),
            FromDepot);

        Assert.Equal([new PlannedPick(11, 1, 5)], plan.Picks);
        Assert.Empty(plan.Shortages);
    }

    [Fact]
    public void SplitsAcrossLocationsWhenOneIsNotEnough()
    {
        var plan = PickPlanner.Plan(
            Order((1, 12)),
            [new(10, 1, 4, new(1, 0)), new(11, 1, 20, new(3, 0)), new(12, 1, 20, new(9, 9))],
            new Dictionary<int, int>(),
            FromDepot);

        Assert.Equal([new PlannedPick(10, 1, 4), new PlannedPick(11, 1, 8)], plan.Picks);
    }

    [Fact]
    public void UsesDockOnlyAfterLocationsAndReportsShortage()
    {
        var plan = PickPlanner.Plan(
            Order((1, 10)),
            [new(10, 1, 3, new(5, 5))],
            new Dictionary<int, int> { [1] = 4 },
            FromDepot);

        Assert.Equal([new PlannedPick(10, 1, 3), new PlannedPick(null, 1, 4)], plan.Picks);
        Assert.Equal(3, plan.Shortages[1]);
    }

    [Fact]
    public void PrefersStopAlreadyOnTheRoute()
    {
        // A 2-es termék csak a (6, 6) megállónál van, ezért az 1-es is onnan jön a közelebbi (1, 1) helyett
        var plan = PickPlanner.Plan(
            Order((1, 2), (2, 2)),
            [new(10, 1, 5, new(1, 1)), new(11, 1, 5, new(6, 6)), new(12, 2, 5, new(6, 6))],
            new Dictionary<int, int>(),
            FromDepot);

        Assert.Contains(new PlannedPick(11, 1, 2), plan.Picks);
        Assert.DoesNotContain(plan.Picks, p => p.LocationId == 10);
    }

    [Fact]
    public void IgnoresUnreachableLocations()
    {
        var plan = PickPlanner.Plan(
            Order((1, 2)),
            [new(10, 1, 5, null), new(11, 1, 5, new(4, 4))],
            new Dictionary<int, int>(),
            FromDepot);

        Assert.Equal([new PlannedPick(11, 1, 2)], plan.Picks);
    }
}
