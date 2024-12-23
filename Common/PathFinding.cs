namespace Common;

public record Node(Point Position, int AccumulatedCost, Node? Parent)
{
    public virtual string Key => Position.ToString();

    public virtual IEnumerable<Node> GetNeighbours(char[][] matrix)
    {
        List<Point> nextPositions = [Position.Add(0, 1), Position.Add(1, 0), Position.Add(0, -1), Position.Add(-1, 0)];
        foreach (var nextPosition in nextPositions)
        {
            if (matrix.Contains(nextPosition) && matrix.GetSymbol(nextPosition) != '#')
            {
                yield return new Node(nextPosition, AccumulatedCost + 1, this);
            }
        }
    }

    public virtual IEnumerable<Node> GetNeighbours(int size, Point[] obstacles)
    {
        List<Point> nextPositions = [Position.Add(0, 1), Position.Add(1, 0), Position.Add(0, -1), Position.Add(-1, 0)];
        foreach (var nextPosition in nextPositions)
        {
            if (IsWithinBounds(nextPosition, size) && !obstacles.Contains(nextPosition))
            {
                yield return new Node(nextPosition, AccumulatedCost + 1, this);
            }
        }

        bool IsWithinBounds(Point p, int size) => p.X >= 0 && p.Y >= 0 && p.Y <= size && p.X <= size;
    }


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
}

public static class PathFinding
{
    // Dijkstra's Algorithm
    public static (bool FoundPath, Node EndNode) FindShortestPath(Point start, Point target,
        Func<Node, IEnumerable<Node>> getNeighbours)
    {
        var startNode = new Node(start, AccumulatedCost: 0, Parent: null);

        PriorityQueue<Node, int> queue = new([(startNode, startNode.AccumulatedCost)]);
        Dictionary<string, Node> shortestPaths = new() { [startNode.Key] = startNode };

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var n in getNeighbours(node))
            {
                if (n.Position == target) return (true, n);

                if (!shortestPaths.ContainsKey(n.Key) || n.AccumulatedCost < shortestPaths[n.Key].AccumulatedCost)
                {
                    shortestPaths[n.Key] = n;
                    queue.Enqueue(n, n.AccumulatedCost);
                }
            }
        }

        return (false, startNode);
    }
}