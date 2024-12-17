const string inputFile = @"input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

var computer = new Computer(lines);
var result1 = computer.Run();
Console.WriteLine(result1);

// sadly I was not able to solve this without a few hints :(
var result2 = FindInputForOutput(computer, computer.Program);
Console.WriteLine(result2);

Console.WriteLine("done");

// the instructions + operand of the example are
// 2,4 <-- B = A % 8
// 1,5 <-- B = B ^ 5 -- 0x101
// 7,5 <-- C = A / 2^A
// 1,6 <-- B = B ^ 6 -- 0x110
// 4,1 <-- B = B ^ C
// 5,5 <-- out B % 8
// 0,3 <-- A = A / 2^A
// 3,0 <-- restart if A != 0
//
// Notes and observations:
// * ending condition is when A is 0
// * when representing the values in octal system (because it's a 3-bit system),
//   you can see that A = A / (2^A) is actually just cutting off the last digit
// ** since we cut off a digit every iteration, the solution must be between 8^15 and 8^16 (16 numbers in solution)
long FindInputForOutput(Computer computer, int[] program)
{
    // start with a = 0 for the first position
    var options = new Queue<(long A, int Pos)>();
    options.Enqueue((0, 1));

    // do while we have options; if there is a solution, an early return in the loop happens
    while (options.Any())
    {
        var (a, pos) = options.Dequeue();

        // it's not always the first possibility -> there are options that lead to a dead end
        // -> just test all possibilities before another digit changes (=8)
        for (var option = a; option < a + 8; option++)
        {
            // split the program -> we only take care about the last 'n' positions of the sequence
            var remaining = program[^pos..];

            // test the option by running the computer
            var current = computer.RestartWith(option);

            // ignore the attempts if the option yields a different sequence
            if (remaining.SequenceEqual(current))
            {
                // if the option resulted in the complete sequence, a solution is found
                if (pos == program.Length)
                {
                    return option;
                }

                // add one digit at the end by multiplying by its base
                options.Enqueue((option * 8, pos + 1));
            }
        }
    }

    throw new Exception("invalid program");
}

class Computer
{
    private readonly string[] program;
    private readonly Dictionary<int, Action<long>> instructions;

    public Computer(string[] program)
    {
        this.program = program;
        instructions = new()
        {
            [0] = Adv, [1] = Bxl, [2] = Bst, [3] = Jnz, [4] = Bxc, [5] = Out, [6] = Bdv, [7] = Cdv,
        };

        Init();
    }

    public long A { get; private set; }
    public long B { get; private set; }
    public long C { get; private set; }
    public long InstructionPointer { get; private set; } = 0;

    public int[] Program { get; private set; }
    public List<int> Output { get; } = [];

    public string Run()
    {
        while (InstructionPointer < Program.Length - 1)
        {
            var opcode = Program[InstructionPointer++];
            var operand = Program[InstructionPointer++];
            var parameter = MapOperand(opcode, operand);
            instructions[opcode](parameter);
        }

        return string.Join(',', Output);
    }

    public List<int> RestartWith(long a)
    {
        Init();
        A = a;
        _ = Run();
        return Output;
    }

    private void Init()
    {
        A = long.Parse(program[0].Split(' ').Last());
        B = long.Parse(program[1].Split(' ').Last());
        C = long.Parse(program[2].Split(' ').Last());
        Program = program[4].Split(' ').Last().Split(',').Select(int.Parse).ToArray();

        InstructionPointer = 0;
        Output.Clear();
    }

    private long MapOperand(int opcode, int operand)
    {
        bool isLiteral = opcode is 1 or 3;
        if (isLiteral) return operand;

        return operand switch
        {
            0 => 0, 1 => 1, 2 => 2, 3 => 3,
            4 => A, 5 => B, 6 => C,
            _ => throw new Exception("invalid operand"),
        };
    }

    private void Adv(long value) => A = (long)(A / Math.Pow(2, value));
    private void Bxl(long value) => B = B ^ value;
    private void Bst(long value) => B = value % 8;

    private void Jnz(long value)
    {
        if (A == 0) return;
        InstructionPointer = value;
    }

    private void Bxc(long _) => B = B ^ C;
    private void Out(long value) => Output.Add((int)(value % 8));
    private void Bdv(long value) => B = (long)(A / Math.Pow(2, value));
    private void Cdv(long value) => C = (long)(A / Math.Pow(2, value));
}