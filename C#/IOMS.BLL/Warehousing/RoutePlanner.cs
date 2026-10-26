namespace IOMS.BLL.Warehousing;

// Egy körút eredménye: a megállók bejárási sorrendje (a bemeneti lista indexei), a teljes cellasorozat
// a depótól a depóig, és a hossza méterben. Az el nem érhető megállók kimaradnak a körútból.
public record RoutePlan(IReadOnlyList<int> Order, IReadOnlyList<GridPoint> Path, int Distance, IReadOnlyList<int> Unreachable);

// Körút tervezése: a depóból indulva minden megállót érint, majd visszatér (utazó ügynök probléma).
// 1. A depó és a megállók között BFS-sel páronkénti távolságmátrix (a polcokat kerülő, valódi gyalogos távolság).
// 2. Kezdő sorrend: mindig a legközelebbi még nem érintett megálló (legközelebbi szomszéd).
// 3. Javítás 2-opt-tal: ha két él cseréje (egy szakasz megfordítása) rövidíti a kört, megcseréljük, amíg van javulás.
// Egy rendelésnél jellemzően néhány-néhány tucat megálló van, erre ez gyors és az optimumhoz közeli.
public static class RoutePlanner
{
    public static RoutePlan Plan(WarehouseGrid grid, GridPoint depot, IReadOnlyList<GridPoint> stops)
    {
        var fromDepot = grid.ShortestPaths(depot);

        // Csak az elérhető megállók kerülnek a körbe; az azonos cellájúak egyetlen megállót alkotnak
        var unreachable = new List<int>();
        var points = new List<GridPoint>();
        var pointIndex = new Dictionary<GridPoint, int>();
        var stopToPoint = new int[stops.Count];
        for (var i = 0; i < stops.Count; i++)
        {
            if (fromDepot.DistanceTo(stops[i]) is null)
            {
                unreachable.Add(i);
                stopToPoint[i] = -1;
                continue;
            }
            if (!pointIndex.TryGetValue(stops[i], out var index))
            {
                index = points.Count;
                pointIndex[stops[i]] = index;
                points.Add(stops[i]);
            }
            stopToPoint[i] = index;
        }

        // A mátrix 0. sora/oszlopa a depó, az i. megálló az i + 1.
        var trees = new List<PathTree> { fromDepot };
        trees.AddRange(points.Select(grid.ShortestPaths));
        var nodes = new List<GridPoint> { depot };
        nodes.AddRange(points);
        var n = nodes.Count;
        var dist = new int[n, n];
        for (var a = 0; a < n; a++)
            for (var b = 0; b < n; b++)
                dist[a, b] = trees[a].DistanceTo(nodes[b])!.Value;

        var tour = NearestNeighbour(dist, n);
        TwoOpt(tour, dist);

        // Cellasorozat: a szakaszok összefűzve, az ismétlődő csatlakozó cellák nélkül
        var path = new List<GridPoint> { depot };
        var closed = tour.Append(0).ToList();
        for (var i = 0; i + 1 < closed.Count; i++)
            path.AddRange(trees[closed[i]].PathTo(nodes[closed[i + 1]]).Skip(1));

        // A bejárási sorrend a bemeneti megállók indexeivel (az azonos cellájúak egymás után)
        var order = tour
            .Skip(1)
            .SelectMany(node => Enumerable.Range(0, stops.Count).Where(i => stopToPoint[i] == node - 1))
            .ToList();

        return new RoutePlan(order, path, TourLength(closed, dist), unreachable);
    }

    // A kör csúcsai a depóval (0) kezdve, a visszatérés nélkül
    private static List<int> NearestNeighbour(int[,] dist, int n)
    {
        var tour = new List<int> { 0 };
        var visited = new bool[n];
        visited[0] = true;
        for (var step = 1; step < n; step++)
        {
            var last = tour[^1];
            var next = -1;
            for (var candidate = 1; candidate < n; candidate++)
                if (!visited[candidate] && (next < 0 || dist[last, candidate] < dist[last, next]))
                    next = candidate;
            visited[next] = true;
            tour.Add(next);
        }
        return tour;
    }

    // A (zárt) kör i..j szakaszának megfordítása, ha az rövidebb: az (i-1, i) és (j, j+1) élek helyett
    // (i-1, j) és (i, j+1). A depó (0. elem) a helyén marad.
    private static void TwoOpt(List<int> tour, int[,] dist)
    {
        var n = tour.Count;
        var improved = true;
        while (improved)
        {
            improved = false;
            for (var i = 1; i < n - 1; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var before = tour[i - 1];
                    var after = tour[(j + 1) % n];
                    var delta = dist[before, tour[j]] + dist[tour[i], after]
                              - dist[before, tour[i]] - dist[tour[j], after];
                    if (delta < 0)
                    {
                        tour.Reverse(i, j - i + 1);
                        improved = true;
                    }
                }
            }
        }
    }

    private static int TourLength(IReadOnlyList<int> closedTour, int[,] dist)
    {
        var length = 0;
        for (var i = 0; i + 1 < closedTour.Count; i++)
            length += dist[closedTour[i], closedTour[i + 1]];
        return length;
    }
}
