const string inputFile = @"input.txt";

var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

var currentStones = input.ToList();
for (int i = 0; i < 25; i++)
{
    currentStones = BlinkAll(currentStones);
}
var result1 = currentStones.Count;
Console.WriteLine(result1);

Dictionary<long, long> countMap = input.ToDictionary(x => x, x => 1L);
for (int i = 0; i < 75; i++)
{
    var existingStones = countMap.Where(kvp => kvp.Value > 0).Select(kvp => (Number: kvp.Key, Count: kvp.Value)).ToArray();
    foreach (var (number, count) in existingStones)
    {
        var nextStones = Blink(number);
        foreach (var stone in nextStones)
        {
            countMap[stone] = countMap.GetValueOrDefault(stone) + count;
        }
        countMap[number] -= count;
    }
}
long result2 = countMap.Values.Sum();
Console.WriteLine(result2);

List<long> BlinkAll(IEnumerable<long> stones)
{
    List<long> result = [];

    foreach (long stone in stones)
    {
        result.AddRange(Blink(stone));
    }

    return result;
}

long[] Blink(long stone)
{
    if (stone == 0) return [1];

    var numberStr = stone.ToString();
    if (numberStr.Length % 2 == 0)
    {
        var middle = numberStr.Length / 2;
        var left = long.Parse(numberStr.Substring(0, middle));
        var right = long.Parse(numberStr.Substring(middle));
        return [left, right];
    }

    return [stone * 2024];
}
