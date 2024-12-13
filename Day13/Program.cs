using System.Text.RegularExpressions;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var games = lines
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Chunk(3)
    .Select(Game.Create)
    .ToArray();

var result1 = games.Select(game => game.Solve().Cost).Sum();
Console.WriteLine(result1);

const long offset = 10000000000000;
var result2 = games.Select(game => game.Solve(offset).Cost).Sum();
Console.WriteLine(result2);

record Game(Button A, Button B, long TargetX, long TargetY)
{
    public static Game Create(string[] gameLines)
    {
        var numbers = Regex.Matches(gameLines[2], @"\d+").Select(match => long.Parse(match.Value)).ToArray();
        return new Game(Button.Create(gameLines[0]), Button.Create(gameLines[1]), numbers[0], numbers[1]);
    }

    public (bool IsSolved, long Cost) Solve(long offset = 0) => Solve(TargetX + offset, TargetY + offset);

    private (bool IsSolved, long Cost) Solve(long targetX, long targetY)
    {
        var bPresses = (targetX * A.Y - targetY * A.X) / (B.X * A.Y - B.Y * A.X);
        var aPresses = (targetY - bPresses * B.Y) / A.Y;

        if (aPresses * A.X + bPresses * B.X == targetX && aPresses * A.Y + bPresses * B.Y == targetY)
        {
            var cost = aPresses * A.Cost + bPresses * B.Cost;
            return (IsSolved: true, cost);
        }

        return (IsSolved: false, Cost: 0);
    }
};

record Button(int Cost, long X, long Y)
{
    public static Button Create(string config)
    {
        var cost = config.StartsWith("Button A") ? 3 : 1;
        var numbers = Regex.Matches(config, @"\d+").Select(match => long.Parse(match.Value)).ToArray();
        return new Button(cost, numbers[0], numbers[1]);
    }
};