using Common;

const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var input = lines
    .Select(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries))
    .Select(parts => (Result: long.Parse(parts[0]), Numbers: parts[1..].ToInts().ToArray()))
    .ToArray();

var result1 = input
    .Where(calc => IsValid1(calc.Result, calc.Numbers[0], calc.Numbers[1..]))
    .Sum(calc => calc.Result);
Console.WriteLine(result1);

var result2 = input
    .Where(calc => IsValid2(calc.Result, calc.Numbers[0], calc.Numbers[1..]))
    .Sum(calc => calc.Result);
Console.WriteLine(result2);

Console.WriteLine("done");

bool IsValid1(long result, long intermediate, int[] numbers)
{
    if (intermediate > result)
    {
        return false;
    }
    
    if (numbers.Length == 0)
    {
        return result == intermediate;
    }
    
    return IsValid1(result, intermediate * numbers[0], numbers[1..]) ||
           IsValid1(result, intermediate + numbers[0], numbers[1..]);
}

bool IsValid2(long result, long intermediate, int[] numbers)
{
    if (intermediate > result)
    {
        return false;
    }
    
    if (numbers.Length == 0)
    {
        return result == intermediate;
    }

    return IsValid2(result, intermediate * numbers[0], numbers[1..]) ||
           IsValid2(result, intermediate + numbers[0], numbers[1..]) ||
           IsValid2(result, long.Parse($"{intermediate}{numbers[0]}"), numbers[1..]);
}