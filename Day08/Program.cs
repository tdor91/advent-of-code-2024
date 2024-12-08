using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var map = lines.Select(line => line.ToCharArray()).ToArray();

var antennas = ParseAntennaPositions(map); 

HashSet<Point> uniqueAntinodes = [];
foreach (var (_, positions) in antennas)
{
    foreach (var pair in GetPairPermutations(positions))
    {
        var pathAtoB = pair.A.PathTo(pair.B);
        var antinode1 = pair.B + pathAtoB;
        var antinode2 = pair.A + pathAtoB.Invert();
    
        if (map.Contains(antinode1)) uniqueAntinodes.Add(antinode1);
        if (map.Contains(antinode2)) uniqueAntinodes.Add(antinode2);
    }
}
var result1 = uniqueAntinodes.Count;
Console.WriteLine(result1);

HashSet<Point> uniqueAntinodes2 = [];
foreach (var (_, positions) in antennas)
{
    foreach (var pair in GetPairPermutations(positions))
    {
        var pathAtoB = pair.A.PathTo(pair.B);
        
        for (int i = 0;; i++)
        {
            var antinode = pair.B + pathAtoB * i;
            if (!map.Contains(antinode)) break;
            
            uniqueAntinodes2.Add(antinode);
        }
        
        for (int i = 0;; i++)
        {
            var antinode = pair.A + pathAtoB.Invert() * i;
            if (!map.Contains(antinode)) break;
            
            uniqueAntinodes2.Add(antinode);
        }
    }
}
var result2 = uniqueAntinodes2.Count;
Console.WriteLine(result2);

Console.WriteLine("done");

Dictionary<char, List<Point>> ParseAntennaPositions(char[][] matrix)
{
    Dictionary<char, List<Point>> result = new();

    foreach (var point in matrix.Points())
    {
        var symbol = matrix.GetSymbol(point);
        if (symbol == '.') continue;
    
        if (!result.TryGetValue(symbol, out var positions))
        {
            result[symbol] = positions = [];
        }
        positions.Add(point);
    }
    
    return result;
}

IEnumerable<(Point A, Point B)> GetPairPermutations(List<Point> points)
{
    for (int i = 0; i < points.Count; i++)
    {
        for (int j = i + 1; j < points.Count; j++)
        {
            yield return (points[i], points[j]);
        }
    }
}