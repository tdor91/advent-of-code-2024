using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var input = lines .Select(line => line.Select(c => c.ToInt()).ToArray()).ToArray();

var startingPoints = input.Points().Where(point => input.GetValue(point) == 0);

var trailHeads = startingPoints
    .Select(start => (start, Hike(input, start, -1)))
    .ToDictionary();

var result1 = trailHeads.Values.Select(top => top.Distinct().Count()).Sum();
Console.WriteLine(result1);

var result2 = trailHeads.Values.Select(top => top.Count()).Sum();
Console.WriteLine(result2);

Console.WriteLine("done");

IEnumerable<Point> Hike(int[][] matrix, Point start, int lastValue)
{
    var currentValue = matrix.GetValue(start);

    if (currentValue - lastValue != 1)
    {
        return [];
    }

    if (currentValue == 9)
    {
        return [start];
    }

    return new[] { start.Add(0, 1), start.Add(0, -1), start.Add(1, 0), start.Add(-1, 0) }
        .Where(matrix.Contains)
        .SelectMany(step => Hike(matrix, step, currentValue));
}