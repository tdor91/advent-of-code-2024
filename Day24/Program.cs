using System.Text.RegularExpressions;
using Common;
using OperationMap = System.Collections.Generic.Dictionary<string, System.Func<bool, bool, bool>>;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var inputs = ParseInputs(lines);
var gates = ParseGates(lines);

var binaryString = ResolveWires(gates, inputs)
    .Where(kvp => kvp.Key.StartsWith("z"))
    .OrderByDescending(kvp => kvp.Key)
    .Select(kvp => kvp.Value)
    .Aggregate("", (agg, b) => agg + (b ? "1" : "0"));

var result1 = Convert.ToInt64(binaryString, 2);
Console.WriteLine(result1);

var susGates = FindInvalidGates(gates);
var result2 = susGates.OrderBy(x => x).AsString();
Console.WriteLine(result2);

/// <summary>
/// Resolves the value for every wire.
/// </summary>
/// <param name="gates">The gate definitions.</param>
/// <param name="inputs">The input definitions.</param>
/// <param name="operations">Map with the available operations.</param>
/// <returns>All wires with their static value.</returns>
Dictionary<string, bool> ResolveWires(Dictionary<string, Gate> gates, Dictionary<string, bool> inputs)
{
    OperationMap operations = new()
    {
        ["AND"] = (a, b) => a && b,
        ["OR"] = (a, b) => a || b,
        ["XOR"] = (a, b) => a ^ b,
    };

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
    return knownWires;
}

/// <summary>
/// Finds "invalid" gates in the given gates dictionary.
/// </summary>
/// <param name="gates">The gate definitions.</param>
/// <returns>A list of invalid gate keys.</returns>
/// <remarks>
/// Invalid gates are gates that are not conform with the following definition:
/// see: https://en.wikipedia.org/wiki/Adder_(electronics)
/// (a) S = (A XOR B) XOR Cin
/// (b) Cout = ((A XOR B) AND Cin) OR (A AND B)
///     
/// From this we can create the following rules that gates must follow:
/// 1. S = ??? _XOR_ ???;
///   -> z gates must have XOR operator
///
/// 2. Cout = ((A _XOR_ B) AND ???) OR ???
///   -> XOR gates that are not in z gates must have x and y as input
/// 
/// 3. Cout = (??? _AND_ ???) OR (??? _AND_ ???)
///   -> AND gates must be input of an OR gate
/// 
/// 4. Cout = (??? AND ???) _OR_ (??? AND ???)
///   -> OR gates must have AND gates as input
/// </remarks>
List<string> FindInvalidGates(Dictionary<string, Gate> gates)
{
    HashSet<string> susGates = [];

    susGates.UnionWith(Rule1Violations(gates));
    susGates.UnionWith(Rule2Violations(gates));
    susGates.UnionWith(Rule3Violations(gates));
    susGates.UnionWith(Rule4Violations(gates));

    return susGates.ToList();

    HashSet<string> Rule1Violations(Dictionary<string, Gate> gates)
    {
        HashSet<string> set = new();

        var zGates = gates.Where(g => g.Key.StartsWith("z"));
        foreach (var zGate in zGates)
        {
            if (zGate.Key == "z45")
            {
                // special case: last digit
                // there is one more "z" than "x" and "y" -> z gate needs to have the OR from the carry over
                continue;
            }

            if (zGate.Value.Op != "XOR")
            {
                set.Add(zGate.Key);
            }
        }

        return set;
    }

    HashSet<string> Rule2Violations(Dictionary<string, Gate> gates)
    {
        HashSet<string> set = new();

        var xorGatesExceptZ = gates.Where(g => g.Value.Op == "XOR" && !g.Key.StartsWith("z"));
        foreach (var xorGate in xorGatesExceptZ)
        {
            if (!Regex.IsMatch(xorGate.Value.A, "(x|y)..") || !Regex.IsMatch(xorGate.Value.B, "(x|y).."))
            {
                set.Add(xorGate.Key);
            }
        }

        return set;
    }

    HashSet<string> Rule3Violations(Dictionary<string, Gate> gates)
    {
        HashSet<string> set = new();

        // special case: first carry over has "x00 AND y00"
        var andGates = gates.Where(g => g.Value.Op == "AND" && !(g.Value.HasInput("x00") && g.Value.HasInput("y00")));
        foreach (var andGate in andGates)
        {
            var inputIn = gates.Where(g => g.Value.HasInput(andGate.Key));
            if (inputIn.Any(gate => gate.Value.Op != "OR"))
            {
                susGates.Add(andGate.Key);
            }
        }

        return set;
    }

    HashSet<string> Rule4Violations(Dictionary<string, Gate> gates)
    {
        HashSet<string> set = new();

        var orGates = gates.Where(g => g.Value.Op == "OR");
        foreach (var orGate in orGates)
        {
            var inputOf = new[] { gates.First(g => g.Key == orGate.Value.A), gates.First(g => g.Key == orGate.Value.B) };
            foreach (var gate in inputOf)
            {
                if (gate.Value.Op != "AND")
                {
                    set.Add(gate.Key);
                }
            }
        }

        return set;
    }
}

Dictionary<string, bool> ParseInputs(string[] lines)
{
    return lines.Where(line => line.Contains(":"))
        .Select(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries))
        .ToDictionary(
            parts => parts[0],
            parts => parts[1] == "1");
}

Dictionary<string, Gate> ParseGates(string[] lines)
{
    return lines.Where(line => line.Contains("->"))
        .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .ToDictionary(
            parts => parts.Last(),
            parts => new Gate(A: parts[0], B: parts[2], Op: parts[1]));
}

internal record Gate(string A, string B, string Op)
{
    public Gate Copy()
    {
        return new Gate(A, B, Op);
    }

    public override string ToString() => $"{A} {Op} {B}";

    public bool HasInput(string input) => A == input || B == input;
}