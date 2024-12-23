using System.Diagnostics;
using Common;

const string inputFile = @"input.txt";
Stopwatch sw = Stopwatch.StartNew();
var input = await MatrixExtensions.Parse(inputFile);
var start = input.Points().First(p => input.GetSymbol(p) == 'S');
var end = input.Points().First(p => input.GetSymbol(p) == 'E');

var pathFindingResult = PathFinding.FindShortestPath(start, end, node => node.GetNeighbours(input));
var path = pathFindingResult.EndNode.ReconstructPath().Select(n => n.Position).ToArray().AsReadOnly();

const int minimumSaving = 100;

long result1 = 0;
foreach (var (shortcutFrom, fromIndex) in path.Select((p, i) => (p, i)))
{
    var shortcutsTo = path
        .Select((p, i) => (ShortcutTo: p, ToIndex: i))
        .Skip(fromIndex + minimumSaving)
        .Where(x => shortcutFrom.ManhattanDistanceTo(x.ShortcutTo) == 2);

    result1 += shortcutsTo.Count(x => SavedSteps(fromIndex, x.ToIndex, 2) >= minimumSaving);
}

Console.WriteLine(result1);
Console.WriteLine($"Elapsed total time: {sw.ElapsedMilliseconds}ms");

long result2 = 0;
foreach (var (shortcutFrom, fromIndex) in path.Select((p, i) => (p, i)))
{
    var shortcutsTo = path
        .Select((p, i) => (ShortcutTo: p, ToIndex: i))
        .Skip(fromIndex + minimumSaving)
        .Select(x => (x.ToIndex, Steps: shortcutFrom.ManhattanDistanceTo(x.ShortcutTo)))
        .Where(x => x.Steps <= 20);

    result2 += shortcutsTo.Count(x => SavedSteps(fromIndex, x.ToIndex, x.Steps) >= minimumSaving);
}

Console.WriteLine(result2);
Console.WriteLine($"Elapsed total time: {sw.ElapsedMilliseconds}ms");
Console.WriteLine("done");

int SavedSteps(int from, int to, int steps) => to - from - steps;