using System;
using System.Collections.Generic;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Client-side input history for movement prediction with server
    /// reconciliation. The game simulates inputs immediately and records
    /// each with the seq stamped on the realtime message; when the
    /// authoritative state arrives it is adopted and every input sent
    /// after the acknowledged seq is replayed on top.
    /// </summary>
    /// <typeparam name="TState">Movement snapshot (position, velocity, ...).</typeparam>
    /// <typeparam name="TInput">One tick of player intent.</typeparam>
    public sealed class PredictionBuffer<TState, TInput>
    {
        private struct Entry
        {
            public uint Seq;
            public TInput Input;
            public TState State;
        }

        private readonly Queue<Entry> _pending = new Queue<Entry>();
        private readonly int _capacity;
        private uint _lastSeq;

        /// <param name="capacity">Max unacked inputs retained (128 covers ~2s at 60Hz).</param>
        public PredictionBuffer(int capacity = 128)
        {
            _capacity = Math.Max(1, capacity);
        }

        /// <summary>Seq of the next input to send.</summary>
        public uint NextSeq() => unchecked(++_lastSeq);

        /// <summary>Inputs still awaiting authoritative acknowledgement.</summary>
        public int PendingCount => _pending.Count;

        /// <summary>
        /// Records a locally applied input and its resulting state.
        /// </summary>
        public void Record(uint seq, TInput input, TState predictedState)
        {
            _pending.Enqueue(new Entry { Seq = seq, Input = input, State = predictedState });
            while (_pending.Count > _capacity) _pending.Dequeue();
        }

        /// <summary>
        /// Reconciles against the authoritative state for <paramref name="ackSeq"/>:
        /// drops acknowledged inputs and replays the rest on top.
        /// </summary>
        /// <param name="ackSeq">Seq echoed back by the authority (0 = none).</param>
        /// <param name="authoritative">Authoritative state for ackSeq.</param>
        /// <param name="apply">(state, input, dtSeconds) -> advanced state.</param>
        /// <param name="dtSeconds">Fixed input tick length used at predict time.</param>
        public TState Reconcile(uint ackSeq, TState authoritative,
            Func<TState, TInput, float, TState> apply, float dtSeconds)
        {
            while (_pending.Count > 0 && _pending.Peek().Seq <= ackSeq)
                _pending.Dequeue();

            var state = authoritative;
            foreach (var e in _pending)
                state = apply(state, e.Input, dtSeconds);
            return state;
        }

        /// <summary>Clears all recorded inputs (teleport / respawn).</summary>
        public void Reset() => _pending.Clear();
    }

    /// <summary>
    /// Interpolation buffer for remote entities: keeps recent timestamped
    /// snapshots and samples them at a render time offset in the past,
    /// so remote movement plays back smoothly behind the wire.
    /// </summary>
    /// <typeparam name="TState">Remote entity snapshot.</typeparam>
    public sealed class SnapshotBuffer<TState>
    {
        private struct Snap
        {
            public double Time;
            public uint Seq;
            public TState State;
        }

        private readonly List<Snap> _snaps = new List<Snap>();
        private readonly int _capacity;
        private uint _lastSeq;

        /// <param name="capacity">Snapshots retained (~1s of history is plenty).</param>
        public SnapshotBuffer(int capacity = 32)
        {
            _capacity = Math.Max(2, capacity);
        }

        /// <summary>
        /// Adds a snapshot stamped with its emission time. Stale seqs are
        /// dropped — UDP may reorder and the latest state always wins.
        /// </summary>
        public void Add(double timeSeconds, TState state, uint seq = 0)
        {
            if (seq != 0)
            {
                if (seq <= _lastSeq) return;
                _lastSeq = seq;
            }
            var snap = new Snap { Time = timeSeconds, Seq = seq, State = state };
            int i = _snaps.Count;
            while (i > 0 && _snaps[i - 1].Time > timeSeconds) i--;
            _snaps.Insert(i, snap);
            while (_snaps.Count > _capacity) _snaps.RemoveAt(0);
        }

        /// <summary>
        /// Samples the buffer at <paramref name="renderTime"/> (typically
        /// now minus the interpolation delay). Clamps at the edges.
        /// </summary>
        /// <param name="lerp">(a, b, alpha 0..1) -> blended state.</param>
        public bool TrySample(double renderTime, Func<TState, TState, double, TState> lerp,
            out TState result)
        {
            result = default!;
            if (_snaps.Count == 0) return false;
            if (renderTime <= _snaps[0].Time)
            {
                result = _snaps[0].State;
                return true;
            }
            var last = _snaps[_snaps.Count - 1];
            if (renderTime >= last.Time)
            {
                result = last.State;
                return true;
            }
            for (int i = _snaps.Count - 1; i > 0; i--)
            {
                if (_snaps[i - 1].Time <= renderTime)
                {
                    var a = _snaps[i - 1];
                    var b = _snaps[i];
                    var span = b.Time - a.Time;
                    var alpha = span > 0 ? (renderTime - a.Time) / span : 0.0;
                    result = lerp(a.State, b.State, Math.Clamp(alpha, 0.0, 1.0));
                    return true;
                }
            }
            result = last.State;
            return true;
        }

        /// <summary>Drops buffered snapshots (player left, entity despawned).</summary>
        public void Reset()
        {
            _snaps.Clear();
            _lastSeq = 0;
        }
    }
}
