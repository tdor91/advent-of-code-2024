using Common;

using Cache = System.Collections.Generic.Dictionary<(Path Path, int Iteration), long>;

const int numberOfRobots = 25; // 2 for part 1
const string inputFile = "input.txt";
var codes = await File.ReadAllLinesAsync(inputFile);

Cache resultCache = [];
var numKeyboard = new NumKeyboard();
var dirKeyboard = new DirKeyboard();

long result = 0;
foreach (var code in codes)
{
    var numRobotKeyPresses = Decode(code, numKeyboard);

    var minKeyPresses = long.MaxValue;
    foreach (var sequence in numRobotKeyPresses)
    {
        var pairs = sequence.Prepend('A').Pairwise(); // append 'A' as starting position

        long sum = 0;
        foreach (var (from, to) in pairs)
        {
            sum += FindLowestKeyPresses(from, to, 1, numberOfRobots, resultCache, dirKeyboard);
        }

        if (sum < minKeyPresses)
        {
            minKeyPresses = sum;
        }
    }

    result += minKeyPresses * int.Parse(code[..^1]);
}
Console.WriteLine(result);

// Provides all possible sequences of a "parent" keyboard.
// e.g. "029A" -> "<A^A>^^AvvvA", "<A^A^^>AvvvA", ...
List<string> Decode(string code, IKeyboard keyboard)
{
    List<string> sequences = [];
    var start = 'A';
    foreach (var c in code)
    {
        var end = c;
        // append 'A' to activate the robot
        var paths = FindShortestPaths(start, end, keyboard).Select(p => p + 'A');
        sequences = Permutate(sequences, paths);
        start = end;
    }
    return sequences;

    List<string> Permutate(IEnumerable<string> a, IEnumerable<string> b)
    {
        if (a.Count() == 0) return b.ToList();

        List<string> result = [];
        foreach (var x in a) foreach (var y in b) result.Add(x + y);
        return result;
    }
}

long FindLowestKeyPresses(char start, char end, int currentIteration, int iterationLimit, Cache cache, IKeyboard keyboard)
{
    var path = new Path(start, end);

    if (cache.TryGetValue((path, currentIteration), out var value))
    {
        return value;
    }

    var minKeyPresses = long.MaxValue;
    // prepend 'A' because it's always the starting position; append 'A' to activate
    var sequences = FindShortestPaths(start, end, keyboard).Select(p => 'A' + p + 'A');
    foreach (var sequence in sequences)
    {
        var pairs = sequence.Pairwise();

        long sum = 0;
        foreach (var pair in pairs)
        {
            if (currentIteration >= iterationLimit)
            {
                sum += 1;
            }
            else
            {
                sum += FindLowestKeyPresses(pair.A, pair.B, currentIteration + 1, iterationLimit, cache, keyboard);
            }
        }

        if (sum < minKeyPresses)
        {
            minKeyPresses = sum;
        }
    }

    cache.Add((path, currentIteration), minKeyPresses);

    return minKeyPresses;
}

// Dijkstra's Algorithm... again, but this time with a cache
IEnumerable<string> FindShortestPaths(char startChar, char targetChar, IKeyboard keyboard)
{
    if (startChar == targetChar)
    {
        // return an empty string to indicate that you dont have to move but an action is still required
        // (i.e. 'A' has to be pressed either way)
        return [""];
    }

    if (keyboard.Cache.TryGetValue((startChar, targetChar), out var cacheHit))
    {
        return cacheHit;
    }

    List<string> foundPaths = [];

    var start = keyboard.Keys.Points().First(p => keyboard.Keys.GetSymbol(p) == startChar);
    var target = keyboard.Keys.Points().First(p => keyboard.Keys.GetSymbol(p) == targetChar);

    var startNode = new Node(start, AccumulatedCost: 0, Parent: null);

    PriorityQueue<Node, int> queue = new([(startNode, startNode.AccumulatedCost)]);
    Dictionary<string, Node> shortestPaths = new() { [startNode.Key] = startNode };

    int costOfShortestPath = int.MaxValue;
    while (queue.Count > 0)
    {
        var node = queue.Dequeue();

        foreach (var n in node.GetNeighbours(keyboard.Keys))
        {
            if (n.Position == target && n.AccumulatedCost <= costOfShortestPath)
            {
                costOfShortestPath = n.AccumulatedCost;
                var path = n
                    .ReconstructPath()
                    .Select(x => x.Position)
                    .Pairwise()
                    .Select(pair => keyboard.ArrowMap[pair.B - pair.A])
                    .ToArray();

                foundPaths.Add(new string(path));
            }
            else if (!shortestPaths.ContainsKey(n.Key) || n.AccumulatedCost <= shortestPaths[n.Key].AccumulatedCost)
            {
                shortestPaths[n.Key] = n;
                queue.Enqueue(n, n.AccumulatedCost);
            }
        }
    }

    keyboard.Cache.Add((startChar, targetChar), foundPaths.ToArray());
    return foundPaths;
}

interface IKeyboard
{
    public char[][] Keys { get; }
    public Point A { get; }
    public Dictionary<(char start, char end), string[]> Cache { get; }
    public Dictionary<Point, char> ArrowMap { get; }
}

class NumKeyboard : IKeyboard
{
    public char[][] Keys => [
        ['7', '8', '9',],
        ['4', '5', '6',],
        ['1', '2', '3',],
        ['#', '0', 'A',],
    ];

    public Point A => new Point(2, 3);

    public Dictionary<(char start, char end), string[]> Cache { get; } = [];

    public Dictionary<Point, char> ArrowMap => new()
    {
        [new Point(1, 0)] = '>',
        [new Point(-1, 0)] = '<',
        [new Point(0, 1)] = 'v',
        [new Point(0, -1)] = '^',
    };
}

class DirKeyboard : IKeyboard
{
    public char[][] Keys =>
    [
        ['#', '^', 'A',],
        ['<', 'v', '>',],
    ];

    public Point A => new Point(2, 3);

    public Dictionary<(char start, char end), string[]> Cache { get; } = [];

    public Dictionary<Point, char> ArrowMap => new()
    {
        [new Point(1, 0)] = '>',
        [new Point(-1, 0)] = '<',
        [new Point(0, 1)] = 'v',
        [new Point(0, -1)] = '^',
    };
}

record Path(char Start, char End);