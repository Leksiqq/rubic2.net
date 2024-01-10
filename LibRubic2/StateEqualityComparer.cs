using System.Diagnostics.CodeAnalysis;

namespace Net.Leksi.Rubic2;

internal class StateEqualityComparer : IEqualityComparer<State>
{
    public bool Equals(State x, State y)
    {
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] State obj)
    {
        return obj.GetHashCode();
    }
}
