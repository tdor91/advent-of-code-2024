using Common;

const string inputFile = @"input.txt";

var lines = await File.ReadAllLinesAsync(inputFile);
var reports = lines
    .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    .Select(parts => parts.Select(int.Parse).ToArray())
    .ToArray();

var result1 = reports.Count(IsSafe);
Console.WriteLine(result1);

var result2 = reports.Count(report => IsSafe(report) || IsMostlySafe(report));
Console.WriteLine(result2);

Console.WriteLine("done");

bool IsMostlySafe(int[] report)
{
    var pairs = report.Pairwise().ToArray();
    var firstErrorAsc = pairs.TakeWhile(IsSafeAsc).Count();
    var isMostlySafeAsc = IsSafe(report.WithoutAt(firstErrorAsc))
                          || IsSafe(report.WithoutAt(firstErrorAsc + 1));
    if (isMostlySafeAsc)
    {
        return true;
    }

    var firstErrorDesc = pairs.TakeWhile(IsSafeDesc).Count();
    return IsSafe(report.WithoutAt(firstErrorDesc))
           || IsSafe(report.WithoutAt(firstErrorDesc + 1));
}

bool IsSafe(IEnumerable<int> report)
{
    var pairs = report.Pairwise().ToArray();
    return pairs.All(IsSafeAsc) || pairs.All(IsSafeDesc);
}

bool IsSafeAsc((int A, int B) pair)
{
    return ValidGap(pair) && pair.A < pair.B;
}

bool IsSafeDesc((int A, int B) pair)
{
    return ValidGap(pair) && pair.A > pair.B;
}

bool ValidGap((int A, int B) pair)
{
    return Math.Abs(pair.A - pair.B) is >= 1 and <= 3;
}
