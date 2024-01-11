using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Rubic2;

public readonly struct Corner(params Color[] colors) : IComparable<Corner>
{
    private readonly Color[] _colors = [.. colors.OrderBy(c => c)];

    public override int GetHashCode()
    {
        return HashCode.Combine(_colors[0], _colors[1], _colors[2]);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Corner corner && Enumerable.Zip(_colors, corner._colors).All(v => v.First == v.Second);
    }
    public override string ToString()
    {
        return $"{_colors[0]},{_colors[1]},{_colors[2]}";
    }

    public int CompareTo(Corner other)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (_colors[i] != other._colors[i])
            {
                return _colors[i] < other._colors[i] ? -1 : 1;
            }
        }
        return 0;
    }
}
