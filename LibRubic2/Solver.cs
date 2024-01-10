using System.Collections.Generic;

namespace Net.Leksi.Rubic2;

public class Solver
{
    private static readonly Dictionary<Move, List<int>> s_transforms = new(new MoveEqualityComparer())
    {
        { new(Face.Front, Spin.ClockWise),         [ 2, 0, 3, 1,18,16, 6, 7, 8, 9,10,11,12,13,23,21,14,17,15,19,20, 4,22, 5 ] },
        { new(Face.Front, Spin.CounterClockWise),  [ 1, 3, 0, 2,21,23, 6, 7, 8, 9,10,11,12,13,16,18, 5,17, 4,19,20,15,22,14 ] },
        { new(Face.Back, Spin.ClockWise),          [ 0, 1, 2, 3, 4, 5,20,22,10, 8,11, 9,17,19,14,15,16, 7,18, 6,13,21,12,23 ] },
        { new(Face.Back, Spin.CounterClockWise),   [ 0, 1, 2, 3, 4, 5,19,17, 9,11, 8,10,22,20,14,15,16,12,18,13, 6,21, 7,23 ] },
        { new(Face.Right, Spin.ClockWise),         [ 0, 5, 2, 7, 4, 9, 6,11, 8,13,10,15,12, 1,14, 3,18,16,19,17,20,21,22,23 ] },
        { new(Face.Right, Spin.CounterClockWise),  [ 0,13, 2,15, 4, 1, 6, 3, 8, 5,10, 7,12, 9,14,11,17,19,16,18,20,21,22,23 ] },
        { new(Face.Left, Spin.ClockWise),          [12, 1,14, 3, 0, 5, 2, 7, 4, 9, 6,11, 8,13,10,15,16,17,18,19,22,20,23,21 ] },
        { new(Face.Left, Spin.CounterClockWise),   [ 4, 1, 6, 3, 8, 5,10, 7,12, 9,14,11, 0,13, 2,15,16,17,18,19,21,23,20,22 ] },
        { new(Face.Top, Spin.ClockWise),           [16,17, 2, 3, 4, 5, 6, 7, 8, 9,21,20,14,12,15,13,11,10,18,19, 0, 1,22,23 ] },
        { new(Face.Top, Spin.CounterClockWise),    [20,21, 2, 3, 4, 5, 6, 7, 8, 9,17,16,13,15,12,14, 0, 1,18,19,11,10,22,23 ] },
        { new(Face.Bottom, Spin.ClockWise),        [ 0, 1,22,23, 6, 4, 7, 5,19,18,10,11,12,13,14,15,16,17, 2, 3,20,21, 9, 8 ] },
        { new(Face.Bottom, Spin.CounterClockWise), [ 0, 1,18,19, 5, 7, 4, 6,23,22,10,11,12,13,14,15,16,17, 9, 8,20,21, 2, 3 ] },
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
    public static Tuple<List<Move>, State> Solve(State state)
    {
        if (state.Completeness is Completeness.Incomplete)
        {
            throw new InvalidOperationException("Input state is incomplete!");
        }
        if (state.Completeness is Completeness.Wrong)
        {
            throw new InvalidOperationException("Input state is wrong!");
        }
        List<Move> list = new List<Move>();
        if (!s_finished.Contains(state))
        {
            int ans = -1;
            State found = new();

            StateEqualityComparer sec = new();
            Dictionary<State, Tuple<State, Move>> prev0 = new(sec);
            Dictionary<State, Tuple<State, Move>> prev = new(sec);
            List<Queue<State>> qu0 = new() { new() };
            Dictionary<State, int> dists0 = new(sec);
            foreach(State s in s_finished)
            {
                qu0[0].Enqueue(s);
                dists0[s] = 0;
            }
            List<Queue<State>> qu = new() { new() };
            Dictionary<State, int> dists = new(sec);
            qu[0].Enqueue(state);
            dists[state] = 0;

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
                                    qu0.AddRange(Enumerable.Range(0, i + 2 - qu0.Count).Select(v => new Queue<State>()));
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
                                    qu.AddRange(Enumerable.Range(0, i + 2 - qu.Count).Select(v => new Queue<State>()));
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
                throw new InvalidOperationException("Input state is unreachable!");
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
                list.Add(new Move(obj.Item2.Face, obj.Item2.Spin is Spin.ClockWise ? Spin.CounterClockWise : Spin.ClockWise));
                cur = obj.Item1;
            }
        }
        return new Tuple<List<Move>, State>(list, state);
    }
}
