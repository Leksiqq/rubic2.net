using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Rubic2;

public readonly struct Move(Face face, Spin spin)
{
    public readonly Face Face = face;
    public readonly Spin Spin = spin;

    public override int GetHashCode()
    {
        return HashCode.Combine(Face, Spin);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Move move && move.Face == Face && move.Spin == Spin;
    }

    public override string ToString()
    {
        return $"{Face}:{Spin}";
    }

    public static bool operator ==(Move left, Move right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Move left, Move right)
    {
        return !(left == right);
    }
}
