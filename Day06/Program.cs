using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines.Select(line => line.ToCharArray()).ToArray();

Dictionary<char, Point> startingSteps = new()
{
    ['^'] = new Point(0, -1),
    ['>'] = new Point(1, 0),
    ['v'] = new Point(0, 1),
    ['<'] = new Point(-1, 0),
};

Dictionary<Point, Point> nextDirection = new()
{
    [new Point(-1, 0)] = new Point(0, -1),
    [new Point(0, -1)] = new Point(1, 0),
    [new Point(1, 0)] = new Point(0, 1),
    [new Point(0, 1)] = new Point(-1, 0),
};

var startPos = input.Points().First(point => "^>v<".Contains(input.GetSymbol(point)));
var visitedPositions = Move(input, startPos, startingSteps[input.GetSymbol(startPos)], []);
var result1 = visitedPositions.Count();
Console.WriteLine(result1);

int result2 = 0;
foreach (var pos in visitedPositions.Skip(1))
{
    var inputCopy = input.Select(a => a.ToArray()).ToArray();
    inputCopy[pos.Y][pos.X] = 'O';

    var hasLoop = FindLoop(inputCopy, startPos, startingSteps[input.GetSymbol(startPos)], []);
    if (hasLoop)
    {
        result2++;
    }
}
Console.WriteLine(result2);

HashSet<Point> Move(char[][] matrix, Point start, Point step, HashSet<Point> visited)
{
    visited.Add(start);

    Point target = start + step;
    if (!matrix.Contains(target))
    {
        return visited;
    }

    var turns = 0;
    while (matrix.GetSymbol(target) is '#' && turns < 4)
    {
        step = nextDirection[step];
        target = start + step;
        turns++;
    }

    if (turns == 4)
    {
        throw new Exception("Invalid input: Guard cannot move.");
    }

    return Move(matrix, target, step, visited);
}

bool FindLoop(char[][] matrix, Point start, Point step, HashSet<(Point, Point)> visited)
{
    if (!visited.Add((start, step)))
    {
        return true;
    }

    Point target = start + step;
    if (!matrix.Contains(target))
    {
        return false;
    }

    var turns = 0;
    while (matrix.GetSymbol(target) is '#' or 'O' && turns < 4)
    {
        step = nextDirection[step];
        target = start + step;
        turns++;
    }

    if (turns == 4)
    {
        throw new Exception("Invalid input: Guard cannot move.");
    }

    return FindLoop(matrix, target, step, visited);
}