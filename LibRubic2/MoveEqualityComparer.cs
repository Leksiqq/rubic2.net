using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Rubic2;

public class MoveEqualityComparer : IEqualityComparer<Move>
{
    public bool Equals(Move x, Move y)
    {
        return x.Face == y.Face && x.Spin == y.Spin;
    }

    public int GetHashCode([DisallowNull] Move obj)
    {
        return 2 * (int)obj.Face + (int)obj.Spin;
    }
}
