using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var codes = lines.Select(long.Parse).ToArray();

IEnumerable<long> nextCodes = codes.ToArray();
for (int i = 0; i < 2000; i++)
{
    nextCodes = nextCodes.Select(Next);
}
var result1 = nextCodes.Sum();
Console.WriteLine(result1);

var histories = codes.ToDictionary(c => c, c => new Queue<long>([c]));
var sequenceProfits = new Dictionary<string, Dictionary<long, long>>();
for (int i = 0; i < 2000; i++)
{
    foreach (var code in codes)
    {
        var hist = histories[code];
        var price = Next(hist.Last());
        hist.Enqueue(price);
        
        if (hist.Count > 5) hist.Dequeue();
        
        if (hist.Count == 5)
        {
            var seq = string.Join(",", hist.Select(x => x % 10).Pairwise().Select(pair => pair.B - pair.A));
            if (!sequenceProfits.ContainsKey(seq))
            {
                sequenceProfits.Add(seq, new());
            }

            sequenceProfits[seq].TryAdd(code, price % 10);
        }
    }
}
var result2 = sequenceProfits.Values.Max(kvp => kvp.Values.Sum());
Console.WriteLine(result2);

long Next(long n)
{
    var a = ((n * 64) ^ n) % 16777216;
    var b = ((a / 32) ^ a) % 16777216;
    var c = ((b * 2048) ^ b) % 16777216;
    return c;
}
