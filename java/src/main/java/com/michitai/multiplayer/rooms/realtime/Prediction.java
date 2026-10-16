package com.michitai.multiplayer.rooms.realtime;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Deque;
import java.util.List;

/**
 * Movement-prediction helpers for the realtime channel.
 *
 * <p>{@link PredictionBuffer} is the client-side input history used for
 * prediction with server reconciliation: simulate locally, record each
 * input with the seq stamped on the realtime message, then adopt the
 * authoritative state when it arrives and replay every input sent after
 * the acknowledged seq.</p>
 *
 * <p>{@link SnapshotBuffer} is the remote-entity side: a small history of
 * timestamped snapshots sampled at a render time offset in the past, so
 * remote movement plays back smoothly behind the wire.</p>
 */
public final class Prediction {

    private Prediction() {}

    /** (state, input, dtSeconds) -> advanced state. */
    @FunctionalInterface
    public interface ApplyFn<S, I> {
        S apply(S state, I input, float dtSeconds);
    }

    /** (a, b, alpha 0..1) -> blended state. */
    @FunctionalInterface
    public interface LerpFn<S> {
        S lerp(S a, S b, double alpha);
    }

    /**
     * Client-side prediction input history.
     *
     * @param <S> Movement snapshot (position, velocity, ...).
     * @param <I> One tick of player intent.
     */
    public static final class PredictionBuffer<S, I> {
        private static final class Entry<S, I> {
            long seq;
            I input;
            S state;
        }

        private final Deque<Entry<S, I>> pending = new ArrayDeque<>();
        private final int capacity;
        private long lastSeq;

        /** @param capacity Max unacked inputs retained (128 covers ~2s at 60Hz). */
        public PredictionBuffer(int capacity) {
            this.capacity = Math.max(1, capacity);
        }

        public PredictionBuffer() {
            this(128);
        }

        /** Seq of the next input to send. */
        public long nextSeq() {
            return ++lastSeq;
        }

        /** Inputs still awaiting authoritative acknowledgement. */
        public int pendingCount() {
            return pending.size();
        }

        /** Records a locally applied input and its resulting state. */
        public synchronized void record(long seq, I input, S predictedState) {
            Entry<S, I> e = new Entry<>();
            e.seq = seq;
            e.input = input;
            e.state = predictedState;
            pending.addLast(e);
            while (pending.size() > capacity) pending.pollFirst();
        }

        /**
         * Reconciles against the authoritative state for {@code ackSeq}:
         * drops acknowledged inputs and replays the rest on top.
         *
         * @param ackSeq        Seq echoed back by the authority (0 = none).
         * @param authoritative Authoritative state for ackSeq.
         * @param apply         (state, input, dtSeconds) -> advanced state.
         * @param dtSeconds     Fixed input tick length used at predict time.
         */
        public synchronized S reconcile(long ackSeq, S authoritative,
                                        ApplyFn<S, I> apply, float dtSeconds) {
            while (!pending.isEmpty() && pending.peekFirst().seq <= ackSeq) {
                pending.pollFirst();
            }
            S state = authoritative;
            for (Entry<S, I> e : pending) {
                state = apply.apply(state, e.input, dtSeconds);
            }
            return state;
        }

        /** Clears all recorded inputs (teleport / respawn). */
        public synchronized void reset() {
            pending.clear();
        }
    }

    /**
     * Remote-entity interpolation buffer.
     *
     * @param <S> Remote entity snapshot.
     */
    public static final class SnapshotBuffer<S> {
        private static final class Snap<S> {
            double time;
            long seq;
            S state;
        }

        private final List<Snap<S>> snaps = new ArrayList<>();
        private final int capacity;
        private long lastSeq;

        /** @param capacity Snapshots retained (~1s of history is plenty). */
        public SnapshotBuffer(int capacity) {
            this.capacity = Math.max(2, capacity);
        }

        public SnapshotBuffer() {
            this(32);
        }

        /**
         * Adds a snapshot stamped with its emission time. Stale seqs are
         * dropped — UDP may reorder and the latest state always wins.
         */
        public synchronized void add(double timeSeconds, S state, long seq) {
            if (seq != 0) {
                if (seq <= lastSeq) return;
                lastSeq = seq;
            }
            Snap<S> snap = new Snap<>();
            snap.time = timeSeconds;
            snap.seq = seq;
            snap.state = state;
            int i = snaps.size();
            while (i > 0 && snaps.get(i - 1).time > timeSeconds) i--;
            snaps.add(i, snap);
            while (snaps.size() > capacity) snaps.remove(0);
        }

        /**
         * Samples the buffer at {@code renderTime} (typically now minus the
         * interpolation delay). Clamps at the edges.
         *
         * @param lerp (a, b, alpha 0..1) -> blended state.
         * @return The sampled state, or null when the buffer is empty.
         */
        public synchronized S sample(double renderTime, LerpFn<S> lerp) {
            if (snaps.isEmpty()) return null;
            Snap<S> first = snaps.get(0);
            if (renderTime <= first.time) return first.state;
            Snap<S> last = snaps.get(snaps.size() - 1);
            if (renderTime >= last.time) return last.state;
            for (int i = snaps.size() - 1; i > 0; i--) {
                Snap<S> a = snaps.get(i - 1);
                Snap<S> b = snaps.get(i);
                if (a.time <= renderTime) {
                    double span = b.time - a.time;
                    double alpha = span > 0 ? (renderTime - a.time) / span : 0.0;
                    return lerp.lerp(a.state, b.state, Math.max(0.0, Math.min(1.0, alpha)));
                }
            }
            return last.state;
        }

        /** Drops buffered snapshots (player left, entity despawned). */
        public synchronized void reset() {
            snaps.clear();
            lastSeq = 0;
        }
    }
}
