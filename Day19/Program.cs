const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var definedPatterns = lines[0].Split([',', ' '], StringSplitOptions.RemoveEmptyEntries).ToArray();
var desiredPatterns = lines.Skip(2).ToArray();

var patternPossibilities = BuildPossibilities(desiredPatterns, definedPatterns);
var desiredPatternPossibilities = patternPossibilities.Where(kvp => desiredPatterns.Contains(kvp.Key)).ToArray();

var result1 = desiredPatternPossibilities.Count(kvp => kvp.Value > 0);
Console.WriteLine(result1);
var result2 = desiredPatternPossibilities.Sum(kvp => kvp.Value);
Console.WriteLine(result2);

Console.WriteLine("done");

Dictionary<string, long> BuildPossibilities(string[] desiredPatterns, string[] definedPatterns)
{
    Dictionary<string, long> patternPossibilities = [];

    foreach (var pattern in desiredPatterns)
    {
        BuildPossibilitiesRec(pattern, definedPatterns, patternPossibilities);
    }

    return patternPossibilities;

    long BuildPossibilitiesRec(string desired, string[] definedPatterns, Dictionary<string, long> foundPatternPossibilities)
    {
        if (foundPatternPossibilities.ContainsKey(desired))
        {
            return foundPatternPossibilities[desired];
        }

        if (desired == string.Empty)
        {
            return 1;
        }

        long possibilities = 0;
        foreach (var pattern in definedPatterns)
        {
            if (desired.StartsWith(pattern))
            {
                possibilities += BuildPossibilitiesRec(desired[pattern.Length..], definedPatterns, foundPatternPossibilities);
            }
        }

        foundPatternPossibilities[desired] = possibilities;
        return possibilities;
    }
}
