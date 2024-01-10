using System;
using System.Text;

namespace Net.Leksi.Rubic2;

public struct State : IComparable<State>, IEquatable<State>
{
    private static UInt64[] s_clearMasks = [
        UInt64.MaxValue ^ 0b111,
        UInt64.MaxValue ^ 0b111000,
        UInt64.MaxValue ^ 0b111000000,
        UInt64.MaxValue ^ 0b111000000000,
        UInt64.MaxValue ^ 0b111000000000000,
        UInt64.MaxValue ^ 0b111000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000000000000000,
        UInt64.MaxValue ^ 0b111000000000000000000000000000000000,
    ];
    #region finished coloring

    private static Color[] s_wo_finished = [
        Color.White,
        Color.Orange,
        Color.Yellow,
        Color.Red,
        Color.Green,
        Color.Blue,
    ];
    private static Color[] s_wb_finished = [
        Color.White,
        Color.Blue,
        Color.Yellow,
        Color.Green,
        Color.Orange,
        Color.Red,
    ];
    private static Color[] s_wr_finished = [
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Orange,
        Color.Blue,
        Color.Green,
    ];
    private static Color[] s_wg_finished = [
        Color.White,
        Color.Green,
        Color.Yellow,
        Color.Blue,
        Color.Red,
        Color.Orange,
    ];
    private static Color[] s_rw_finished = [
        Color.Red,
        Color.White,
        Color.Orange,
        Color.Yellow,
        Color.Green,
        Color.Blue,
    ];
    private static Color[] s_rb_finished = [
        Color.Red,
        Color.Blue,
        Color.Orange,
        Color.Green,
        Color.White,
        Color.Yellow,
    ];
    private static Color[] s_ry_finished = [
        Color.Red,
        Color.Yellow,
        Color.Orange,
        Color.White,
        Color.Blue,
        Color.Green,
    ];
    private static Color[] s_rg_finished = [
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Blue,
        Color.Yellow,
        Color.White,
    ];
    private static Color[] s_yr_finished = [
        Color.Yellow,
        Color.Red,
        Color.White,
        Color.Orange,
        Color.Green,
        Color.Blue,
    ];
    private static Color[] s_yg_finished = [
        Color.Yellow,
        Color.Green,
        Color.White,
        Color.Blue,
        Color.Orange,
        Color.Red,
    ];
    private static Color[] s_yo_finished = [
        Color.Yellow,
        Color.Orange,
        Color.White,
        Color.Red,
        Color.Blue,
        Color.Green,
    ];
    private static Color[] s_yb_finished = [
        Color.Yellow,
        Color.Blue,
        Color.White,
        Color.Green,
        Color.Red,
        Color.Orange,
    ];
    private static Color[] s_by_finished = [
        Color.Blue,
        Color.Yellow,
        Color.Green,
        Color.White,
        Color.Orange,
        Color.Red,
    ];
    private static Color[] s_br_finished = [
        Color.Blue,
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.White,
        Color.Yellow,
    ];
    private static Color[] s_bw_finished = [
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Yellow,
        Color.White,
    ];
    private static Color[] s_bo_finished = [
        Color.Blue,
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.White,
        Color.Yellow,
    ];
    private static Color[] s_ow_finished = [
        Color.Orange,
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Blue,
        Color.Green,
    ];
    private static Color[] s_ob_finished = [
        Color.Orange,
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Yellow,
        Color.White,
    ];
    private static Color[] s_oy_finished = [
        Color.Orange,
        Color.Yellow,
        Color.Red,
        Color.White,
        Color.Green,
        Color.Blue,
    ];
    private static Color[] s_og_finished = [
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.Blue,
        Color.White,
        Color.Yellow,
    ];
    private static Color[] s_gw_finished = [
        Color.Green,
        Color.White,
        Color.Blue,
        Color.Yellow,
        Color.Orange,
        Color.Red,
    ];
    private static Color[] s_go_finished = [
        Color.Green,
        Color.Orange,
        Color.Blue,
        Color.Red,
        Color.Yellow,
        Color.White,
    ];
    private static Color[] s_gy_finished = [
        Color.Green,
        Color.Yellow,
        Color.Blue,
        Color.White,
        Color.Red,
        Color.Orange,
    ];
    private static Color[] s_gr_finished = [
        Color.Green,
        Color.Red,
        Color.Blue,
        Color.Orange,
        Color.White,
        Color.Yellow,
    ];
    #endregion Finished coloring
    #region Corners
    private static List<Corner> s_possible_corners = [
        new(Color.White, Color.Orange, Color.Blue),
        new(Color.Yellow, Color.Orange, Color.Blue),
        new(Color.Yellow, Color.Orange, Color.Green),
        new(Color.White, Color.Orange, Color.Green),
        new(Color.White, Color.Red, Color.Blue),
        new(Color.Yellow, Color.Red, Color.Blue),
        new(Color.Yellow, Color.Red, Color.Green),
        new(Color.White, Color.Red, Color.Green),
    ];
    #endregion Corners

    private static int[] s_corners_indexes = new int[] { 0, 1, 2, 3, 6, 7, 10, 11 };

    private UInt64 _hi = 0;
    private UInt64 _lo = 0;
    private static readonly IEqualityComparer<Corner> _cec = new Corner.EqualityComparer();

