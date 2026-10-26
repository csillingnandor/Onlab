namespace IOMS.BLL.Warehousing;

// Egy cella a raktár rácsán (1 cella = 1 m); X jobbra, Y a bejárat felé nő
public readonly record struct GridPoint(int X, int Y);

// Egy polc alakja a rácson: az (X, Y) cellától Length cellát foglal, vízszintesen (X irányban) vagy függőlegesen
public readonly record struct RackShape(int X, int Y, int Length, bool Horizontal)
{
    public GridPoint BayCell(int bay) => Horizontal ? new(X + bay - 1, Y) : new(X, Y + bay - 1);

    public IEnumerable<GridPoint> Cells => Enumerable.Range(1, Length).Select(BayCell);
}

// A raktár alaprajza járható (folyosó) és foglalt (polc) cellákkal. Útkeresés: szélességi bejárás (BFS)
// 4 irányban, egységnyi lépésköltséggel; a rács kicsi (néhány száz cella), ezért ez elég, A* nem kell.
public class WarehouseGrid
{
    private static readonly (int Dx, int Dy)[] Directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    private readonly bool[,] _blocked;

    public int Width { get; }
    public int Depth { get; }

    public WarehouseGrid(int width, int depth, IEnumerable<RackShape> racks)
    {
        if (width <= 0 || depth <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Az alaprajz mérete pozitív kell legyen.");

        Width = width;
        Depth = depth;
        _blocked = new bool[width, depth];
        foreach (var cell in racks.SelectMany(r => r.Cells))
            if (Contains(cell))
                _blocked[cell.X, cell.Y] = true;
    }

    public bool Contains(GridPoint p) => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Depth;

    public bool IsWalkable(GridPoint p) => Contains(p) && !_blocked[p.X, p.Y];

    // A rekesz előtti folyosócella, ahonnan a tárolóhelyek elérhetők. Ha a polc mindkét oldalról járható,
    // a bejárat felőli (nagyobb Y / X) oldal; null, ha egyik oldalról sem (pl. két polc közé zárt rekesz).
    public GridPoint? AccessPoint(RackShape rack, int bay)
    {
        var cell = rack.BayCell(bay);
        GridPoint front = rack.Horizontal ? new(cell.X, cell.Y + 1) : new(cell.X + 1, cell.Y);
        GridPoint back = rack.Horizontal ? new(cell.X, cell.Y - 1) : new(cell.X - 1, cell.Y);

        if (IsWalkable(front)) return front;
        if (IsWalkable(back)) return back;
        return null;
    }

    // Legrövidebb utak egy kezdőcellából az összes járható cellába
    public PathTree ShortestPaths(GridPoint start)
    {
        if (!IsWalkable(start))
            throw new ArgumentException($"A kezdőcella ({start.X}, {start.Y}) nem járható.", nameof(start));

        var distance = new int[Width, Depth];
        var previous = new GridPoint?[Width, Depth];
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Depth; y++)
                distance[x, y] = -1;

        var queue = new Queue<GridPoint>();
        distance[start.X, start.Y] = 0;
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var (dx, dy) in Directions)
            {
                var next = new GridPoint(current.X + dx, current.Y + dy);
                if (!IsWalkable(next) || distance[next.X, next.Y] >= 0)
                    continue;
                distance[next.X, next.Y] = distance[current.X, current.Y] + 1;
                previous[next.X, next.Y] = current;
                queue.Enqueue(next);
            }
        }

        return new PathTree(start, distance, previous);
    }
}

// Egy BFS eredménye: távolság és visszavezető út a kezdőcellából
public class PathTree
{
    private readonly int[,] _distance;
    private readonly GridPoint?[,] _previous;

    public GridPoint Start { get; }

    internal PathTree(GridPoint start, int[,] distance, GridPoint?[,] previous)
    {
        Start = start;
        _distance = distance;
        _previous = previous;
    }

    // Lépések száma (m); null, ha a cella nem érhető el
    public int? DistanceTo(GridPoint p)
    {
        if (p.X < 0 || p.Y < 0 || p.X >= _distance.GetLength(0) || p.Y >= _distance.GetLength(1))
            return null;
        var d = _distance[p.X, p.Y];
        return d < 0 ? null : d;
    }

    // A kezdőcellától a célig (mindkettőt beleértve); üres, ha a cél nem érhető el
    public IReadOnlyList<GridPoint> PathTo(GridPoint target)
    {
        if (DistanceTo(target) is null)
            return [];

        var path = new List<GridPoint>();
        for (GridPoint? p = target; p is not null; p = _previous[p.Value.X, p.Value.Y])
            path.Add(p.Value);
        path.Reverse();
        return path;
    }
}
