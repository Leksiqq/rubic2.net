using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Net.Leksi.Rubik2;

public class State : IComparable<State>
{
    private static readonly ulong[] s_clearMasks = [
        ulong.MaxValue ^ 0b111,
        ulong.MaxValue ^ 0b111000,
        ulong.MaxValue ^ 0b111000000,
        ulong.MaxValue ^ 0b111000000000,
        ulong.MaxValue ^ 0b111000000000000,
        ulong.MaxValue ^ 0b111000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000000000000000,
        ulong.MaxValue ^ 0b111000000000000000000000000000000000,
    ];
    #region finished coloring

    private static readonly Color[] s_wo_finished = [
        Color.White,
        Color.Orange,
        Color.Yellow,
        Color.Red,
        Color.Green,
        Color.Blue,
    ];
    private static readonly Color[] s_wb_finished = [
        Color.White,
        Color.Blue,
        Color.Yellow,
        Color.Green,
        Color.Orange,
        Color.Red,
    ];
    private static readonly Color[] s_wr_finished = [
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Orange,
        Color.Blue,
        Color.Green,
    ];
    private static readonly Color[] s_wg_finished = [
        Color.White,
        Color.Green,
        Color.Yellow,
        Color.Blue,
        Color.Red,
        Color.Orange,
    ];
    private static readonly Color[] s_rw_finished = [
        Color.Red,
        Color.White,
        Color.Orange,
        Color.Yellow,
        Color.Green,
        Color.Blue,
    ];
    private static readonly Color[] s_rb_finished = [
        Color.Red,
        Color.Blue,
        Color.Orange,
        Color.Green,
        Color.White,
        Color.Yellow,
    ];
    private static readonly Color[] s_ry_finished = [
        Color.Red,
        Color.Yellow,
        Color.Orange,
        Color.White,
        Color.Blue,
        Color.Green,
    ];
    private static readonly Color[] s_rg_finished = [
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Blue,
        Color.Yellow,
        Color.White,
    ];
    private static readonly Color[] s_yr_finished = [
        Color.Yellow,
        Color.Red,
        Color.White,
        Color.Orange,
        Color.Green,
        Color.Blue,
    ];
    private static readonly Color[] s_yg_finished = [
        Color.Yellow,
        Color.Green,
        Color.White,
        Color.Blue,
        Color.Orange,
        Color.Red,
    ];
    private static readonly Color[] s_yo_finished = [
        Color.Yellow,
        Color.Orange,
        Color.White,
        Color.Red,
        Color.Blue,
        Color.Green,
    ];
    private static readonly Color[] s_yb_finished = [
        Color.Yellow,
        Color.Blue,
        Color.White,
        Color.Green,
        Color.Red,
        Color.Orange,
    ];
    private static readonly Color[] s_by_finished = [
        Color.Blue,
        Color.Yellow,
        Color.Green,
        Color.White,
        Color.Orange,
        Color.Red,
    ];
    private static readonly Color[] s_br_finished = [
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Yellow,
        Color.White,
    ];
    private static readonly Color[] s_bw_finished = [
        Color.Blue,
        Color.White,
        Color.Green,
        Color.Yellow,
        Color.Red,
        Color.Orange,
    ];
    private static readonly Color[] s_bo_finished = [
        Color.Blue,
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.White,
        Color.Yellow,
    ];
    private static readonly Color[] s_ow_finished = [
        Color.Orange,
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Blue,
        Color.Green,
    ];
    private static readonly Color[] s_ob_finished = [
        Color.Orange,
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Yellow,
        Color.White,
    ];
    private static readonly Color[] s_oy_finished = [
        Color.Orange,
        Color.Yellow,
        Color.Red,
        Color.White,
        Color.Green,
        Color.Blue,
    ];
    private static readonly Color[] s_og_finished = [
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.Blue,
        Color.White,
        Color.Yellow,
    ];
    private static readonly Color[] s_gw_finished = [
        Color.Green,
        Color.White,
        Color.Blue,
        Color.Yellow,
        Color.Orange,
        Color.Red,
    ];
    private static readonly Color[] s_go_finished = [
        Color.Green,
        Color.Orange,
        Color.Blue,
        Color.Red,
        Color.Yellow,
        Color.White,
    ];
    private static readonly Color[] s_gy_finished = [
        Color.Green,
        Color.Yellow,
        Color.Blue,
        Color.White,
        Color.Red,
        Color.Orange,
    ];
    private static readonly Color[] s_gr_finished = [
        Color.Green,
        Color.Red,
        Color.Blue,
        Color.Orange,
        Color.White,
        Color.Yellow,
    ];
    #endregion Finished coloring
    #region Possible corners
    private static readonly List<Corner> s_possible_corners = [
        new(Color.White, Color.Orange, Color.Blue),
        new(Color.Yellow, Color.Orange, Color.Blue),
        new(Color.Yellow, Color.Orange, Color.Green),
        new(Color.White, Color.Orange, Color.Green),
        new(Color.White, Color.Red, Color.Blue),
        new(Color.Yellow, Color.Red, Color.Blue),
        new(Color.Yellow, Color.Red, Color.Green),
        new(Color.White, Color.Red, Color.Green),
    ];
    #endregion Possible corners

