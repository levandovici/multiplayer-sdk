"""Internal helpers for JSON (de)serialization of SDK models.

The API speaks JSON with snake_case keys. Response/request models are plain
dataclasses whose field names already match the wire format, so this module
only needs generic dict <-> dataclass conversion plus ISO-8601 datetime parsing.
"""

from __future__ import annotations

import dataclasses
import enum
import typing
from datetime import datetime, timezone
from typing import Any, Dict, Optional, Type, TypeVar

T = TypeVar("T")


def _parse_datetime(value: Any) -> Any:
    """Parse an ISO-8601 string into an aware ``datetime``. Returns the raw
    value unchanged if it is not a parseable date string."""
    if not isinstance(value, str):
        return value
    text = value.strip()
    if not text:
        return None
    if text.endswith("Z"):
        text = text[:-1] + "+00:00"
    # MySQL-style "YYYY-MM-DD HH:MM:SS" -> ISO
    if len(text) >= 10 and text[10:11] == " ":
        text = text[:10] + "T" + text[11:]
    try:
        dt = datetime.fromisoformat(text)
    except ValueError:
        return value
    if dt.tzinfo is None:
        dt = dt.replace(tzinfo=timezone.utc)
    return dt


def _strip_optional(typ: Any) -> typing.Tuple[Any, bool]:
    """Return (inner_type, is_optional) for ``Optional[X]`` / ``X | None``."""
    origin = typing.get_origin(typ)
    if origin is typing.Union:
        args = [a for a in typing.get_args(typ) if a is not type(None)]
        if len(args) == 1:
            return args[0], True
    return typ, False


def convert_value(value: Any, typ: Any) -> Any:
    """Convert a raw JSON value into the annotated field type."""
    if value is None:
        return None

    typ, _ = _strip_optional(typ)

    if typ is Any or typ is None:
        return value

    origin = typing.get_origin(typ)
    if origin in (list, typing.List):
        (item_type,) = typing.get_args(typ) or (Any,)
        if isinstance(value, list):
            return [convert_value(item, item_type) for item in value]
        return value

    if origin in (dict, typing.Dict):
        return value

    if isinstance(typ, type):
        if issubclass(typ, enum.Enum):
            if isinstance(value, typ):
                return value
            try:
                return typ(value)
            except (ValueError, KeyError):
                try:
                    return typ[str(value).strip().lower()]
                except (KeyError, AttributeError):
                    return value
        if typ is datetime:
            return _parse_datetime(value)
        if dataclasses.is_dataclass(typ) and isinstance(value, dict):
            return from_dict(typ, value)
        if typ is bool:
            return bool(value)
        if typ is int and not isinstance(value, bool):
            try:
                return int(value)
            except (TypeError, ValueError):
                return value

    return value


def from_dict(cls: Type[T], data: Optional[Dict[str, Any]]) -> T:
    """Build a dataclass instance from a JSON dict.

    Keys are matched case-insensitively; unknown keys are ignored and missing
    keys fall back to the field default.
    """
    if data is None:
        data = {}

    try:
        hints = typing.get_type_hints(cls)
    except Exception:
        hints = {}

    lowered = {str(k).lower(): v for k, v in data.items()}
    kwargs: Dict[str, Any] = {}
    for field in dataclasses.fields(cls):
        if not field.init:
            continue
        if field.name in data:
            raw = data[field.name]
        elif field.name.lower() in lowered:
            raw = lowered[field.name.lower()]
        else:
            continue
        kwargs[field.name] = convert_value(raw, hints.get(field.name, field.type))
    return cls(**kwargs)  # type: ignore[arg-type]


def to_jsonable(value: Any) -> Any:
    """Convert dataclasses/enums/datetimes into JSON-serializable values.

    ``None`` dict values are dropped to mirror the .NET SDK's
    ``WhenWritingNull`` behaviour.
    """
    if value is None:
        return None
    if isinstance(value, enum.Enum):
        return value.value
    if isinstance(value, datetime):
        return value.astimezone(timezone.utc).isoformat().replace("+00:00", "Z")
    if dataclasses.is_dataclass(value) and not isinstance(value, type):
        return {
            f.name: to_jsonable(getattr(value, f.name))
            for f in dataclasses.fields(value)
            if getattr(value, f.name) is not None
        }
    if isinstance(value, dict):
        return {k: to_jsonable(v) for k, v in value.items() if v is not None}
    if isinstance(value, (list, tuple)):
        return [to_jsonable(v) for v in value]
    return value
