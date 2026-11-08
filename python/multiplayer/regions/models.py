"""Models for the region discovery API (``discover.php``)."""

from __future__ import annotations

import enum
from dataclasses import dataclass, field
from typing import ClassVar, List, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import CommonError


@dataclass
class RealtimeEndpoint:
    """Realtime (WS/UDP) endpoint advertised by a service node."""

    host: str = ""
    port: int = 443
    protocol: str = "wss"
    udp_port: Optional[int] = None


@dataclass
class ServiceNode:
    """One enabled API region from the discovery directory."""

    region: str = ""
    label: str = ""
    api_url: str = ""
    realtime: Optional[RealtimeEndpoint] = None


@dataclass
class DiscoverResponse(ApiResponse):
    """Enabled service nodes plus the server's geo suggestion."""

    error_enum: ClassVar[Type[enum.Enum]] = CommonError

    nodes: List[ServiceNode] = field(default_factory=list)
    suggested_region: Optional[str] = None


@dataclass
class GameNodeResponse(ApiResponse):
    """Result of ``discover.php?api_key=…`` — the node a game is pinned to."""

    error_enum: ClassVar[Type[enum.Enum]] = CommonError

    region: Optional[str] = None
    node: Optional[ServiceNode] = None
