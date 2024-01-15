using Net.Leksi.Rubik2;
using Rubik2Console;
using Rubik2Console.Properties;
using System.Text;
using System.Text.RegularExpressions;


Options options = Options.Create(args);

Regex reSpaces = new("\\s+");
Regex reInstruction = new("([^:]+):([<v>])", RegexOptions.IgnoreCase);

StringBuilder sb = new();

State start = new();

string? input = options.Reader!.ReadLine();
while (input is { })
{
    sb.Append(reSpaces.Replace(input, string.Empty));
    input = options.Reader.ReadLine();
}
if(sb.Length != 24)
{
    Console.WriteLine(Resources.InvalidSourceCharsCount, 24, sb.Length);
    Environment.Exit(1);
}
try
{
    for (int i = 0; i < sb.Length; i++)
    {
        start[i] = ParseColor(sb[i]);
    }
}
catch(Exception ex)
{
#if DEBUG
        throw;
#else
    Console.WriteLine(ex.Message);
    Environment.Exit(1);
#endif
}
sb.Clear();
if (options.InstructionReader is null)
{
    State? target = null;
    if (options.TargetReader is { })
    {
        target = new();
        input = options.TargetReader.ReadLine();
        while (input is { })
        {
            sb.Append(reSpaces.Replace(input, string.Empty));
            input = options.TargetReader.ReadLine();
        }
        if (sb.Length != 24)
        {
            Console.WriteLine(Resources.InvalidTargetCharsCount, 24, sb.Length);
            Environment.Exit(1);
        }
        for (int i = 0; i < sb.Length; i++)
        {
            target[i] = ParseColor(sb[i]);
        }
    }

    try
    {
        Tuple<List<Move>, State> solvation = Calculator.Solve(start, target);
        Console.Write(start);
        if (target is { })
        {
            Console.Write($" -> {target}");
        }
        Console.WriteLine();
        Console.WriteLine();
        foreach (var move in solvation.Item1)
        {
            if (options.ShowIntermediateStates)
            {
                Calculator.Move(start, move);
                Console.Write($"{{0,-28}}-> {start}", $"{move.Face}: {move.Spin}");
            }
            else
            {
                Console.Write($"{move.Face}: {move.Spin}");
            }

            Console.WriteLine();
        }
        if (solvation.Item1.Count > 0)
        {
            Console.WriteLine();
        }
        Console.WriteLine(solvation.Item2);
    }
    catch (Exception ex)
    {
        string message = ex.HResult switch
        {
            1 => string.Format(Resources.ColorIndexOutOfRange, ex.Data["index"]),
            2 => Resources.SourceIncomplete,
            3 => Resources.SourceWrong,
            4 => Resources.TargetUnreachable,
            5 => Resources.TargetIncomplete,
            6 => Resources.TargetWrong,
            7 => Resources.StateIncomplete,
            8 => Resources.StateWrong,
            _ => throw new NotImplementedException()

        };
#if DEBUG
        throw new Exception(message, ex);
#else
        Console.WriteLine(message);
        Environment.Exit(1);
#endif
    }
}
else
{
    List<string> instructions = [];
    input = options.InstructionReader.ReadLine();
    while (input is { })
    {
        instructions.AddRange(reSpaces.Split(input));
        input = options.InstructionReader.ReadLine();
    }
    try
    {
        Console.WriteLine(start);
        Console.WriteLine();
        foreach (string instruction in instructions)
        {
            Move move = ParseInstruction(instruction);
            Calculator.Move(start, move);
            if (options.ShowIntermediateStates)
            {
                Console.WriteLine($"{{0,-28}}-> {start}", $"{move.Face}: {move.Spin}");
            }
        }
        if (options.ShowIntermediateStates) 
        { 
            Console.WriteLine();
        }
        Console.WriteLine(start);
    }
    catch (Exception ex)
    {
#if DEBUG
        throw;
#else
        Console.WriteLine(ex.Message);
        Environment.Exit(1);
#endif
    }
}

Move ParseInstruction(string instruction)
{
    Match m = reInstruction.Match(instruction);
    if (!m.Success)
    {
        throw new InvalidOperationException(string.Format(Resources.UnexpectedInstruction, instruction));
    }
    Face face;
    if ("f".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Front;
    }
    else if ("bo".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Bottom;
    }
    else if ("ba".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Back;
    }
    else if ("t".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Top;
    }
    else if ("r".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Right;
    }
    else if ("l".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.Left;
    }
    else if ("fb".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.FrontBack;
    }
    else if ("rl".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.RightLeft;
    }
    else if ("tb".Equals(m.Groups[1].Value.ToLower()))
    {
        face = Face.TopBottom;
    }
    else
    {
        throw new InvalidOperationException(string.Format(Resources.UnexpectedInstruction, instruction));
    }
    Spin spin = m.Groups[2].Value.ToLower()[0] switch
    {
        '>' => Spin.ClockWise,
        '<' => Spin.CounterClockWise,
        'v' => Spin.HalfTurn,
        _ => throw new NotImplementedException()
    };
    return new Move(face, spin);
}

static Color ParseColor(char c)
{
    return c switch
    {
        'w' or 'W' => Color.White,
        'r' or 'R' => Color.Red,
        'b' or 'B' => Color.Blue,
        'g' or 'G' => Color.Green,
        'o' or 'O' => Color.Orange,
        'y' or 'Y' => Color.Yellow,
        _ => throw new InvalidOperationException(string.Format(Resources.UnexpectedColorChar, c))
    };
}

