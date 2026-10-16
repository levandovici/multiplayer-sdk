"""Movement-prediction helpers for the realtime channel.

:class:`PredictionBuffer` is the client-side input history used for
prediction with server reconciliation: simulate locally, record each
input with the ``seq`` stamped on the realtime message, then adopt the
authoritative state when it arrives and replay every input sent after
the acknowledged seq.

:class:`SnapshotBuffer` is the remote-entity side: a small history of
timestamped snapshots sampled at a render time offset in the past, so
remote movement plays back smoothly behind the wire.
"""

from __future__ import annotations

from collections import deque
from typing import Any, Callable, Deque, Generic, List, Optional, Tuple, TypeVar

S = TypeVar("S")  # movement snapshot (position, velocity, ...)
I = TypeVar("I")  # one tick of player intent


class PredictionBuffer(Generic[S, I]):
    """Client-side prediction input history."""

    def __init__(self, capacity: int = 128) -> None:
        #: ~2s of unacked inputs at 60Hz
        self._pending: Deque[Tuple[int, I, S]] = deque(maxlen=max(1, capacity))
        self._last_seq = 0

    def next_seq(self) -> int:
        """Seq of the next input to send."""
        self._last_seq += 1
        return self._last_seq

    @property
    def pending_count(self) -> int:
        """Inputs still awaiting authoritative acknowledgement."""
        return len(self._pending)

    def record(self, seq: int, input_: I, predicted_state: S) -> None:
        """Record a locally applied input and its resulting state."""
        self._pending.append((seq, input_, predicted_state))

    def reconcile(
        self,
        ack_seq: int,
        authoritative: S,
        apply: Callable[[S, I, float], S],
        dt_seconds: float,
    ) -> S:
        """Reconcile against the authoritative state for ``ack_seq``.

        Drops acknowledged inputs and replays the rest on top.

        :param ack_seq: Seq echoed back by the authority (0 = none).
        :param authoritative: Authoritative state for ``ack_seq``.
        :param apply: ``(state, input, dt_seconds) -> advanced state``.
        :param dt_seconds: Fixed input tick length used at predict time.
        """
        while self._pending and self._pending[0][0] <= ack_seq:
            self._pending.popleft()
        state = authoritative
        for _, input_, _s in self._pending:
            state = apply(state, input_, dt_seconds)
        return state

    def reset(self) -> None:
        """Clear all recorded inputs (teleport / respawn)."""
        self._pending.clear()


class SnapshotBuffer(Generic[S]):
    """Remote-entity interpolation buffer."""

    def __init__(self, capacity: int = 32) -> None:
        #: ~1s of retained history is plenty
        self._capacity = max(2, capacity)
        self._snaps: List[Tuple[float, int, S]] = []
        self._last_seq = 0

    def add(self, time_seconds: float, state: S, seq: int = 0) -> None:
        """Add a snapshot stamped with its emission time.

        Stale seqs are dropped — UDP may reorder and the latest state
        always wins.
        """
        if seq:
            if seq <= self._last_seq:
                return
            self._last_seq = seq
        snap = (time_seconds, seq, state)
        i = len(self._snaps)
        while i > 0 and self._snaps[i - 1][0] > time_seconds:
            i -= 1
        self._snaps.insert(i, snap)
        while len(self._snaps) > self._capacity:
            self._snaps.pop(0)

    def sample(
        self,
        render_time: float,
        lerp: Callable[[S, S, float], S],
    ) -> Optional[S]:
        """Sample the buffer at ``render_time`` (typically now minus the
        interpolation delay). Clamps at the edges; None when empty.

        :param lerp: ``(a, b, alpha 0..1) -> blended state``.
        """
        if not self._snaps:
            return None
        if render_time <= self._snaps[0][0]:
            return self._snaps[0][2]
        if render_time >= self._snaps[-1][0]:
            return self._snaps[-1][2]
        for i in range(len(self._snaps) - 1, 0, -1):
            a, b = self._snaps[i - 1], self._snaps[i]
            if a[0] <= render_time:
                span = b[0] - a[0]
                alpha = (render_time - a[0]) / span if span > 0 else 0.0
                return lerp(a[2], b[2], max(0.0, min(1.0, alpha)))
        return self._snaps[-1][2]

    def reset(self) -> None:
        """Drop buffered snapshots (player left, entity despawned)."""
        self._snaps.clear()
        self._last_seq = 0


__all__ = ["PredictionBuffer", "SnapshotBuffer"]
