using System.Text.RegularExpressions;
using Common;

const int width = 101;
const int height = 103;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var robots = lines.Select(line => new Robot(line, width, height)).ToList();

for (int i = 1;; i++)
{
    robots.ForEach(robot => robot.Move());

    if (i == 100)
    {
        var result1 = robots
            .Select(robot => robot.GetQuadrant())
            .Where(quadrant => quadrant is not null)
            .GroupBy(quadrant => quadrant)
            .Select(group => group.Count())
            .Aggregate(seed: 1, (a, b) => a * b);
        Console.WriteLine(result1);
    }
    
    if (ProbablyHasChristmasTree(robots, width, height, print: true))
    {
        Console.WriteLine($"------- Result 2 (Iteration): {i} -------");
        break;
    }
}


bool ProbablyHasChristmasTree(IEnumerable<Robot> robots, int width, int height, bool print)
{
    char[][] matrix = Enumerable.Range(0, height)
        .Select(_ => Enumerable.Repeat(' ', width).ToArray())
        .ToArray();

    foreach (var robot in robots)
    {
        matrix[robot.Position.Y][robot.Position.X] = '#';
    }
    
    if (matrix.Select(row => new string(row)).Any(row => row.Contains("#########")))
    {
        // break here: check the output for a christmas tree
        if (print) matrix.Print();
        
        return true;
    }

    return false;
}

record Robot
{
    public Robot(string definition, int width, int height)
    {
        var numbers = Regex.Matches(definition, @"-?\d+").Select(match => int.Parse(match.Value)).ToArray();
        Position = new Point(numbers[0], numbers[1]);
        Step = new Point(numbers[2], numbers[3]);

        Width = width;
        Height = height;
    }

    public Point Position { get; set; }
    public Point Step { get; }

    public int Width { get; }
    public int Height { get; }

    public void Move()
    {
        var newX = (Position.X + Step.X + Width) % Width;
        var newY = (Position.Y + Step.Y + Height) % Height;
        Position = new Point(newX, newY);
    }

    public int? GetQuadrant()
    {
        var (midX, midY) = (Width / 2, Height / 2);
        
        if (Position.X < midX && Position.Y < midY) return 1;
        if (Position.X > midX && Position.Y < midY) return 2;
        if (Position.X < midX && Position.Y > midY) return 3;
        if (Position.X > midX && Position.Y > midY) return 4;
        return null;
    }
};