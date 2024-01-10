namespace Net.Leksi.Rubic2;

public struct Move(Face face, Spin spin)
{
    public readonly Face Face = face;
    public readonly Spin Spin = spin;
}
