"""Per-game character policy for display and lobby names.

Configured in the developer dashboard and published inside the public game
data blob as ``name_rules``::

    class MyGameData(TypedDict, total=False):
        name_rules: "NameRules"

The server enforces the policy at registration, rename and room creation
regardless of what the client sends — ``NameRules.validate`` mirrors the same
checks locally so input fields can give live feedback. When no rules are
configured the Latin-letters-only default applies. Profanity is a separate
concern — combine with ``text_filter.is_clean`` / ``Moderation.censor`` (the
server checks both).
"""

from __future__ import annotations

import unicodedata
from dataclasses import dataclass
from typing import ClassVar, Optional

_CASE_VALUES = ("any", "lower", "upper", "capitalize", "capitalize_strict")

# Latin Unicode blocks — Script=Latin is approximated as letters inside
# these ranges (combining marks and punctuation there are Inherited/Common).
_LATIN_RANGES = (
    (0x41, 0x5A), (0x61, 0x7A), (0xC0, 0x2AF), (0x2B0, 0x2FF),
    (0x1D00, 0x1DBF), (0x1E00, 0x1EFF), (0x2C60, 0x2C7F), (0xA720, 0xA7FF),
    (0xAB30, 0xAB6F), (0xFB00, 0xFB06), (0xFF21, 0xFF3A), (0xFF41, 0xFF5A),
    (0x10780, 0x107BA), (0x1DF00, 0x1DF1E),
)

# Cyrillic blocks — Script=Cyrillic covers the whole block including marks.
_CYRILLIC_RANGES = (
    (0x400, 0x52F), (0x2DE0, 0x2DFF), (0xA640, 0xA69F), (0x1C80, 0x1C8F),
)

_LABELS = {
    "latin": "Latin letters",
    "cyrillic": "Cyrillic letters",
    "digits": "digits",
    "space": "spaces",
    "underscore": "underscores",
    "hyphen": "hyphens",
    "symbols": "symbols",
}


def _in_ranges(cp: int, ranges) -> bool:
    return any(lo <= cp <= hi for lo, hi in ranges)


def _classify(ch: str) -> str:
    cp = ord(ch)
    cat = unicodedata.category(ch)
    if cat.startswith("L") and _in_ranges(cp, _LATIN_RANGES):
        return "latin"
    if _in_ranges(cp, _CYRILLIC_RANGES):
        return "cyrillic"
    if cat == "Nd":  # decimal digits only, like \p{Nd}
        return "digits"
    if cat in ("Zs", "Zl", "Zp") or 0x09 <= cp <= 0x0D or cp == 0x85:
        return "space"
    if ch == "_":
        return "underscore"
    if ch == "-" or 0x2010 <= cp <= 0x2015:
        return "hyphen"

    return "symbols"


@dataclass
class NameRules:
    """Character policy mirroring the server-side ``name_rules`` object."""

    latin: bool = True
    cyrillic: bool = False
    digits: bool = False
    space: bool = False
    underscore: bool = False
    hyphen: bool = False
    symbols: bool = False
    case: str = "any"  # any | lower | upper | capitalize | capitalize_strict

    #: Latin-letters-only fallback for games without configured rules.
    DEFAULT: "ClassVar[NameRules]" = None  # set below the class body

    @classmethod
    def from_dict(cls, data: Optional[dict]) -> "NameRules":
        """Builds normalized rules from a raw ``name_rules`` game-data dict."""
        rules = cls()
        if not isinstance(data, dict):
            return rules
        for key in _LABELS:
            if key in data:
                setattr(rules, key, bool(data[key]))
        if data.get("case") in _CASE_VALUES:
            rules.case = data["case"]
        return rules

    def validate(self, name: Optional[str]) -> Optional[str]:
        """Returns a human-readable reason, or None when the name is valid."""
        if not name:
            return "Name is required"

        upper = lower = 0
        first_upper: Optional[bool] = None
        for ch in name:
            cls = _classify(ch)
            if not getattr(self, cls):
                return f"Name cannot contain {_LABELS[cls]}"
            if cls in ("latin", "cyrillic"):
                if ch.isupper():
                    upper += 1
                    if first_upper is None:
                        first_upper = True
                else:
                    lower += 1
                    if first_upper is None:
                        first_upper = False

        if self.case == "lower" and upper:
            return "Name must be lowercase"
        if self.case == "upper" and lower:
            return "Name must be uppercase"
        if self.case == "capitalize" and (upper > 1 or (upper == 1 and first_upper is False)):
            return "Only the first letter of the name may be uppercase"
        if self.case == "capitalize_strict" and (upper != 1 or first_upper is not True):
            return "Name must start with a capital letter followed by lowercase"

        return None


NameRules.DEFAULT = NameRules()


def validate_name(name: Optional[str], rules: Optional[NameRules] = None) -> Optional[str]:
    """Checks a name against the given rules (or the default policy)."""
    return (rules or NameRules.DEFAULT).validate(name)
