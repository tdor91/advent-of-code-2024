const string inputFile = @"demo-input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Dictionary<string, Func<bool, bool, bool>> operations = new()
{
    ["AND"] = (a, b) => a && b,
    ["OR"] = (a, b) => a || b,
    ["XOR"] = (a, b) => a ^ b,
};

var inputs = ParseInputs(lines);
var gates = ParseGates(lines);

var unknownWires = gates.Keys.ToList();
var knownWires = inputs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
while (unknownWires.Count > 0)
{
    var wire = unknownWires.First();
    unknownWires.RemoveAt(0);

    var (inputA, inputB, op) = gates[wire];
    if (knownWires.TryGetValue(inputA, out var aVal) && knownWires.TryGetValue(inputB, out var bVal))
    {
        knownWires[wire] = operations[op](aVal, bVal);
    }
    else
    {
        unknownWires.Add(wire);
    }
}

var binaryString = knownWires
    .Where(kvp => kvp.Key.StartsWith("z"))
    .OrderByDescending(kvp => kvp.Key)
    .Select(kvp => kvp.Value)
    .Aggregate("", (agg, b) => agg + (b ? "1" : "0"));

var result1 = Convert.ToInt64(binaryString, 2);
Console.WriteLine(result1);










Console.WriteLine("done");

Dictionary<string, bool> ParseInputs(string[] lines)
{
    return lines.Where(line => line.Contains(":"))
        .Select(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries))
        .ToDictionary(
            parts => parts[0],
            parts => parts[1] == "1");
}

Dictionary<string, (string A, string B, string Op)> ParseGates(string[] lines)
{
    return lines.Where(line => line.Contains("->"))
        .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .ToDictionary(
            parts => parts.Last(),
            parts => (A: parts[0], B: parts[2], Op: parts[1]));
}


//bool Resolve(string a, string b, string op, Dictionary<string, bool> knownWires)
//{
//    var isResolvedA = knownWires.TryGetValue(a, out bool aVal);
//    var isResolvedB = knownWires.TryGetValue(b, out bool bVal);

//    if (isResolvedA && isResolvedB)
//    {
//        return operations[op](aVal, bVal);
//    }
//    else if (isResolvedA && !isResolvedB)
//    {
//        bVal = Resolve(b, gates[b].B, gates[b].Op, knownWires);
//        //knownWires.TryAdd(b, bVal);
//    }
//    else if (!isResolvedA && isResolvedB)
//    {
//        aVal = Resolve(a, gates[a].A, gates[a].Op, knownWires);
//        //knownWires.TryAdd(a, aVal);
//    }

//    return operations[op](aVal, bVal);
//}

//List<bool> binaryValues = new();
//foreach (var z in )
//{
//    binaryValues.Add(x);

//    Console.WriteLine($"{z}: {x}");
//}

//var binaryString = binaryValues.Aggregate("", (agg, b) => agg + (b ? "1" : "0"));