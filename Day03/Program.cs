using System.Text.RegularExpressions;

const string inputFile = @"input.txt";
var input = await File.ReadAllTextAsync(inputFile);

var mulRegex = new Regex(@"mul\((?<num1>\d{1,3}),(?<num2>\d{1,3})\)");
var matches = mulRegex.Matches(input);

var result1 = matches
    .Select(match => ExtractNumber(match, "num1") * ExtractNumber(match, "num2"))
    .Sum();
Console.WriteLine(result1);

var result2 = 0;

var instructions = GetInstructions(input)
    .OrderByDescending(instruction => instruction.Index)
    .ToList();

foreach (Match match in matches)
{
    var lastInstruction = instructions.First(instruction => instruction.Index < match.Index);
    if (lastInstruction.Enabled)
    {
        result2 += ExtractNumber(match, "num1") * ExtractNumber(match, "num2");
    }
}
Console.WriteLine(result2);

int ExtractNumber(Match match, string name) => int.Parse(match.Groups[name].Value);

IEnumerable<(int Index, bool Enabled)> GetInstructions(string memory)
{
    const string doInstruction = "do()";
    const string dontInstruction = "don't()";

    yield return (-1, true);
    
    for (int i = 0; i < memory.Length; i++)
    {
        if (memory[i..].StartsWith(doInstruction))
        {
            yield return (i, true);
        }
        else if (memory[i..].StartsWith(dontInstruction))
        {
            yield return (i, false);
        }
    }
}

// alternative with regex
IEnumerable<(int Index, bool Enabled)> GetInstructionsByRegex(string memory)
{
    var doInstruction = new Regex(@"do\(\)");
    var dontInstruction = new Regex(@"don't\(\)");

    yield return (-1, true);
    
    foreach (Match match in doInstruction.Matches(memory))
    {
        yield return (match.Index + 1, true);
    }
    
    foreach (Match match in dontInstruction.Matches(memory))
    {
        yield return (match.Index + 1, false);
    }
}