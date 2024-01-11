using Net.Leksi.Rubic2;
using Rubic2Console;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

Options options = Options.Create(args);

StringBuilder sb = new();

string? input = options.Reader!.ReadLine();
while(input is { })
{
    sb.Append(RemoveSpaces().Replace(input, string.Empty));
    input = options.Reader!.ReadLine();
}
State start = new();

for(int i = 0; i < Math.Min(sb.Length, 24); i++)
{
    start[i] = sb[i] switch
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
Tuple<List<Move>, State> solvation = Solver.Solve(start);
Console.WriteLine(start);
Console.WriteLine();
foreach (var move in solvation.Item1)
{
    Console.WriteLine($"{move.Face}: {move.Spin}");
}
if(solvation.Item1.Count > 0)
{
    Console.WriteLine();
}
Console.WriteLine(solvation.Item2);

partial class Program
{
    [GeneratedRegex("\\s+")]
    private static partial Regex RemoveSpaces();
}