using Common;

Dictionary<char, Point> stepMap = new()
{
    ['^'] = new Point(0, -1),
    ['>'] = new Point(1, 0),
    ['v'] = new Point(0, 1),
    ['<'] = new Point(-1, 0),
};

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var matrix = lines.Where(line => line.StartsWith('#')).Select(line => line.ToCharArray()).ToArray();
var moves = lines.Where(line => !line.StartsWith('#')).SelectMany(line => line.ToCharArray()).ToArray();

var robotPosition = matrix.Points().First(p => matrix.GetSymbol(p) == '@');
foreach (var step in moves.Select(c => stepMap[c]))
{
    var nextRobotPosition = robotPosition + step;
    var nextSymbol = matrix.GetSymbol(nextRobotPosition);

    if (nextSymbol == '.')
    {
        robotPosition = UpdateRobotPosition(matrix, robotPosition, nextRobotPosition);
    }

    if (nextSymbol == 'O')
    {
        List<Point> boxes = [nextRobotPosition];
        var limitingPosition = nextRobotPosition + step;
        var limitingSymbol = matrix.GetSymbol(limitingPosition);

        while (limitingSymbol == 'O')
        {
            boxes.Add(limitingPosition);
            limitingPosition += step;
            limitingSymbol = matrix.GetSymbol(limitingPosition);
        }

        if (limitingSymbol == '.')
        {
            robotPosition = UpdateRobotPosition(matrix, robotPosition, nextRobotPosition);
            var newBoxPos = boxes.Last() + step;
            matrix[newBoxPos.Y][newBoxPos.X] = 'O';
        }
    }
}

var result1 = matrix.Points()
    .Where(p => matrix.GetSymbol(p) == 'O')
    .Select(p => p.Y * 100 + p.X)
    .Sum();
Console.WriteLine(result1);

var wideMatrix = lines.Where(line => line.StartsWith('#')).Select(line => Widen(line).ToCharArray()).ToArray();
robotPosition = wideMatrix.Points().First(p => wideMatrix.GetSymbol(p) == '@');
foreach (var step in moves.Select(c => stepMap[c]))
{
    var nextRobotPosition = robotPosition + step;
    var nextSymbol = wideMatrix.GetSymbol(nextRobotPosition);

    if (nextSymbol == '.')
    {
        robotPosition = UpdateRobotPosition(wideMatrix, robotPosition, nextRobotPosition);
    }

    if (nextSymbol is '[' or ']')
    {
        var box = new WideBox(nextRobotPosition, nextSymbol);
        var boxGroup = GetBoxGroup(box, step, wideMatrix);
        
        var limitingSymbols = boxGroup
            .SelectMany(b => b.PointsBehind(step))
            .Except(boxGroup.SelectMany(b => new[] { b.Left, b.Right }))
            .Select(p => wideMatrix.GetSymbol(p))
            .ToList();

        if (limitingSymbols.All(c => c == '.'))
        {
            UpdateWideBoxPositions(wideMatrix, boxGroup, step);
            robotPosition = UpdateRobotPosition(wideMatrix, robotPosition, nextRobotPosition);
        }
    }
}
var result2 = wideMatrix.Points()
    .Where(p => wideMatrix.GetSymbol(p) == '[')
    .Select(p => p.Y * 100 + p.X)
    .Sum();
Console.WriteLine(result2);

string Widen(string line) => line.Replace("#", "##").Replace("O", "[]").Replace(".", "..").Replace("@", "@.");

Point UpdateRobotPosition(char[][] matrix, Point from, Point to)
{
    matrix.SetSymbols('.', from);
    matrix.SetSymbols('@', to);
    return to;
}

void UpdateWideBoxPositions(char[][] matrix, ICollection<WideBox> boxes, Point step)
{
    matrix.SetSymbols('.', boxes.SelectMany(b => b.Points));
    matrix.SetSymbols('[', boxes.Select(b => b.Left + step));
    matrix.SetSymbols(']', boxes.Select(b => b.Right + step));
}

HashSet<WideBox> GetBoxGroup(WideBox box, Point step, char[][] matrix)
{
    var neighbors = new HashSet<WideBox>();
    
    foreach (var pos in box.PointsBehind(step))
    {
        var symbol = matrix.GetSymbol(pos);
        if (symbol is '[' or ']')
        {
            var newBox = new WideBox(pos, symbol);
            neighbors.UnionWith(GetBoxGroup(newBox, step, matrix));
        }
    }

    return [box, ..neighbors];
}

record WideBox(Point Left, Point Right)
{
    public WideBox(Point p, char symbol) : this(symbol == '[' ? p : p.Add(-1, 0), symbol == '[' ? p.Add(1, 0) : p)
    {
        if (symbol != '[' && symbol != ']') throw new ArgumentException("Invalid box.");
    }

    public Point[] Points => [Left, Right];

    public Point[] PointsBehind(Point step)
    {
        return step switch
        {
            { X: -1, Y: 0 } => [Left + step],
            { X: 1, Y: 0 } => [Right + step],
            { Y: 1 } or { Y: -1 } => [Left + step, Right + step],
            _ => throw new Exception("invalid step")
        };
    }
}