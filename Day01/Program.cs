const string inputFile = @"input.txt";

var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines
    .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    .Select(parts => (Left: int.Parse(parts[0]), Right: int.Parse(parts[1])))
    .ToArray();

var leftValues = input.Select(x => x.Left).ToArray();
var rightValues = input.Select(x => x.Right).ToArray();

var result1 = leftValues.Order().Zip(rightValues.Order(), (left, right) => Math.Abs(left - right)).Sum();
Console.WriteLine(result1);

var result2 = leftValues.Select(left => left * rightValues.Count(right => right == left)).Sum();
Console.WriteLine(result2);

Console.WriteLine("done");