using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Rubic2;

internal struct Corner: IComparable<Corner>
{
    internal class EqualityComparer : IEqualityComparer<Corner>
    {
        public bool Equals(Corner x, Corner y)
        {
            Console.WriteLine($"{x} ? {y}");
            return Enumerable.Zip(x._colors, y._colors).All(v => v.First == v.Second);
        }

        public int GetHashCode([DisallowNull] Corner obj)
        {
            Console.WriteLine($"hc: {obj} = {obj.GetHashCode()}");
            return obj.GetHashCode();
        }
    }


    private readonly Color[] _colors;

    public Corner(params Color[] colors)
    {
        _colors = colors.OrderBy(c => c).ToArray();
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(_colors);
    }
    public override string ToString()
    {
        return $"{_colors[0]},{_colors[1]},{_colors[2]}";
    }

    public int CompareTo(Corner other)
    {
        for(int i = 0; i < 3; ++i)
        {
            if (_colors[i] != other._colors[i])
            {
                return _colors[i] < other._colors[i] ? -1 : 1;
            }
        }
        return 0;
    }
}
