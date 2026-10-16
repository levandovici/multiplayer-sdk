#pragma once

#include <algorithm>
#include <cstdint>
#include <deque>
#include <functional>
#include <vector>

namespace michitai {
namespace multiplayer {
namespace rooms {
namespace realtime {

// Movement-prediction helpers for the realtime channel.
//
// PredictionBuffer is the client-side input history used for prediction
// with server reconciliation: simulate locally, record each input with
// the seq stamped on the realtime message, then adopt the authoritative
// state when it arrives and replay every input sent after the
// acknowledged seq.
//
// SnapshotBuffer is the remote-entity side: a small history of
// timestamped snapshots sampled at a render time offset in the past, so
// remote movement plays back smoothly behind the wire.

/// Client-side prediction input history.
/// @tparam S Movement snapshot (position, velocity, ...).
/// @tparam I One tick of player intent.
template <typename S, typename I>
class PredictionBuffer {
public:
    /// @param capacity Max unacked inputs retained (128 covers ~2s at 60Hz).
    explicit PredictionBuffer(size_t capacity = 128) : capacity_(capacity) {}

    /// Seq of the next input to send.
    uint32_t nextSeq() { return ++lastSeq_; }

    /// Inputs still awaiting authoritative acknowledgement.
    size_t pendingCount() const { return pending_.size(); }

    /// Records a locally applied input and its resulting state.
    void record(uint32_t seq, const I& input, const S& predictedState) {
        pending_.push_back({seq, input, predictedState});
        while (pending_.size() > capacity_) pending_.pop_front();
    }

    /// Reconciles against the authoritative state for @p ackSeq: drops
    /// acknowledged inputs and replays the rest on top.
    /// @param ackSeq Seq echoed back by the authority (0 = none).
    /// @param authoritative Authoritative state for ackSeq.
    /// @param apply (state, input, dtSeconds) -> advanced state.
    /// @param dtSeconds Fixed input tick length used at predict time.
    S reconcile(uint32_t ackSeq, const S& authoritative,
                const std::function<S(const S&, const I&, float)>& apply,
                float dtSeconds) {
        while (!pending_.empty() && pending_.front().seq <= ackSeq)
            pending_.pop_front();
        S state = authoritative;
        for (const auto& e : pending_)
            state = apply(state, e.input, dtSeconds);
        return state;
    }

    /// Clears all recorded inputs (teleport / respawn).
    void reset() { pending_.clear(); }

private:
    struct Entry {
        uint32_t seq;
        I input;
        S state;
    };
    std::deque<Entry> pending_;
    size_t capacity_;
    uint32_t lastSeq_ = 0;
};

/// Remote-entity interpolation buffer.
/// @tparam S Remote entity snapshot.
template <typename S>
class SnapshotBuffer {
public:
    /// @param capacity Snapshots retained (~1s of history is plenty).
    explicit SnapshotBuffer(size_t capacity = 32) : capacity_(capacity < 2 ? 2 : capacity) {}

    /// Adds a snapshot stamped with its emission time. Stale seqs are
    /// dropped — UDP may reorder and the latest state always wins.
    void add(double timeSeconds, const S& state, uint32_t seq = 0) {
        if (seq != 0) {
            if (seq <= lastSeq_) return;
            lastSeq_ = seq;
        }
        Snap snap{timeSeconds, seq, state};
        auto it = snaps_.end();
        while (it != snaps_.begin() && (it - 1)->time > timeSeconds) --it;
        snaps_.insert(it, snap);
        while (snaps_.size() > capacity_) snaps_.erase(snaps_.begin());
    }

    /// Samples the buffer at @p renderTime (typically now minus the
    /// interpolation delay). Clamps at the edges; false when empty.
    /// @param lerp (a, b, alpha 0..1) -> blended state.
    bool sample(double renderTime,
                const std::function<S(const S&, const S&, double)>& lerp,
                S& out) const {
        if (snaps_.empty()) return false;
        if (renderTime <= snaps_.front().time) {
            out = snaps_.front().state;
            return true;
        }
        if (renderTime >= snaps_.back().time) {
            out = snaps_.back().state;
            return true;
        }
        for (size_t i = snaps_.size() - 1; i > 0; --i) {
            const Snap& a = snaps_[i - 1];
            const Snap& b = snaps_[i];
            if (a.time <= renderTime) {
                double span = b.time - a.time;
                double alpha = span > 0 ? (renderTime - a.time) / span : 0.0;
                out = lerp(a.state, b.state, std::max(0.0, std::min(1.0, alpha)));
                return true;
            }
        }
        out = snaps_.back().state;
        return true;
    }

    /// Drops buffered snapshots (player left, entity despawned).
    void reset() {
        snaps_.clear();
        lastSeq_ = 0;
    }

private:
    struct Snap {
        double time;
        uint32_t seq;
        S state;
    };
    std::vector<Snap> snaps_;
    size_t capacity_;
    uint32_t lastSeq_ = 0;
};

} // namespace realtime
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
