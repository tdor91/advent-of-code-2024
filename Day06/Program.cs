using Common;

const string inputFile = @"demo-input.txt";
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
var visitedPositions = Move(input, startPos, startingSteps[input.GetSymbol(startPos)], new());
var result1 = visitedPositions.Count();
Console.WriteLine(result1);

Dictionary<Point, List<Point>> hist = new()
{
    [new Point(0, -1)] = [],
    [new Point(1, 0)] = [],
    [new Point(0, 1)] = [],
    [new Point(-1, 0)] = [],
};

Dictionary<Point, List<Point>> Move2(char[][] matrix, Point start, Point step, Dictionary<Point, List<Point>> history)
{
    history[step].Add(start);
    
    // CONTINUE: if "next step" could hit a historic step, we could create a loop -> store and move on
    
    Point target = start + step;
    if (!matrix.Contains(target))
    {
        return history;
    }

    if (matrix.GetSymbol(target) != '#')
    {
        return Move(matrix, target, step, visited);
    }
    else
    {
        var newStep = nextDirection[step];
        var newtarget = start + newStep;
        return Move(matrix, newtarget, newStep, visited);
    }
}

HashSet<Point> Move(char[][] matrix, Point start, Point step, HashSet<Point> visited)
{
    visited.Add(start);
    
    Point target = start + step;
    if (!matrix.Contains(target))
    {
        return visited;
    }

    if (matrix.GetSymbol(target) != '#')
    {
        return Move(matrix, target, step, visited);
    }
    else
    {
        var newStep = nextDirection[step];
        var newtarget = start + newStep;
        return Move(matrix, newtarget, newStep, visited);
    }
}