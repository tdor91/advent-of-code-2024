using Common;

const string inputFile = @"input.txt";
var input = await MatrixExtensions.Parse(inputFile);

var start = input.Points().First(p => input.GetSymbol(p) == 'S');
var end = input.Points().First(p => input.GetSymbol(p) == 'E');

var shortestPath = FindShortestPaths(input, start, end).First();
var result1 = shortestPath.Last().AccumulatedCost;
Console.WriteLine(result1);

var allShortestPaths = FindShortestPaths(input, start, end);
var result2 = allShortestPaths.SelectMany(n => n).Select(n => n.Position).Distinct().Count();
Console.WriteLine(result2);

// Relaxed Dijkstra's Algorithm
IEnumerable<List<Node>> FindShortestPaths(char[][] matrix, Point start, Point target)
{
    var startNode = new Node(start, AccumulatedCost: 0, Step: new(1, 0), Parent: null);

    PriorityQueue<Node, int> queue = new([(startNode, startNode.AccumulatedCost)]);
    Dictionary<string, Node> shortestPaths = new() { [startNode.Key] = startNode };

    while (queue.Count > 0)
    {
        var node = queue.Dequeue();

        foreach (var n in node.GetNeighbours(matrix))
        {
            if (n.Position == target) yield return n.ReconstructPath();

            if (!shortestPaths.ContainsKey(n.Key) || n.AccumulatedCost <= shortestPaths[n.Key].AccumulatedCost) // relaxed
            {
                shortestPaths[n.Key] = n;
                queue.Enqueue(n, n.AccumulatedCost);
            }
        }
    }
}

record Node(Point Position, int AccumulatedCost, Point Step, Node? Parent)
{
    public string Key => $"({Position.X},{Position.Y}):({Step.X},{Step.Y})";

    public List<Node> ReconstructPath()
    {
        List<Node> path = [];

        var cur = this;
        while (cur != null)
        {
            path.Insert(0, cur);
            cur = cur.Parent;
        }

        return path;
    }

    public IEnumerable<Node> GetNeighbours(char[][] matrix)
    {
        foreach (var (direction, cost) in GetPossibleMoves(Step))
        {
            var pos = Position + direction;
            if (matrix.Contains(pos) && matrix.GetSymbol(pos) != '#')
            {
                yield return new Node(pos, AccumulatedCost + cost, direction, this);
            }
        }
    }

    private IEnumerable<(Point Direction, int Cost)> GetPossibleMoves(Point step)
    {
        yield return (step, 1);

        if (step.X != 0)
        {
            yield return (new Point(0, -1), 1001);
            yield return (new Point(0, 1), 1001);
        }

        if (step.Y != 0)
        {
            yield return (new Point(-1, 0), 1001);
            yield return (new Point(1, 0), 1001);
        }
    }
}
