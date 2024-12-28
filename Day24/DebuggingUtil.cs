using OperationMap = System.Collections.Generic.Dictionary<string, System.Func<bool, bool, bool>>;

internal static class DebuggingUtil
{
    public static List<(string a, string b)[]> GenerateSwapPairs(List<string> invalidGates)
    {
        var possibleSwaps = new List<(string a, string b)>();
        for (int i = 0; i < invalidGates.Count - 1; i++)
        {
            for (int j = i + 1; j < invalidGates.Count; j++)
            {

                possibleSwaps.Add((invalidGates[i], invalidGates[j]));

            }
        }

        var swapPairs = new HashSet<(string a, string b)[]>();
        for (int i = 0; i < possibleSwaps.Count - 1; i++)
        {
            for (int j = i + 1; j < possibleSwaps.Count; j++)
            {
                for (int k = j + 1; k < possibleSwaps.Count; k++)
                {
                    for (int l = k + 1; l < possibleSwaps.Count; l++)
                    {
                        {
                            swapPairs.Add([possibleSwaps[i], possibleSwaps[j], possibleSwaps[k], possibleSwaps[l]]);
                        }
                    }
                }
            }
        }

        return swapPairs.ToList();
    }

    public static Dictionary<string, bool> CreateInputs(long x, long y, Dictionary<string, bool> inputs)
    {
        int xLength = inputs.Keys.Count(k => k.StartsWith("x"));
        int yLength = inputs.Keys.Count(k => k.StartsWith("y"));

        var xs = Convert.ToString(x, 2).PadLeft(xLength, '0');
        var ys = Convert.ToString(y, 2).PadLeft(yLength, '0');

        if (xs.Length > xLength)
        {
            throw new ArgumentException("x is too large");
        }

        if (ys.Length > yLength)
        {
            throw new ArgumentException("y is too large");
        }

        Dictionary<string, bool> result = new();
        for (int i = 0; i < xs.Length; i++)
        {
            var wire = $"x{i.ToString("D2")}";
            result[wire] = xs[i] == '1';
        }

        for (int i = 0; i < ys.Length; i++)
        {
            var wire = $"y{i.ToString("D2")}";
            result[wire] = ys[i] == '1';
        }

        return result;
    }

    public static Dictionary<string, Gate> SwapGates(Dictionary<string, Gate> gates, params IEnumerable<(string a, string b)> swaps)
    {
        var copy = gates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy());
        foreach (var (a, b) in swaps)
        {
            var temp = copy[a];
            copy[a] = copy[b];
            copy[b] = temp;
        }
        return copy;
    }

    public static void Test(
        long x,
        long y,
        Dictionary<string, bool> inputs,
        Dictionary<string, Gate> gates,
        IEnumerable<(string a, string b)> swaps,
        Func<Dictionary<string, Gate>, Dictionary<string, bool>, Dictionary<string, bool>> resolveWires)
    {
        var newGates = SwapGates(gates, swaps);
        var testInputs = CreateInputs(x, y, inputs);
        var binaryString = resolveWires(newGates, testInputs)
            .Where(kvp => kvp.Key.StartsWith("z"))
            .OrderByDescending(kvp => kvp.Key)
            .Select(kvp => kvp.Value)
            .Aggregate("", (agg, b) => agg + (b ? "1" : "0"));

        var result = Convert.ToInt64(binaryString, 2);

        Console.WriteLine($"expected: {x} + {y} = {x + y} == {result}");
        Console.WriteLine(Convert.ToString(x, 2).PadLeft(50));
        Console.WriteLine(Convert.ToString(y, 2).PadLeft(50));
        Console.WriteLine(Convert.ToString(result, 2).PadLeft(50) + " <-- actual");
        Console.WriteLine(Convert.ToString(x + y, 2).PadLeft(50) + " <-- expected");

        if (x + y == result)
        {
            Console.WriteLine("we found it :)");
        }
    }
}
