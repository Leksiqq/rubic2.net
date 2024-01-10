using Net.Leksi.Rubic2;
using System.Text;
using System.Text.RegularExpressions;

StringBuilder sb = new();

string? input = Console.ReadLine();
while(!string.IsNullOrEmpty(input))
{
    sb.Append(Regex.Replace(input, "\\s+", string.Empty));
    input = Console.ReadLine();
}
State start = new();
Console.WriteLine(sb);
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
foreach (var move in solvation.Item1)
{
    Console.WriteLine($"{move.Face}: {move.Spin}");
}
Console.WriteLine(solvation.Item2);
