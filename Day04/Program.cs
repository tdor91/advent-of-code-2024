using Common;

Point[] allDirections =
[
    new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(-1, 1),
    new Point(-1, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1),
];

Point[] diagonalDirections =
[
    new Point(1, 1), new Point(-1, 1), new Point(-1, -1), new Point(1, -1),
];

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var input = lines.Select(line => line.ToCharArray()).ToArray();

var result1 = input.Points()
    .Where(point => input.GetSymbol(point) == 'X')
    .SelectMany(start => WordsFromPoint(input, start, allDirections, wordLen: 4))
    .Count(word => word == "XMAS");
Console.WriteLine(result1);

var result2 = input.Points()
    .Where(point => input.GetSymbol(point) == 'M')
    .SelectMany(start => EndpointsOfWord(input, start, diagonalDirections, "MAS"))
    .GroupBy(endpoints => endpoints.Start.CenterTo(endpoints.End))
    .Count(group => group.Count() > 1);
Console.WriteLine(result2);

Console.WriteLine("done");

IEnumerable<string> WordsFromPoint(char[][] matrix, Point start, Point[] directions, int wordLen)
{
    foreach (var direction in directions)
    {
        var end = start + direction * (wordLen - 1);
        if (!matrix.Contains(end)) continue;
        
        string word = "";
        for (int i = 0; i < wordLen; i++)
        {
            word += matrix.GetSymbol(start + direction * i);
        }
        yield return word;
    }
}

IEnumerable<(Point Start, Point End)> EndpointsOfWord(char[][] matrix, Point start, Point[] directions, string word)
{
    foreach (var direction in directions)
    {
        var end = start + direction * (word.Length - 1);
        if (!matrix.Contains(end)) continue;
        
        string w = "";
        for (int i = 0; i < word.Length; i++)
        {
            w += matrix.GetSymbol(start + direction * i);
        }

        if (w == word)
        {
            yield return (start, end);
        }
    }
}