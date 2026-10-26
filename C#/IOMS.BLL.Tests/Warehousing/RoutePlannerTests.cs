using IOMS.BLL.Warehousing;

namespace IOMS.BLL.Tests.Warehousing;

public class RoutePlannerTests
{
    [Fact]
    public void NoStopsMeansStayingAtDepot()
    {
        var grid = new WarehouseGrid(4, 4, []);

        var plan = RoutePlanner.Plan(grid, new(0, 0), []);

        Assert.Equal(0, plan.Distance);
        Assert.Equal([new GridPoint(0, 0)], plan.Path);
        Assert.Empty(plan.Order);
    }

    [Fact]
    public void SingleStopIsThereAndBack()
    {
        var grid = new WarehouseGrid(6, 6, []);

        var plan = RoutePlanner.Plan(grid, new(0, 0), [new(3, 2)]);

        Assert.Equal(10, plan.Distance);
        Assert.Equal([0], plan.Order);
    }

    [Fact]
    public void StopsInACorridorAreVisitedInOneSweep()
    {
        var grid = new WarehouseGrid(7, 1, []);

        var plan = RoutePlanner.Plan(grid, new(0, 0), [new(3, 0), new(1, 0), new(5, 0)]);

        Assert.Equal([1, 0, 2], plan.Order);
        Assert.Equal(10, plan.Distance);
    }

    [Fact]
    public void PathStartsAndEndsAtDepotAndMatchesDistance()
    {
        var (grid, depot) = SampleWarehouse();
        GridPoint[] stops = [new(2, 1), new(9, 4), new(4, 4), new(10, 1), new(6, 7)];

        var plan = RoutePlanner.Plan(grid, depot, stops);

        Assert.Equal(depot, plan.Path[0]);
        Assert.Equal(depot, plan.Path[^1]);
        Assert.Equal(plan.Distance, plan.Path.Count - 1);
        WarehouseGridTests.AssertContiguousAndWalkable(grid, plan.Path);
        Assert.Equal(Enumerable.Range(0, stops.Length), plan.Order.Order());
        Assert.All(stops, s => Assert.Contains(s, plan.Path));
    }

    [Fact]
    public void DuplicateStopsAreVisitedOnce()
    {
        var grid = new WarehouseGrid(6, 1, []);

        var plan = RoutePlanner.Plan(grid, new(0, 0), [new(4, 0), new(2, 0), new(4, 0)]);

        Assert.Equal(8, plan.Distance);
        Assert.Equal(3, plan.Order.Count);
        // Az azonos cellájú megállók egymás után következnek
        Assert.Equal([1, 0, 2], plan.Order);
    }

    [Fact]
    public void UnreachableStopsAreReportedAndSkipped()
    {
        var grid = new WarehouseGrid(3, 2, [new RackShape(1, 0, 1, true), new RackShape(1, 1, 2, true)]);

        var plan = RoutePlanner.Plan(grid, new(0, 0), [new(0, 1), new(2, 0)]);

        Assert.Equal([1], plan.Unreachable);
        Assert.Equal([0], plan.Order);
        Assert.Equal(2, plan.Distance);
    }

    // Kis, véletlen példákon a heurisztika (legközelebbi szomszéd + 2-opt) eredménye az összes sorrendet
    // kipróbáló optimum közelében van, és sosem rosszabb a megállók megadott sorrendjénél.
    [Fact]
    public void HeuristicIsCloseToOptimumOnSmallInstances()
    {
        var (grid, depot) = SampleWarehouse();
        var walkable = Enumerable.Range(0, grid.Width)
            .SelectMany(x => Enumerable.Range(0, grid.Depth).Select(y => new GridPoint(x, y)))
            .Where(grid.IsWalkable)
            .ToList();
        var random = new Random(42);

        for (var round = 0; round < 25; round++)
        {
            var stops = walkable.OrderBy(_ => random.Next()).Take(6).ToList();

            var plan = RoutePlanner.Plan(grid, depot, stops);
            var optimum = BruteForceOptimum(grid, depot, stops);
            var inGivenOrder = TourLength(grid, depot, stops);

            Assert.True(plan.Distance >= optimum);
            Assert.True(plan.Distance <= optimum * 1.15, $"{round}. kör: {plan.Distance} m, optimum {optimum} m");
            Assert.True(plan.Distance <= inGivenOrder);
        }
    }

    // 12 × 9-es raktár két háttal álló polcpárral, a depó lent középen
    private static (WarehouseGrid Grid, GridPoint Depot) SampleWarehouse()
    {
        RackShape[] racks =
        [
            new(2, 2, 8, true), new(2, 3, 8, true),
            new(2, 5, 8, true), new(2, 6, 8, true),
        ];
        return (new WarehouseGrid(12, 9, racks), new GridPoint(6, 8));
    }

    private static int TourLength(WarehouseGrid grid, GridPoint depot, IReadOnlyList<GridPoint> order)
    {
        var points = order.Prepend(depot).Append(depot).ToList();
        var length = 0;
        for (var i = 0; i + 1 < points.Count; i++)
            length += grid.ShortestPaths(points[i]).DistanceTo(points[i + 1])!.Value;
        return length;
    }

    private static int BruteForceOptimum(WarehouseGrid grid, GridPoint depot, IReadOnlyList<GridPoint> stops) =>
        Permutations(stops.ToList()).Min(order => TourLength(grid, depot, order));

    private static IEnumerable<List<GridPoint>> Permutations(List<GridPoint> items)
    {
        if (items.Count <= 1)
        {
            yield return items;
            yield break;
        }
        for (var i = 0; i < items.Count; i++)
        {
            var rest = items.Where((_, j) => j != i).ToList();
            foreach (var tail in Permutations(rest))
                yield return [items[i], .. tail];
        }
    }
}
