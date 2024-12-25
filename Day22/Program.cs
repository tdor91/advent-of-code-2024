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

Console.WriteLine("done");

long Next(long n)
{
    var a = ((n * 64) ^ n) % 16777216;
    var b = ((a / 32) ^ a) % 16777216;
    var c = ((b * 2048) ^ b) % 16777216;
    return c;
}