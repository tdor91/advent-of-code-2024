using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines.Select(line => line.ToCharArray()).ToArray();

HashSet<Point> visited = [];
List<Region> regions = [];
foreach (var point in input.Points())
{
    if (visited.Contains(point)) continue;

    var symbol = input.GetSymbol(point);
    var group = Expand(input, point, symbol, visited).ToArray();
    regions.Add(new Region(symbol, group));
    visited.UnionWith(group);
}

var result1 = regions.Select(region => region.CalculatePrice()).Sum();
Console.WriteLine(result1);

var result2 = regions.Select(region => region.CalculateDiscountedPrice()).Sum();
Console.WriteLine(result2);


IEnumerable<Point> Expand(char[][] matrix, Point point, char symbol, HashSet<Point> visited)
{
    if (visited.Contains(point) || !matrix.Contains(point) || matrix.GetSymbol(point) != symbol)
    {
        yield break;
    }

    visited.Add(point);
    yield return point;

    foreach (var step in new[] { point.Add(1, 0), point.Add(0, 1), point.Add(-1, 0), point.Add(0, -1) })
    {
        foreach (var nextPoint in Expand(matrix, step, symbol, visited))
        {
            yield return nextPoint;
        }
    }
}

class Region(char symbol, Point[] points)
{
    public char Symbol { get; } = symbol;

    public Point[] Points { get; } = points;

    public int CalculatePrice()
    {
        var area = Points.Length;
        var perimeter = GetPerimeter();
        return area * perimeter;
    }

    public int CalculateDiscountedPrice()
    {
        var area = Points.Length;
        var sides = GetSides();
        return area * sides;
    }

    private int GetPerimeter()
    {
        int perimeter = 0;
        foreach (var point in Points)
        {
            perimeter += 4 - GetNeighbours(point).Count();
        }

        return perimeter;
    }

    private IEnumerable<Point> GetNeighbours(Point point)
    {
        foreach (var other in Points)
        {
            if (Math.Abs(other.X - point.X) == 1 && other.Y == point.Y ||
                Math.Abs(other.Y - point.Y) == 1 && other.X == point.X)
            {
                yield return other;
            }
        }
    }

    private bool AreAdjacent(Direction direction, Point a, Point b)
    {
        if (direction is Direction.Top or Direction.Bottom)
        {
            return a.Y == b.Y && Math.Abs(a.X - b.X) == 1;
        }

        if (direction is Direction.Left or Direction.Right)
        {
            return a.X == b.X && Math.Abs(a.Y - b.Y) == 1;
        }

        throw new ArgumentException("Invalid direction");
    }

    private int GetSides()
    {
        var edges = Points.SelectMany(GetEdgeDirections);

        var edgeGroups = edges
            .GroupBy(e => e.Direction)
            .ToDictionary(
                g => g.Key, 
                g => g.OrderBy(e => e.Pos.X).ThenBy(e => e.Pos.Y).ToList());

        int sides = 0;
        foreach (var edgeGroup in edgeGroups)
        {
            HashSet<Point> knownEdges = [];
            foreach (var edge in edgeGroup.Value)
            {
                if (!knownEdges.Any(knownEdge => AreAdjacent(edge.Direction, edge.Pos, knownEdge)))
                {
                    sides++;
                }

                knownEdges.Add(edge.Pos);
            }
        }
        
        return sides;
    }
    
    private IEnumerable<(Point Pos, Direction Direction)> GetEdgeDirections(Point point)
    {
        if (!Points.Contains(point.Add(1, 0))) yield return (point, Direction.Right);
        if (!Points.Contains(point.Add(0, 1))) yield return (point, Direction.Bottom);
        if (!Points.Contains(point.Add(-1, 0))) yield return (point, Direction.Left);
        if (!Points.Contains(point.Add(0, -1))) yield return (point, Direction.Top);
    }
}

enum Direction
{
    Top,
    Left,
    Bottom,
    Right
}