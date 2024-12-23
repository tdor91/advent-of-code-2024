using Common;

const int size = 70;
const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var obstacles = lines.Select(Point.Parse).ToArray();

var start = new Point(0, 0);
var end = new Point(size, size);

var (_, endNode) = PathFinding.FindShortestPath(start, end, node => node.GetNeighbours(size, obstacles.Take(1024).ToArray()));
var result1 = endNode.AccumulatedCost;
Console.WriteLine(result1);

var lastPossibleIndex = 0;
int from = 0, to = obstacles.Length;
while (from < to)
{
    int index = (from + to) / 2;
    var currentObstacles = obstacles.Take(index).ToArray();
    var (foundPath, _) = PathFinding.FindShortestPath(start, end, node => node.GetNeighbours(size, currentObstacles));

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