    private static readonly int[] s_corners_indexes = [0, 1, 2, 3, 6, 7, 10, 11];

    private ulong _hi = 0;
    private ulong _lo = 0;

    public Completeness Completeness
    {
        get
        {
            State th = this;
            Corner[] corners = [.. s_corners_indexes.Select(i => th.GetCorner(i)).OrderBy(c => c)];
            HashSet<Corner> set = new(s_possible_corners);
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
            CheckIndex(index);
            if (index / 12 == 0)
            {
                return (Color)((_lo & (7UL << index * 3)) >> index * 3);
            }
            index %= 12;
            return (Color)((_hi & (7UL << index * 3)) >> index * 3);
        }
        set
        {
            CheckIndex(index);
            if (index / 12 == 0)
            {
                _lo &= s_clearMasks[index];
                _lo |= (((ulong)value) << index * 3);
            }
            index %= 12;
            _hi &= s_clearMasks[index];
            _hi |= (((ulong)value) << index * 3);
        }
    }

    public State() { }
    public int CompareTo(State? other)
    {
        if(other is null)
        {
            return -1;
        }
        return _hi < other._hi || (_hi == other._hi && _lo < other._lo) ? -1 :
            (_hi == other._hi && _lo == other._lo ? 0 : 1);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is State other && _hi == other._hi && _lo == other._lo;
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
    public static bool operator ==(State left, State right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(State left, State right)
    {
        return !(left == right);
    }
    internal State GetTransformed(List<int> transformer)
    {
        State result = new();
        for (int i = 0; i < 24; ++i)
        {
            result[i] = this[transformer[i]];
        }
        return result;
    }
    internal void Assign(State other)
    {
        _lo = other._lo;
        _hi = other._hi;
    }
    internal static State CreateFinished(Color front, Color bottom)
    {
        State result = new();
        Color[] coloring = null!;
        switch (front)
        {
            case Color.White:
                {
                    coloring = bottom switch
                    {
                        Color.Orange => s_wo_finished,
                        Color.Green => s_wg_finished,
                        Color.Red => s_wr_finished,
                        Color.Blue => s_wb_finished,
                        _ => throw new InvalidOperationException(),
                    };
                    break;
                }
            case Color.Blue:
                {
                    coloring = bottom switch
                    {
                        Color.Orange => s_bo_finished,
                        Color.White => s_bw_finished,
                        Color.Red => s_br_finished,
                        Color.Yellow => s_by_finished,
                        _ => throw new InvalidOperationException(),
                    };
                    break;
                }
            case Color.Yellow:
                {
                    coloring = bottom switch
                    {
                        Color.Orange => s_yo_finished,
                        Color.Blue => s_yb_finished,
                        Color.Red => s_yr_finished,
                        Color.Green => s_yg_finished,
                        _ => throw new InvalidOperationException(),
                    };
                    break;
                }
            case Color.Green:
                {
                    coloring = bottom switch
                    {
                        Color.Orange => s_go_finished,
                        Color.Yellow => s_gy_finished,
                        Color.Red => s_gr_finished,
                        Color.White => s_gw_finished,
                        _ => throw new InvalidOperationException(),
                    };
                    break;
                }
            case Color.Orange:
                {
                    coloring = bottom switch
                    {
                        Color.Blue => s_ob_finished,
                        Color.Yellow => s_oy_finished,
                        Color.Green => s_og_finished,
                        Color.White => s_ow_finished,
                        _ => throw new InvalidOperationException(),
                    };
                    break;
                }
            case Color.Red:
                {
                    coloring = bottom switch
                    {
                        Color.Blue => s_rb_finished,
                        Color.Yellow => s_ry_finished,
                        Color.Green => s_rg_finished,
                        Color.White => s_rw_finished,
                        _ => throw new InvalidOperationException(),
                    };
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
        CheckIndex(cell);
        return cell switch
        {
            0 or 14 or 21 => new Corner(this[0], this[14], this[21]),
            1 or 15 or 16 => new Corner(this[1], this[15], this[16]),
            2 or 4 or 23 => new Corner(this[2], this[4], this[23]),
            3 or 5 or 18 => new Corner(this[3], this[5], this[18]),
            6 or 8 or 22 => new Corner(this[6], this[8], this[22]),
            7 or 9 or 19 => new Corner(this[7], this[9], this[19]),
            10 or 12 or 20 => new Corner(this[10], this[12], this[20]),
            _ => new Corner(this[11], this[13], this[17]),
        };
    }
    private void CheckIndex(int index)
    {
        if (index < 0 || index > 23)
        {
            throw new IndexOutOfRangeException(string.Format(Constants.s_colorIndexOutOfRange, index)) { HResult = Constants.s_colorIndexOutOfRangeCode };
        }
    }

}
