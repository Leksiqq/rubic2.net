using Net.Leksi.Rubik2;
using Rubik2Console;
using System.Text;
using System.Text.RegularExpressions;

Options options = Options.Create(args);
Regex reSpaces = new("\\s+");

StringBuilder sb = new();

string? input = options.Reader!.ReadLine();
while(input is { })
{
    sb.Append(reSpaces.Replace(input, string.Empty));
    input = options.Reader.ReadLine();
}
if(options.TargetReader is { })
{
    input = options.TargetReader.ReadLine();
    while (input is { })
    {
        sb.Append(reSpaces.Replace(input, string.Empty));
        input = options.TargetReader.ReadLine();
    }
}
State start = new();
State? target = null;

for(int i = 0; i < Math.Min(sb.Length, 48); i++)
{
    if(i < 24)
    {
        start[i] = DecodeColor(sb, i);
    }
    else
    {
        if(i == 24)
        {
            target = new();
        }
        target![i - 24] = DecodeColor(sb, i);
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
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}

static Color DecodeColor(StringBuilder sb, int i)
{
    return sb[i] switch
    {
        'w' => Color.White,
        'r' => Color.Red,
        'b' => Color.Blue,
        'g' => Color.Green,
        'o' => Color.Orange,
        'y' => Color.Yellow,
        _ => throw new InvalidOperationException($"Unexpected character: '{sb[i]}'")
    };
}

