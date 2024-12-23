using Common;

const string inputFile = @"input.txt";
var input = await MatrixExtensions.Parse(inputFile);
var start = input.Points().First(p => input.GetSymbol(p) == 'S');
var end = input.Points().First(p => input.GetSymbol(p) == 'E');

var path = PathFinding
    .FindShortestPath(start, end, node => node.GetNeighbours(input))
    .EndNode.ReconstructPath().Select(n => n.Position).ToArray().AsReadOnly();

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

int SavedSteps(int from, int to, int steps) => to - from - steps;
