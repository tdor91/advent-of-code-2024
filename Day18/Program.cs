using System.Text;
using Common;

const int size = 70;
const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var obstacles = lines.Select(Point.Parse).ToArray();

var start = new Point(0, 0);
var end = new Point(size, size);

var (_, endNode) = FindShortestPath(start, end, obstacles.Take(1024).ToArray(), size);
var result1 = endNode.AccumulatedCost;
Console.WriteLine(result1);

var lastPossibleIndex = 0;
var range = 0..(obstacles.Length - 1);
int from = 0, to = obstacles.Length;
while (from < to)
{
    int index = (from + to) / 2;
    var currentObstacles = obstacles.Take(index).ToArray();
    var (foundPath, _) = FindShortestPath(start, end, currentObstacles, size);

    if (foundPath)
    {
        lastPossibleIndex = index;
        from = index + 1;
    }
    else
    {
        to = index;
    }
}
var result2 = obstacles[lastPossibleIndex];
Console.WriteLine($"{result2.X},{result2.Y}");

// Dijkstra's Algorithm
(bool FoundPath, Node EndNode) FindShortestPath(Point start, Point target, Point[] obstacles, int size)
{
    var startNode = new Node(start, AccumulatedCost: 0, Parent: null);

    PriorityQueue<Node, int> queue = new([(startNode, startNode.AccumulatedCost)]);
    Dictionary<string, Node> shortestPaths = new() { [startNode.Key] = startNode };

    while (queue.Count > 0)
    {
        var node = queue.Dequeue();

        foreach (var n in node.GetNeighbours(size, obstacles))
        {
            if (n.Position == target) return (true, n);

            if (!shortestPaths.ContainsKey(n.Key) || n.AccumulatedCost < shortestPaths[n.Key].AccumulatedCost) // relaxed
            {
                shortestPaths[n.Key] = n;
                queue.Enqueue(n, n.AccumulatedCost);
            }
        }
    }

    return (false, startNode);
}

record Node(Point Position, int AccumulatedCost, Node? Parent)
{
    public string Key => Position.ToString();

    public IEnumerable<Node> GetNeighbours(int size, Point[] obstacles)
    {
        List<Point> nextPositions = [Position.Add(0, 1), Position.Add(1, 0), Position.Add(0, -1), Position.Add(-1, 0)];
        foreach (var nextPosition in nextPositions)
        {
            if (IsWithinBounds(nextPosition, size) && !obstacles.Contains(nextPosition))
            {
                yield return new Node(nextPosition, AccumulatedCost + 1, this);
            }
        }
    }

    private bool IsWithinBounds(Point p, int size) => p.X >= 0 && p.Y >= 0 && p.Y <= size && p.X <= size;
}