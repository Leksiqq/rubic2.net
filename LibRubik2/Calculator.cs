using System.Formats.Asn1;

namespace Net.Leksi.Rubik2;

public static class Calculator
{
    private static readonly Dictionary<Move, List<int>> s_transforms = new()
    {
        { new(Face.Front,  Spin.ClockWise),           [ 2, 0, 3, 1,18,16, 6, 7, 8, 9,10,11,12,13,23,21,14,17,15,19,20, 4,22, 5 ] },
        { new(Face.Front,  Spin.CounterClockWise),    [ 1, 3, 0, 2,21,23, 6, 7, 8, 9,10,11,12,13,16,18, 5,17, 4,19,20,15,22,14 ] },
        { new(Face.Front,  Spin.HalfTurn),            [ 3, 2, 1, 0,15,14, 6, 7, 8, 9,10,11,12,13, 5, 4,23,17,21,19,20,18,22,16 ] },
        { new(Face.Back,   Spin.ClockWise),           [ 0, 1, 2, 3, 4, 5,20,22,10, 8,11, 9,17,19,14,15,16, 7,18, 6,13,21,12,23 ] },
        { new(Face.Back,   Spin.CounterClockWise),    [ 0, 1, 2, 3, 4, 5,19,17, 9,11, 8,10,22,20,14,15,16,12,18,13, 6,21, 7,23 ] },
        { new(Face.Back,   Spin.HalfTurn),            [ 0, 1, 2, 3, 4, 5,13,12,11,10, 9, 8, 7, 6,14,15,16,22,18,20,19,21,17,23 ] },
        { new(Face.Right,  Spin.ClockWise),           [ 0, 5, 2, 7, 4, 9, 6,11, 8,13,10,15,12, 1,14, 3,18,16,19,17,20,21,22,23 ] },
        { new(Face.Right,  Spin.CounterClockWise),    [ 0,13, 2,15, 4, 1, 6, 3, 8, 5,10, 7,12, 9,14,11,17,19,16,18,20,21,22,23 ] },
        { new(Face.Right,  Spin.HalfTurn),            [ 0, 9, 2,11, 4,13, 6,15, 8, 1,10, 3,12, 5,14, 7,19,18,17,16,20,21,22,23 ] },
        { new(Face.Left,   Spin.ClockWise),           [12, 1,14, 3, 0, 5, 2, 7, 4, 9, 6,11, 8,13,10,15,16,17,18,19,22,20,23,21 ] },
        { new(Face.Left,   Spin.CounterClockWise),    [ 4, 1, 6, 3, 8, 5,10, 7,12, 9,14,11, 0,13, 2,15,16,17,18,19,21,23,20,22 ] },
        { new(Face.Left,   Spin.HalfTurn),            [ 8, 1,10, 3,12, 5,14, 7, 0, 9, 2,11, 4,13, 6,15,16,17,18,19,23,22,21,20 ] },
        { new(Face.Top,    Spin.ClockWise),           [16,17, 2, 3, 4, 5, 6, 7, 8, 9,21,20,14,12,15,13,11,10,18,19, 0, 1,22,23 ] },
        { new(Face.Top,    Spin.CounterClockWise),    [20,21, 2, 3, 4, 5, 6, 7, 8, 9,17,16,13,15,12,14, 0, 1,18,19,11,10,22,23 ] },
        { new(Face.Top,    Spin.HalfTurn),            [11,10, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0,15,14,13,12,20,21,18,19,16,17,22,23 ] },
        { new(Face.Bottom, Spin.ClockWise),           [ 0, 1,22,23, 6, 4, 7, 5,19,18,10,11,12,13,14,15,16,17, 2, 3,20,21, 9, 8 ] },
        { new(Face.Bottom, Spin.CounterClockWise),    [ 0, 1,18,19, 5, 7, 4, 6,23,22,10,11,12,13,14,15,16,17, 9, 8,20,21, 2, 3 ] },
        { new(Face.Bottom, Spin.HalfTurn),            [ 0, 1, 9, 8, 7, 6, 5, 4, 3, 2,10,11,12,13,14,15,16,17,22,23,20,21,18,19 ] },
        { new(Face.FrontBack, Spin.ClockWise),           [ 2, 0, 3, 1,18,16,19,17, 9,11, 8,10,22,20,23,21,14,12,15,13, 6, 4, 7, 5 ] },
        { new(Face.FrontBack, Spin.CounterClockWise),    [ 1, 3, 0, 2,21,23,20,22,10, 8,11, 9,17,19,16,18, 5, 7, 4, 6,13,15,12,14 ] },
        { new(Face.FrontBack, Spin.HalfTurn),            [ 3, 2, 1, 0,15,14,13,12,11,10, 9, 8, 7, 6, 5, 4,23,22,21,20,19,18,17,16 ] },
        { new(Face.RightLeft, Spin.ClockWise),           [ 4, 5, 6, 7, 8, 9,10,11,12,13,14,15, 0, 1, 2, 3,18,16,19,17,21,23,20,22 ] },
        { new(Face.RightLeft, Spin.CounterClockWise),    [12,13,14,15, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,17,19,16,18,22,20,23,21 ] },
        { new(Face.RightLeft, Spin.HalfTurn),            [ 8, 9,10,11,12,13,14,15, 0, 1, 2, 3, 4, 5, 6, 7,19,18,17,16,23,22,21,20 ] },
        { new(Face.TopBottom, Spin.ClockWise),           [16,17,18,19, 5, 7, 4, 6,23,22,21,20,14,12,15,13,11,10, 9, 8, 0, 1, 2, 3 ] },
        { new(Face.TopBottom, Spin.CounterClockWise),    [20,21,22,23, 6, 4, 7, 5,19,18,17,16,13,15,12,14, 0, 1, 2, 3,11,10, 9, 8 ] },
        { new(Face.TopBottom, Spin.HalfTurn),            [11,10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0,15,14,13,12,20,21,22,23,16,17,18,19 ] },
    };
    private static readonly HashSet<State> s_finished = [
        State.CreateFinished(Color.White, Color.Red),
        State.CreateFinished(Color.White, Color.Blue),
        State.CreateFinished(Color.White, Color.Orange),
        State.CreateFinished(Color.White, Color.Green),
        State.CreateFinished(Color.Red, Color.Yellow),
        State.CreateFinished(Color.Red, Color.Blue),
        State.CreateFinished(Color.Red, Color.White),
        State.CreateFinished(Color.Red, Color.Green),
        State.CreateFinished(Color.Yellow, Color.Orange),
        State.CreateFinished(Color.Yellow, Color.Blue),
        State.CreateFinished(Color.Yellow, Color.Red),
        State.CreateFinished(Color.Yellow, Color.Green),
        State.CreateFinished(Color.Orange, Color.White),
        State.CreateFinished(Color.Orange, Color.Blue),
        State.CreateFinished(Color.Orange, Color.Yellow),
        State.CreateFinished(Color.Orange, Color.Green),
        State.CreateFinished(Color.Blue, Color.Red),
        State.CreateFinished(Color.Blue, Color.Yellow),
        State.CreateFinished(Color.Blue, Color.Orange),
        State.CreateFinished(Color.Blue, Color.White),
        State.CreateFinished(Color.Green, Color.White),
        State.CreateFinished(Color.Green, Color.Orange),
        State.CreateFinished(Color.Green, Color.Yellow),
        State.CreateFinished(Color.Green, Color.Red),
    ];
    public static void Move(State state, Move move)
    {
        state.Assign(state.GetTransformed(s_transforms[move]));
    }
    public static Tuple<List<Move>, State> Solve(State source, State? target = null)
    {
        if (source.Completeness is Completeness.Incomplete)
        {
            throw new InvalidOperationException(Constants.s_sourceIncomplete) { HResult = Constants.s_sourceIncompleteCode };
        }
        if (source.Completeness is Completeness.Wrong)
        {
            throw new InvalidOperationException(Constants.s_sourceWrong) { HResult = Constants.s_sourceWrongCode };
        }
        if(target is { })
        {
            if (target.Completeness is Completeness.Incomplete)
            {
                throw new InvalidOperationException(Constants.s_targetIncomplete) { HResult = Constants.s_targetIncompleteCode };
            }
            if (target.Completeness is Completeness.Wrong)
            {
                throw new InvalidOperationException(Constants.targetWrong) { HResult = Constants.s_targetWrongCode };
            }
        }
        List<Move> list = [];
        if ((target is null && !s_finished.Contains(source)) || (target is { } && source != target))
        {
            int ans = -1;
            State found = new();

            Dictionary<State, Tuple<State, Move>> prev0 = [];
            Dictionary<State, Tuple<State, Move>> prev = [];
            List<Queue<State>> qu0 = [ new() ];
            Dictionary<State, int> dists0 = [];
            if(target is State t)
            {
                qu0[0].Enqueue(t);
                dists0[t] = 0;
            }
            else
            {
                foreach (State s in s_finished)
                {
                    qu0[0].Enqueue(s);
                    dists0[s] = 0;
                }
            }
            List<Queue<State>> qu = [ new() ];
            Dictionary<State, int> dists = [];
            qu[0].Enqueue(source);
            dists[source] = 0;

            for (int i = 0; ans == -1 && i < Math.Max(qu.Count, qu0.Count); ++i)
            {
                while (ans == -1 && qu0[i].Count > 0)
                {
                    State cur0 = qu0[i].Dequeue();
                    if (dists0[cur0] == i)
                    {
                        foreach (KeyValuePair<Move, List<int>> tr in s_transforms)
                        {
                            State nb = cur0.GetTransformed(tr.Value);
                            if (!dists0.ContainsKey(nb))
                            {
                                dists0[nb] = i + 1;
                                if (qu0.Count - 1 < i + 1)
                                {
                                    qu0.AddRange(Enumerable.Range(qu0.Count, i + 2).Select(v => new Queue<State>()));
                                }
                                qu0[i + 1].Enqueue(nb);
                                prev0[nb] = new Tuple<State, Move>(cur0, tr.Key);
                            }
                            if (dists.TryGetValue(nb, out int d))
                            {
                                ans = d + i + 1;
                                found = nb;
                                break;
                            }
                        }
                    }
                }
                while (ans == -1 && qu[i].Count > 0)
                {
                    State cur1 = qu[i].Dequeue();
                    if (dists[cur1] == i)
                    {
                        foreach (KeyValuePair<Move, List<int>> tr in s_transforms)
                        {
                            State nb = cur1.GetTransformed(tr.Value);
                            if (!dists.ContainsKey(nb))
                            {
                                dists[nb] = i + 1;
                                if (qu.Count - 1 < i + 1)
                                {
                                    qu.AddRange(Enumerable.Range(qu.Count, i + 2).Select(v => new Queue<State>()));
                                }
                                qu[i + 1].Enqueue(nb);
                                prev[nb] = new Tuple<State, Move>(cur1, tr.Key);
                            }
                            if (dists0.TryGetValue(nb, out int d))
                            {
                                ans = d + i + 1;
                                found = nb;
                                break;
                            }
                        }
                    }
                }
            }
            if (ans == -1)
            {
                throw new InvalidOperationException(Constants.s_TargetUnreachable) { HResult = Constants.s_inputSourceUnreachableCode };
            }

            Stack<Move> solve = new();
            State cur = found;
            while (true)
            {
                if (!prev.TryGetValue(cur, out Tuple<State, Move>? obj))
                {
                    break;
                }
                solve.Push(obj.Item2);
                cur = obj.Item1;
            }

            while (solve.Count > 0)
            {
                list.Add(solve.Pop());
            }

            cur = found;
            while (true)
            {
                if (!prev0.TryGetValue(cur, out Tuple<State, Move>? obj))
                {
                    return new Tuple<List<Move>, State>(list, cur);
                }
                list.Add(
                    new Move(
                        obj.Item2.Face, 
                        obj.Item2.Spin is Spin.HalfTurn ? obj.Item2.Spin : (obj.Item2.Spin is Spin.ClockWise ? Spin.CounterClockWise : Spin.ClockWise)
                    )
                );
                cur = obj.Item1;
            }
        }
        return new Tuple<List<Move>, State>(list, source);
    }
}
