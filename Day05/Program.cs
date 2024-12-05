using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

// Key = page, Value = pages that are not allowed _after_ the key
var ruleMap = lines
    .Where(line => line.Contains('|'))
    .Select(line => line.Split('|'))
    .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1])))
    .GroupBy(rule => rule.Item2) // reversed!
    .ToDictionary(group => group.Key, group => group.Select(rule => rule.Item1).ToList());

var orders = lines
    .Where(line => line.Contains(','))
    .Select(line => line.Split(',').ToInts().ToArray())
    .ToArray();

var result1 = orders
    .Where(order => IsCorrectlyOrdered(order, ruleMap))
    .Select(order => order[order.Length / 2])
    .Sum();
Console.WriteLine(result1);

var result2 = orders
    .Where(order => !IsCorrectlyOrdered(order, ruleMap))
    .Select(order => OrderCorrectly(order, ruleMap))
    .Select(order => order[order.Length / 2])
    .Sum();
Console.WriteLine(result2);

Console.WriteLine("done");

bool IsCorrectlyOrdered(int[] order, Dictionary<int, List<int>> rules)
{
    for (int i = 0; i < order.Length - 1; i++)
    {
        if (rules.TryGetValue(order[i], out var invalidSuccessors))
        {
            var remaining = order[(i + 1)..];
            if (remaining.Intersect(invalidSuccessors).Any())
            {
                return false;
            }
        }
    }

    return true;
}

int[] OrderCorrectly(int[] order, Dictionary<int, List<int>> rules)
{
    var result = new List<int>();
    var errors = new List<int>();

    for (int i = 0; i < order.Length; i++)
    {
        var remaining = order[(i + 1)..];
        if (rules.TryGetValue(order[i], out var invalidSuccessors)
            && remaining.Intersect(invalidSuccessors).Any())
        {
            errors.Add(order[i]);
        }
        else
        {
            result.Add(order[i]);
        }
    }

    foreach (var error in errors)
    {
        var invalidSuccessors = ruleMap[error];
        var index = result.FindLastIndex(x => invalidSuccessors.Contains(x));
        result.Insert(index + 1, error);
    }

    return result.ToArray();
}