    public Completeness Completeness
    {
        get
        {
            State th = this;
            Corner[] corners = s_corners_indexes.Select(i => th.GetCorner(i)).OrderBy(c => c).ToArray();
            HashSet<Corner> set = new(s_possible_corners, _cec);
            for(int i = corners.Length - 1; i >= 0; --i)
            {
                if (set.Contains(corners[i]))
                {
                    set.Remove(corners[i]);
                }
                else
                {
                    return Completeness.Wrong;
                }
            }
            return set.Count > 0 ? Completeness.Incomplete : Completeness.Right;
        }
    }
    public Color this[int index]
    {
        get
        {
            if (index < 0 || index > 23)
            {
                throw new IndexOutOfRangeException();
            }
            if (index / 12 == 0)
            {
                return (Color)((_lo & (((UInt64)7) << index * 3)) >> index * 3);
            }
            index %= 12;
            return (Color)((_hi & (((UInt64)7) << index * 3)) >> index * 3);
        }
        set
        {
            if (index < 0 || index > 23)
            {
                throw new IndexOutOfRangeException();
            }
            if (index / 12 == 0)
            {
                _lo &= s_clearMasks[index];
                _lo |= (((UInt64)value) << index * 3);
            }
            index %= 12;
            _hi &= s_clearMasks[index];
            _hi |= (((UInt64)value) << index * 3);
        }
    }
    public State()
    {
    }
    public int CompareTo(State other)
    {
        return _hi < other._hi || (_hi == other._hi && _lo < other._lo) ? -1 :
            (_hi == other._hi && _lo == other._lo ? 0 : 1);
    }

    public bool Equals(State other)
    {
        return _hi == other._hi && _lo == other._lo;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(_hi, _lo);
    }
    public override string ToString()
    {
        StringBuilder sb = new();
        for(int i = 0; i < 24; ++i)
        {
            sb.Append(this[i] switch
            {
                Color.White => 'w',
                Color.Blue => 'b',
                Color.Yellow => 'y',
                Color.Red => 'r',
                Color.Green => 'g',
                Color.Orange => 'o',
                _ => '\0'
            });
            if(i % 4 == 3)
            {
                sb.Append(' ');
            }
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    internal State GetTransformed(List<int> transformer)
    {
        State result = new();
        for (int i = 0; i < 24; ++i)
        {
            result[transformer[i]] = this[i];
        }
        return result;
    }
    internal static State CreateFinished(Color front, Color bottom)
    {
        State result = new();
        Color[] coloring = null!;
        switch (front)
        {
            case Color.White:
                {
                    switch (bottom)
                    {
                        case Color.Orange:
                            coloring = s_wo_finished;
                            break;
                        case Color.Green:
                            coloring = s_wg_finished;
                            break;
                        case Color.Red:
                            coloring = s_wr_finished;
                            break;
                        case Color.Blue:
                            coloring = s_wb_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
            case Color.Blue:
                {
                    switch (bottom)
                    {
                        case Color.Orange:
                            coloring = s_bo_finished;
                            break;
                        case Color.White:
                            coloring = s_bw_finished;
                            break;
                        case Color.Red:
                            coloring = s_br_finished;
                            break;
                        case Color.Yellow:
                            coloring = s_by_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
            case Color.Yellow:
                {
                    switch (bottom)
                    {
                        case Color.Orange:
                            coloring = s_yo_finished;
                            break;
                        case Color.Blue:
                            coloring = s_yb_finished;
                            break;
                        case Color.Red:
                            coloring = s_yr_finished;
                            break;
                        case Color.Green:
                            coloring = s_yg_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
            case Color.Green:
                {
                    switch (bottom)
                    {
                        case Color.Orange:
                            coloring = s_go_finished;
                            break;
                        case Color.Yellow:
                            coloring = s_gy_finished;
                            break;
                        case Color.Red:
                            coloring = s_gr_finished;
                            break;
                        case Color.White:
                            coloring = s_gw_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
            case Color.Orange:
                {
                    switch (bottom)
                    {
                        case Color.Blue:
                            coloring = s_ob_finished;
                            break;
                        case Color.Yellow:
                            coloring = s_oy_finished;
                            break;
                        case Color.Green:
                            coloring = s_og_finished;
                            break;
                        case Color.White:
                            coloring = s_ow_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
            case Color.Red:
                {
                    switch (bottom)
                    {
                        case Color.Blue:
                            coloring = s_rb_finished;
                            break;
                        case Color.Yellow:
                            coloring = s_ry_finished;
                            break;
                        case Color.Green:
                            coloring = s_rg_finished;
                            break;
                        case Color.White:
                            coloring = s_rw_finished;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    break;
                }
        }
        int pos = 0;
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < 4; ++j)
            {
                result[pos] = coloring[i];
                ++pos;
            }
        }
        return result;
    }
    private Corner GetCorner(int cell)
    {
        if (cell < 0 || cell > 23)
        {
            throw new IndexOutOfRangeException();
        }
        switch (cell)
        {
            case 0 or 14 or 21:
                return new Corner(this[0], this[14], this[21]);
            case 1 or 15 or 16:
                return new Corner(this[1], this[15], this[16]);
            case 2 or 4 or 23:
                return new Corner(this[2], this[4], this[23]);
            case 3 or 5 or 18:
                return new Corner(this[3], this[5], this[18]);
            case 6 or 8 or 22:
                return new Corner(this[6], this[8], this[22]);
            case 7 or 9 or 19:
                return new Corner(this[7], this[9], this[19]);
            case 10 or 12 or 20:
                return new Corner(this[10], this[12], this[20]);
            default:
                return new Corner(this[11], this[13], this[17]);
        }
    }
}
