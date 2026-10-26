using IOMS.BLL.Warehousing;

namespace IOMS.BLL.Tests.Warehousing;

public class WarehouseGridTests
{
    [Fact]
    public void RackCellsAreNotWalkable()
    {
        var grid = new WarehouseGrid(6, 4, [new RackShape(1, 1, 3, Horizontal: true)]);

        Assert.False(grid.IsWalkable(new(1, 1)));
        Assert.False(grid.IsWalkable(new(3, 1)));
        Assert.True(grid.IsWalkable(new(4, 1)));
        Assert.True(grid.IsWalkable(new(1, 0)));
        Assert.False(grid.IsWalkable(new(-1, 0)));
        Assert.False(grid.IsWalkable(new(6, 0)));
    }

    [Fact]
    public void VerticalRackOccupiesColumn()
    {
        var rack = new RackShape(2, 1, 3, Horizontal: false);
        var grid = new WarehouseGrid(5, 5, [rack]);

        Assert.Equal([new(2, 1), new(2, 2), new(2, 3)], rack.Cells.ToList());
        Assert.False(grid.IsWalkable(new(2, 3)));
        Assert.True(grid.IsWalkable(new(2, 4)));
    }

    [Fact]
    public void AccessPointPrefersEntranceSide()
    {
        var rack = new RackShape(1, 2, 3, Horizontal: true);
        var grid = new WarehouseGrid(6, 5, [rack]);

        Assert.Equal(new GridPoint(2, 3), grid.AccessPoint(rack, bay: 2));
    }

    [Fact]
    public void BackToBackRacksAreAccessedFromTheirOwnAisle()
    {
        var back = new RackShape(1, 2, 3, Horizontal: true);
        var front = new RackShape(1, 3, 3, Horizontal: true);
        var grid = new WarehouseGrid(6, 6, [back, front]);

        Assert.Equal(new GridPoint(1, 1), grid.AccessPoint(back, bay: 1));
        Assert.Equal(new GridPoint(1, 4), grid.AccessPoint(front, bay: 1));
    }

    [Fact]
    public void EnclosedBayHasNoAccessPoint()
    {
        var middle = new RackShape(0, 1, 2, Horizontal: true);
        var grid = new WarehouseGrid(2, 3, [middle, new RackShape(0, 0, 2, true), new RackShape(0, 2, 2, true)]);

        Assert.Null(grid.AccessPoint(middle, bay: 1));
    }

    [Fact]
    public void ShortestPathGoesAroundRacks()
    {
        // . . . . .
        // S # # # T
        // . . . . .
        var grid = new WarehouseGrid(5, 3, [new RackShape(1, 1, 3, Horizontal: true)]);
        var tree = grid.ShortestPaths(new(0, 1));

        Assert.Equal(6, tree.DistanceTo(new(4, 1)));

        var path = tree.PathTo(new(4, 1));
        Assert.Equal(7, path.Count);
        Assert.Equal(new GridPoint(0, 1), path[0]);
        Assert.Equal(new GridPoint(4, 1), path[^1]);
        AssertContiguousAndWalkable(grid, path);
    }

    [Fact]
    public void UnreachableCellHasNoDistanceOrPath()
    {
        // A (2, 0) cellát a polcok és a fal elzárja
        var grid = new WarehouseGrid(3, 2, [new RackShape(1, 0, 1, true), new RackShape(1, 1, 2, true)]);
        var tree = grid.ShortestPaths(new(0, 0));

        Assert.Null(tree.DistanceTo(new(2, 0)));
        Assert.Empty(tree.PathTo(new(2, 0)));
    }

    [Fact]
    public void StartMustBeWalkable()
    {
        var grid = new WarehouseGrid(3, 3, [new RackShape(1, 1, 1, true)]);

        Assert.Throws<ArgumentException>(() => grid.ShortestPaths(new(1, 1)));
    }

    internal static void AssertContiguousAndWalkable(WarehouseGrid grid, IReadOnlyList<GridPoint> path)
    {
        Assert.All(path, p => Assert.True(grid.IsWalkable(p), $"({p.X}, {p.Y}) nem járható"));
        for (var i = 1; i < path.Count; i++)
            Assert.Equal(1, Math.Abs(path[i].X - path[i - 1].X) + Math.Abs(path[i].Y - path[i - 1].Y));
    }
}
