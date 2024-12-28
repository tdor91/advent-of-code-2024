
const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var (keys, locks) = ParseInput(lines);

var result1 = 0;
foreach (var @lock in locks)
{
    foreach (var key in keys)
    {
        var fit = key.Pattern.Zip(@lock.Pattern).All(z => z.First + z.Second <= 5);
        if (fit)
        {
            result1++;
        }
    }
}
Console.WriteLine(result1);

(List<Key> keys, List<Lock> locks) ParseInput(string[] lines)
{
    List<Key> keys = [];
    List<Lock> locks = [];

    int i = 0;
    while (i < lines.Length)
    {
        var firstLine = lines[i++];
        int[] pattern = new int[firstLine.Length];

        for (int j = 0; j < 5; j++, i++)
        {
            var line = lines[i];
            for (int k = 0; k < line.Length; k++)
            {
                pattern[k] += line[k] == '#' ? 1 : 0;
            }
        }

        if (firstLine == "#####") locks.Add(new Lock(pattern));
        else keys.Add(new Key(pattern));

        i += 2;
    }

    return (keys, locks);
}

record Key(int[] Pattern);
record Lock(int[] Pattern);