using System.Collections.Concurrent;
using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var links = ParseInpput(lines);

HashSet<string> groupsWith3 = [];
foreach (var (first, others) in links)
{
    foreach (var second in links[first])
    {
        var intersection = links[first].Intersect(links[second]).ToList();
        foreach (var third in intersection)
        {
            List<string> list = [first, second, third];
            list.Sort();
            groupsWith3.Add(list.ToJoinedString());
        }
    }
}
var result1 = groupsWith3.Count(g => g.Split(",").Any(n => n.StartsWith("t")));
Console.WriteLine(result1);

var cliques = FindAllMaximalCliques(r: [], p: [.. links.Keys], x: [], links);
var maxClique = cliques.MaxBy(c => c.Length);
var result2 = maxClique!.OrderBy(c => c).ToJoinedString();
Console.WriteLine(result2);

// Bron-Kerbosch's algorithm
List<string[]> FindAllMaximalCliques(HashSet<string> r, HashSet<string> p, HashSet<string> x, IDictionary<string, HashSet<string>> links)
{
    List<string[]> cliques = [];

    if (p.Count == 0 && x.Count == 0)
    {
        cliques.Add([.. r]);
    }

    while (p.Count != 0)
    {
        var v = p.First();

        var neighbors = links[v];
        var newR = r.Union([v]).ToHashSet();
        var newP = p.Intersect(neighbors).ToHashSet();
        var newX = x.Intersect(neighbors).ToHashSet();
        cliques.AddRange(FindAllMaximalCliques(newR, newP, newX, links));

        p.Remove(v);
        x.Add(v);
    }

    return cliques;
}

IDictionary<string, HashSet<string>> ParseInpput(string[] lines)
{
    ConcurrentDictionary<string, HashSet<string>> links = [];

    var connections = lines
        .Select(line => line.Split('-'))
        .Select(parts => (A: parts[0], B: parts[1]));

    foreach (var connection in connections)
    {
        AddLink(connection.A, connection.B);
        AddLink(connection.B, connection.A);
    }

    return links;

    void AddLink(string key, string value) => 
        links.AddOrUpdate(
            key, 
            new HashSet<string> { value }, 
            (k, set) => { set.Add(value); return set; });
}
