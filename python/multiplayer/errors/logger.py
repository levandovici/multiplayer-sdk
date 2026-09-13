"""Logging interfaces used by the SDK client."""

from __future__ import annotations

from typing import Protocol, runtime_checkable


@runtime_checkable
class Logger(Protocol):
    """Protocol for loggers that can be passed to :class:`Client`."""

    def log(self, message: str) -> None: ...
    def warn(self, message: str) -> None: ...
    def error(self, message: str) -> None: ...


class ConsoleLogger:
    """Default logger that prints messages to the console."""

    def log(self, message: str) -> None:
        print(f"[Log] {message}")

    def warn(self, message: str) -> None:
        print(f"[Warning] {message}")

    def error(self, message: str) -> None:
        print(f"[Error] {message}")
