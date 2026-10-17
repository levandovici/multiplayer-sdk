"""Local fast-path profanity screen — hash-only wordlist.

Mirrors the PHP reference (src/text_filter.php) and the .NET TextFilter.
The embedded list (``_data.BASE64``) is an MFIL v1 blob built by
scripts/build_moderation_dat.php — salted FNV-1a-64 hashes plus per-position
run requirements, no readable words. Local checks give instant feedback;
the server stays authoritative (register/rename, ``Moderation.censor``).

Matching is evasion-resistant: lowercase + NFKC, leet/diacritic/Cyrillic/
Greek confusables folded to Latin, combining marks and separators stripped,
same-letter runs collapsed, digits resolve as ambiguity classes ('1' -> i/l,
'8' -> b/p/v), wildcards ('*', '_', ...) act as "any one letter".

Tiers:
  severe    — substring over the whole folded string ("w e e d" flags)
  words>=5  — substring inside a token
  words<5   — whole-token equality only ("noob" flags, "noble" doesn't)
  allow     — folded-string exemptions ("grape", "therapist", ...)
"""

from __future__ import annotations

import base64
import unicodedata

from ._data import BASE64 as _DATA_B64

SENTINEL = ''          # wildcard: matches any one letter
MASK_CHAR = '*'
DIGIT_BASE = 0x10        # marker chars = chr(DIGIT_BASE + digit)

MAX_NAME_LENGTH = 16     # display-name cap — matches the virtual keyboard

_FNV_OFFSET = 14695981039346656037
_FNV_PRIME = 1099511628211
_U64 = (1 << 64) - 1

# Confusable/leet map — applied to the lowercased NFKC form of each input
# char AND to word-list entries, so both sides fold identically.
_SUBST = {
    # leet / symbol lookalikes (digits fold through _DIGIT_CLASS markers)
    '@': 'a', '$': 's', '€': 'e', '£': 'l', '¥': 'y',
    '(': 'c',

    # diacritic folds
    'à': 'a', 'á': 'a', 'â': 'a', 'ã': 'a', 'ä': 'a', 'å': 'a',
    'è': 'e', 'é': 'e', 'ê': 'e', 'ë': 'e', 'ě': 'e',
    'ì': 'i', 'í': 'i', 'î': 'i', 'ï': 'i',
    'ò': 'o', 'ó': 'o', 'ô': 'o', 'õ': 'o', 'ö': 'o', 'ø': 'o',
    'ù': 'u', 'ú': 'u', 'û': 'u', 'ü': 'u', 'ű': 'u',
    'ý': 'y', 'ÿ': 'y',
    'ç': 'c', 'ć': 'c', 'č': 'c',
    'ñ': 'n', 'ń': 'n', 'ň': 'n',
    'š': 's', 'ś': 's', 'ş': 's', 'ș': 's',
    'ž': 'z', 'ź': 'z', 'ż': 'z',
    'ğ': 'g', 'ı': 'i', 'ł': 'l', 'đ': 'd', 'ð': 'd',
    'ţ': 't', 'ț': 't', 'ť': 't', 'þ': 'th', 'ß': 'ss',
    'æ': 'ae', 'œ': 'oe', 'ů': 'u', 'ř': 'r',
    'ď': 'd', 'ę': 'e', 'ą': 'a', 'ė': 'e', 'ų': 'u', 'ū': 'u',
    'ī': 'i', 'ĺ': 'l', 'ļ': 'l', 'ő': 'o', 'ă': 'a',
    'ĉ': 'c', 'ŭ': 'u',

    # Cyrillic → Latin phonetic
    'а': 'a', 'б': 'b', 'в': 'v', 'г': 'g', 'д': 'd',
    'е': 'e', 'ё': 'e', 'ж': 'zh', 'з': 'z', 'и': 'i',
    'й': 'i', 'к': 'k', 'л': 'l', 'м': 'm', 'н': 'n',
    'о': 'o', 'п': 'p', 'р': 'r', 'с': 's', 'т': 't',
    'у': 'u', 'ф': 'f', 'х': 'h', 'ц': 'c', 'ч': 'ch',
    'ш': 'sh', 'щ': 'sh', 'ы': 'y', 'э': 'e', 'ю': 'yu',
    'я': 'ya', 'і': 'i', 'ї': 'yi', 'є': 'e', 'ґ': 'g',
    'ў': 'u',

    # Greek → Latin phonetic
    'α': 'a', 'β': 'v', 'γ': 'g', 'δ': 'd', 'ε': 'e',
    'ζ': 'z', 'η': 'i', 'θ': 'th', 'ι': 'i', 'κ': 'k',
    'λ': 'l', 'μ': 'm', 'ν': 'n', 'ξ': 'x', 'ο': 'o',
    'π': 'p', 'ρ': 'r', 'σ': 's', 'ς': 's', 'τ': 't',
    'υ': 'u', 'φ': 'f', 'χ': 'h', 'ψ': 'ps', 'ω': 'o',
}

# "Any single letter" during matching — '*'/'?' self-censor dodge, '_'/'-'
# name glue. Display passes them through unless inside a masked span.
_WILDCARD = frozenset('*?#%&_-~!+$|•°=.')

# Dropped without breaking a token — combining marks are handled by
# category; soft signs/apostrophes fold away so "don't" stays one token.
_SILENT = frozenset("'’‘`´ъь\"")

# Digits are ambiguous: '1' reads as i/l, '3' as e or Cyrillic З ('z'),
# '8' as b, Cyrillic В ('v') or p... A digit folds to a private marker
# char and a marker satisfies any letter in its class.
_DIGIT_CLASS = {
    '0': 'o', '1': 'il', '2': 'tzs', '3': 'eza', '4': 'a',
    '5': 's', '6': 'bg', '7': 'thv', '8': 'bpv', '9': 'gds',
}

_MARK = unicodedata.category
_MARK_NONSPACING = {'Mn', 'Mc'}
_DROP_CATS = {'Cc', 'Cf', 'Cs', 'Co', 'Cn'}
_KEEP_CATS = {'Lu', 'Ll', 'Lt', 'Lm', 'Lo', 'Nd'}


def _is_marker(ch: str) -> bool:
    o = ord(ch)
    return DIGIT_BASE <= o < DIGIT_BASE + 10


def _fnv_byte(h: int, b: int) -> int:
    return ((h ^ b) * _FNV_PRIME) & _U64


def _fnv_str(h: int, s: str) -> int:
    for b in s.encode('utf-8'):
        h = _fnv_byte(h, b)
    return h


class _Folded:
    """Separator-free, run-collapsed text + map back to source indices."""

    __slots__ = ('text', 'map', 'run_end', 'run', 'token', 'token_start')

    def __init__(self) -> None:
        self.text = ''
        self.map = []        # pos -> original index (first char of run)
        self.run_end = []    # pos -> original index (last char of run)
        self.run = []        # pos -> collapsed repeat count
        self.token = []      # pos -> token id
        self.token_start = []  # token id -> first pos


def _emit_folded(ch: str, out: list) -> None:
    # Single-char compatibility decomposition: fullwidth letters,
    # ligatures (ﬁ -> fi), circled digits, superscripts.
    norm = unicodedata.normalize('NFKC', ch)
    for n in norm:
        # mb_strtolower-style full mapping — can yield >1 char ('İ'->'i̇').
        for low in n.lower():
            if low in _SUBST:
                out.append(_SUBST[low])
            elif low in _DIGIT_CLASS:
                out.append(chr(DIGIT_BASE + ord(low) - ord('0')))
            elif low in _WILDCARD:
                out.append(SENTINEL)
            elif _MARK(low) in _KEEP_CATS:
                out.append(low)


def _fold(text: str) -> _Folded:
    r = _Folded()
    if not text:
        return r

    sb = []
    fpos = 0          # current folded-text length
    token_id = -1
    boundary = True
    last_letter = '\0'   # last non-sentinel char — see the mark rule
    mark_run = 0      # consecutive identical digit markers at the tail

    for i, c in enumerate(text):
        if c.lower() in _SILENT:
            continue

        if _MARK(c) in _MARK_NONSPACING:
            # Combining marks are essential vowels/tones in Thai,
            # Devanagari, Arabic — keep them after a non-ASCII base inside
            # the same token ("บ้า"). Sentinels don't reset the base, so a
            # mark split from its letter by a separator still survives.
            # After ASCII they're zalgo noise and get dropped
            # ("v ä p e" -> "vape").
            if not boundary and sb and ord(last_letter) > 0x7F:
                low = c.lower()
                if sb[-1] == low:
                    r.run_end[-1] = i
                    r.run[-1] += 1
                else:
                    sb.append(low)
                    fpos += len(low)
                    r.map.append(i)
                    r.run_end.append(i)
                    r.run.append(1)
                    r.token.append(token_id)
                    last_letter = low
            continue

        piece: list = []
        _emit_folded(c, piece)
        piece_s = ''.join(piece)

        if not piece_s:
            boundary = True
            continue

        if boundary:
            token_id += 1
            r.token_start.append(fpos)
            boundary = False

        for e in piece_s:
            # Runs of the same folded letter collapse inside a token only.
            # Digit markers stay separate so "d11d0" can feed different
            # slots ('i' then 'l') — except past three in a row, where a
            # long digit tail would make the bounded matcher enumerate
            # exponentially ("e2e-999…9-1").
            if sb and sb[-1] == e and r.token[-1] == token_id:
                if not _is_marker(e) or mark_run >= 3:
                    r.run_end[-1] = i
                    r.run[-1] += 1
                    continue
                mark_run += 1
            else:
                mark_run = 1 if _is_marker(e) else 0
            sb.append(e)
            fpos += 1
            r.map.append(i)
            r.run_end.append(i)
            r.run.append(1)
            r.token.append(token_id)
            if e != SENTINEL:
                last_letter = e

    r.text = ''.join(sb)
    return r


class _State:
    """Loaded hash tables + per-scan scratch (not thread-safe by design —
    create one TextFilter per thread if needed; C#/PHP behave the same)."""

    VARIANT_BUDGET = 4096

    def __init__(self) -> None:
        self.severe = {}       # hash -> runs bytes
        self.word_sub = {}
        self.word_exact = {}
        self.allow = {}
        self.severe_max = self.word_sub_max = self.word_exact_max = 0
        self.seed = _FNV_OFFSET
        self.alphabet = ''
        self.wild_ascii = ''
        self.wild_mark = ''
        self.in_alphabet = set()
        self.slot_run = [0] * 64
        # How each emitted slot was satisfied: 0 literal letter, 1 digit
        # marker, 2 sentinel substitute. A substitute is legal only between
        # two literal edges of the word — "v*pe" keeps "vape" because the
        # '*' sits between real 'f' and 'c', while "Ora-35" guessing a word
        # on "-3" glue is a false positive.
        self.slot_kind = [0] * 64
        # Reachable run ends per slot depth, so the longest run emits
        # first (recursion clobbers shared scratch — rows are per slot).
        self.emit_end = [0] * (64 * 64)
        self.emit_sum = [0] * (64 * 64)
        self.emit_kind = [0] * (64 * 64)
        self.budget = 0
        self.overflow = False
        self.subs = False
        self.loaded = False

    # ---- binary reader ------------------------------------------------

    def _u16(self, d: bytes, off: int) -> tuple:
        return d[off] | (d[off + 1] << 8), off + 2

    def _u32(self, d: bytes, off: int) -> tuple:
        return int.from_bytes(d[off:off + 4], 'little'), off + 4

    def _u64(self, d: bytes, off: int) -> tuple:
        return int.from_bytes(d[off:off + 8], 'little'), off + 8

    def _load_section(self, d: bytes, off: int, table: dict, max_len: int) -> tuple:
        count, off = self._u32(d, off)
        for _ in range(count):
            h, off = self._u64(d, off)
            n = d[off]
            off += 1
            runs = d[off:off + n]
            off += n
            if n == 0 or n > len(self.slot_run):
                continue
            if h not in table:
                table[h] = runs
            if n > max_len:
                max_len = n
        return off, max_len

    def load(self) -> None:
        self.severe.clear()
        self.word_sub.clear()
        self.word_exact.clear()
        self.allow.clear()
        self.severe_max = self.word_sub_max = self.word_exact_max = 0
        self.alphabet = self.wild_ascii = self.wild_mark = ''
        self.in_alphabet = set()
        self.seed = _FNV_OFFSET
        self.loaded = True

        try:
            d = base64.b64decode(_DATA_B64)
            if len(d) < 26 or d[:4] != b'MFIL':
                raise ValueError('bad magic')
            off = 4
            version, off = self._u16(d, off)
            _, off = self._u16(d, off)          # flags, reserved
            if version != 1:
                raise ValueError('unsupported version')
            for i in range(16):
                self.seed = _fnv_byte(self.seed, d[off + i])
            off += 16

            alen, off = self._u16(d, off)
            self.alphabet = d[off:off + alen].decode('utf-8')
            off += alen
            self.in_alphabet = set(self.alphabet)

            # Sentinels enumerate ASCII letters, plus combining marks only
            # when the token holds non-ASCII letters — marks cover
            # separators dropped inside a grapheme cluster, and keeping
            # them out of Latin tokens holds the branch factor (and the
            # variant budget) small for names like "a_b_c".
            self.wild_ascii = ''.join(c for c in self.alphabet if ord(c) < 0x80)
            self.wild_mark = ''.join(
                c for c in self.alphabet if _MARK(c) in ('Mn', 'Mc'))

            off, self.severe_max = self._load_section(d, off, self.severe, self.severe_max)
            off, self.word_sub_max = self._load_section(d, off, self.word_sub, self.word_sub_max)
            off, self.word_exact_max = self._load_section(d, off, self.word_exact, self.word_exact_max)

            n_allow, off = self._u32(d, off)
            for _ in range(n_allow):
                h, off = self._u64(d, off)
                n = d[off]
                off += 1
                self.allow[h] = d[off:off + n]
                off += n
        except Exception:
            # Corrupt/missing data disables the local filter — the
            # server-side check still applies on register/rename.
            self.severe.clear()
            self.word_sub.clear()
            self.word_exact.clear()
            self.allow.clear()
            self.severe_max = self.word_sub_max = self.word_exact_max = 0

    # An allow hit needs the same folded text AND enough input run per
    # position: "shiite" is safe, but the same fold with a single 'i'
    # ("shite") is a listed word, not an allow hit.
    def _allow_ok(self, f: Folded, s: int, e: int) -> bool:
        need = self.allow.get(_fnv_str(self.seed, f.text[s:e]))
        if need is None or len(need) != e - s:
            return False
        return all(f.run[s + i] >= (need[i] & 0x3F) for i in range(e - s))

    def ensure_loaded(self) -> None:
        if not self.loaded:
            self.load()

    # ---- hash-scan matcher --------------------------------------------
    #
    # Matching enumerates the letter strings the folded input can resolve
    # to: a literal position yields its char, a digit marker every letter
    # in its class, a sentinel every alphabet letter (wildcard slots satisfy
    # only run-1 pattern positions). A slot consumes every consecutive
    # input position that can carry its letter, summing runs — "w e e d"
    # still matches "weed". A per-start budget caps adversarial variant
    # storms; overflow just ends the local check instead of flagging.

    @staticmethod
    def _has_letter(f: _Folded, kind: list, cls: list, j: int, l: str) -> bool:
        k = kind[j]
        if k == 0:
            return f.text[j] == l
        if k == 1:
            return l in cls[j]
        return False

    def _runs_ok(self, slots: int, need: bytes) -> bool:
        for k in range(slots):
            run = need[k] & 0x3F
            if self.slot_kind[k] == 2:
                if run > 1:
                    return False
            elif need[k] & 0x80 and self.slot_kind[k] == 0:
                # A digit-derived slot of a listed entry can't be filled by
                # a real letter — "4 2 0" stays digit-only.
                return False
            elif self.slot_run[k] < run:
                return False
        return True

    # A match needs grounded anchors — input positions that are not
    # wildcards: three when a wildcard substitutes, two when separators
    # were merely skipped, one otherwise. Digit markers count — a marker
    # resolves to a small fixed class, so "455" spelling a short entry is
    # deliberate, not a guess. With fewer, sentinels alone could spell a
    # short entry: "-9-" resolves on wildcards without touching a letter.
    @staticmethod
    def _token_wide(f: _Folded, pos: int) -> bool:
        # True when the sentinel's token holds any non-ASCII letter — only
        # then can a wildcard plausibly stand in for a combining mark.
        t = f.token[pos]
        i = pos
        while i >= 0 and f.token[i] == t:
            if ord(f.text[i]) > 0x7F:
                return True
            i -= 1
        i = pos + 1
        while i < len(f.text) and f.token[i] == t:
            if ord(f.text[i]) > 0x7F:
                return True
            i += 1
        return False

    def _literal_ok(self, kind: list, start: int, end: int, slots: int,
                    need: bytes) -> bool:
        lit = 0
        ground = 0
        last_lit = -1
        any_sent = False
        for i in range(start, end):
            if kind[i] == 0:
                lit += 1
                ground += 1
                last_lit = i
            elif kind[i] == 2:
                any_sent = True
            else:
                ground += 1
        # A separator anywhere before the last letter is woven evasion,
        # so the "-digits" tail can no longer pass as name glue
        # ("k_u_n_7"). A bare digit inside the head is NOT enough
        # ("e2e-99" stays clean — timestamp handles carry digits
        # without any separator trickery).
        sent_before_lit = any(kind[i] == 2 for i in range(start, last_lit))
        any_wild = any(self.slot_kind[k] == 2 for k in range(slots))
        all_wild_mark = all(need[k] & 0x40 for k in range(slots)
                            if self.slot_kind[k] == 2)
        # An entry whose slots are all digit-derived is already exact
        # when spelled in digits, so glue between them still flags.
        all_dig = all(need[k] & 0x80 for k in range(slots))
        if not (lit == 0 and all_dig):
            if ground < ((2 if all_wild_mark else 3) if any_wild
                         else 2 if any_sent else 1):
                return False
            # "-digits" after the last real letter is name glue, not
            # evasion ("Xena-21" can't spell a word) — but only when the
            # separators all precede the digits AND the word used
            # no digit substitution. A separator woven between digits
            # ("5_h_1_7") or anywhere earlier in the word is evasion,
            # so the tail rule lifts.
            glue = interleaved = seen_marker = False
            for i in range(last_lit + 1, end):
                if kind[i] == 1:
                    seen_marker = True
                elif kind[i] == 2:
                    if seen_marker:
                        interleaved = True
                        break
                    glue = True
            if glue and not interleaved and not sent_before_lit:
                return False
        return True

    def _enum(self, f: _Folded, pos: int, kind: list, cls: list, slot: int,
              h: int, start: int, table: dict,
              max_len: int, require_end: int, spans: list) -> None:
        n = len(f.text)

        if (slot > 0 and (require_end < 0 or pos == require_end)
                and kind[start] != 2 and kind[pos - 1] != 2
                and (need := table.get(h)) is not None and len(need) == slot
                and self._runs_ok(slot, need)
                and self._literal_ok(kind, start, pos, slot, need)):
            spans.append((start, pos))

        if self.overflow or slot == max_len or pos >= n:
            return

        self.budget -= 1
        if self.budget < 0:
            self.overflow = True
            return

        if kind[pos] == 2:          # sentinel: junk to skip, or a wildcard slot
            # Junk first — "w_e_e_d" spells its word with the '_'s skipped,
            # and the cheap path must run before the 26-way wildcard
            # enumeration drains the variant budget.
            self._enum(f, pos + 1, kind, cls, slot, h,
                       start, table, max_len, require_end, spans)
            # Substitution only onto a literal-bounded slot: the previous
            # slot must have ended on a real letter (the right edge is
            # enforced when the next slot emits — markers can't follow a
            # substitute).
            if self.subs and slot > 0 and self.slot_kind[slot - 1] == 0:
                n_wild = len(self.wild_ascii)
                if self._token_wide(f, pos):
                    n_wild += len(self.wild_mark)
                for a in range(n_wild):
                    w = (self.wild_ascii[a] if a < len(self.wild_ascii)
                         else self.wild_mark[a - len(self.wild_ascii)])
                    self.slot_run[slot] = 0
                    self.slot_kind[slot] = 2
                    self._enum(f, pos + 1, kind, cls, slot + 1,
                               _fnv_str(h, w), start, table,
                               max_len, require_end, spans)
                    if self.overflow:
                        return
            return

        if kind[pos] == 0:          # literal letter
            l = f.text[pos]
            if l not in self.in_alphabet:
                return
            # Emit at every reachable run end, longest first: sentinels
            # inside a run are junk ("w-e-e-d" counts the two e's) while
            # the cut-before-sentinel emission leaves it free to
            # substitute the next slot ("v*pe"). Longest-first keeps deep
            # runs ahead of the wildcard substitution storm when the
            # variant budget is tight.
            j, total, cnt, b = pos, 0, 0, slot * 64
            while j < n and cnt < 64:
                if kind[j] == 2:
                    j += 1
                    continue
                if not self._has_letter(f, kind, cls, j, l):
                    break
                total += f.run[j]
                j += 1
                self.emit_end[b + cnt] = j
                self.emit_sum[b + cnt] = total
                self.emit_kind[b + cnt] = kind[j - 1]
                cnt += 1
            for k in range(cnt - 1, -1, -1):
                self.slot_run[slot] = self.emit_sum[b + k]
                self.slot_kind[slot] = self.emit_kind[b + k]
                self._enum(f, self.emit_end[b + k], kind, cls, slot + 1,
                           _fnv_str(h, l), start, table,
                           max_len, require_end, spans)
                if self.overflow:
                    return
            return

        # A digit marker can't be the next slot's first position after a
        # substitute — that would make the substituted edge a guess both
        # ways. Separators interleaved with markers mid-word are legal
        # ("w_3_3_d"); a glued "-digits" tail is rejected by the hit
        # check instead.
        if slot > 0 and self.slot_kind[slot - 1] == 2:
            return

        for l in cls[pos]:          # digit marker → every class letter
            if l not in self.in_alphabet:
                continue
            j, total, cnt, b = pos, 0, 0, slot * 64
            while j < n and cnt < 64:
                if kind[j] == 2:
                    j += 1
                    continue
                if not self._has_letter(f, kind, cls, j, l):
                    break
                total += f.run[j]
                j += 1
                self.emit_end[b + cnt] = j
                self.emit_sum[b + cnt] = total
                self.emit_kind[b + cnt] = kind[j - 1]
                cnt += 1
            for k in range(cnt - 1, -1, -1):
                self.slot_run[slot] = self.emit_sum[b + k]
                self.slot_kind[slot] = self.emit_kind[b + k]
                self._enum(f, self.emit_end[b + k], kind, cls, slot + 1,
                           _fnv_str(h, l), start, table,
                           max_len, require_end, spans)
                if self.overflow:
                    return

    def _scan_pos(self, f: _Folded, start: int, kind: list, cls: list,
                  table: dict, max_len: int, require_end: int,
                  spans: list) -> None:
        # A match can't start on a wildcard — "D_ape" flagging "vape" on a
        # leading '_' is a false positive, and enumerating from a sentinel
        # burns the whole variant budget on a guaranteed no-hit.
        if max_len == 0 or kind[start] == 2:
            return

        # Pass 1 is skip-only (no wildcard substitutions) — it is cheap,
        # so real skip-evasions resolve before the bounded enumeration can
        # exhaust the variant budget on substitution storms.
        self.subs = False
        self.budget = self.VARIANT_BUDGET
        self.overflow = False
        self._enum(f, start, kind, cls, 0, self.seed, start,
                   table, max_len, require_end, spans)

        self.subs = True
        self.budget = self.VARIANT_BUDGET
        self.overflow = False
        self._enum(f, start, kind, cls, 0, self.seed, start,
                   table, max_len, require_end, spans)

        # On budget exhaustion the local check just stops — enumerating
        # can't prove a match either way and "u_n_i_t_e_d"-shaped names are
        # legitimate. The authoritative server check stays unbounded.

    def find_matches(self, f: _Folded) -> list:
        self.ensure_loaded()

        spans = []
        n = len(f.text)
        if n == 0:
            return spans

        kind = [0] * n
        cls = [None] * n
        for i, c in enumerate(f.text):
            if c == SENTINEL:
                kind[i] = 2
            elif _is_marker(c):
                kind[i] = 1
                cls[i] = _DIGIT_CLASS[chr(ord('0') + ord(c) - DIGIT_BASE)]

        allow_tok = [False] * len(f.token_start)
        for t in range(len(f.token_start)):
            s = f.token_start[t]
            e = f.token_start[t + 1] if t + 1 < len(f.token_start) else n
            allow_tok[t] = self._allow_ok(f, s, e)

        if not self._allow_ok(f, 0, n):
            for p in range(n):
                if kind[p] == 0 and f.text[p] not in self.in_alphabet:
                    continue
                self._scan_pos(f, p, kind, cls, self.severe,
                               self.severe_max, -1, spans)

            # Long words start matches only at token boundaries — "s t u p i d"
            # flags but a stray "stupid" run-in doesn't need to.
            for t in range(len(f.token_start)):
                s = f.token_start[t]
                e = f.token_start[t + 1] if t + 1 < len(f.token_start) else n
                for p in range(s, e):
                    if p != s and allow_tok[t]:
                        continue
                    if kind[p] == 0 and f.text[p] not in self.in_alphabet:
                        continue
                    self._scan_pos(f, p, kind, cls, self.word_sub,
                                   self.word_sub_max, -1, spans)

            # Short words flag only when the whole collapsed string equals
            # the word — "n o o b" -> "noob" flags, "noble" does not.
            self._scan_pos(f, 0, kind, cls, self.word_exact,
                           self.word_exact_max, n, spans)

        # Short words inside a non-allowlisted token — skipped entirely for
        # allowlisted tokens (keeps "seaweed" clean despite "weed").
        for t in range(len(f.token_start)):
            if allow_tok[t]:
                continue
            start = f.token_start[t]
            end = f.token_start[t + 1] if t + 1 < len(f.token_start) else n
            self._scan_pos(f, start, kind, cls, self.word_exact,
                           self.word_exact_max, end, spans)

        # A match fully inside an allowlisted token is safe — "weed" inside
        # "seaweed" — even when the whole string isn't listed. Tokens span
        # separators and digits ("weed-0" is one token), so the same rescue
        # applies to the containing literal letter run.
        def _in_allow_run(sp):
            if (f.token[sp[0]] == f.token[sp[1] - 1]
                    and allow_tok[f.token[sp[0]]]):
                return True
            if kind[sp[0]] != 0:
                return False
            rs, re = sp[0], sp[0] + 1
            while rs > 0 and kind[rs - 1] == 0:
                rs -= 1
            while re < n and kind[re] == 0:
                re += 1
            return sp[1] <= re and self._allow_ok(f, rs, re)

        spans = [sp for sp in spans if not _in_allow_run(sp)]

        # A match with zero literal letters is only evidence when it covers
        # its whole digit group: "455" or "14_88" as standalone tokens flag,
        # but a subslice of a longer digit run ("segg" inside a timestamp's
        # digits) is incidental noise.
        def _glue_subslice(sp):
            if any(kind[p] == 0 for p in range(sp[0], sp[1])):
                return False
            gs, ge = sp[0], sp[0] + 1
            while gs > 0 and kind[gs - 1] != 0:
                gs -= 1
            while ge < n and kind[ge] != 0:
                ge += 1
            return sp[0] != gs or sp[1] != ge

        spans = [sp for sp in spans if not _glue_subslice(sp)]

        # Mask the whole token(s) a match touches — leaving "W***D" behind
        # defeats the purpose.
        out = []
        for s, e in spans:
            first = f.token[s]
            last = f.token[e - 1]
            out.append((f.token_start[first],
                        f.token_start[last + 1] if last + 1 < len(f.token_start)
                        else n))
        return out


_state = _State()


def reload() -> None:
    """Re-reads the embedded word list."""
    _state.load()


def is_clean(text: str) -> bool:
    """True when the text contains no listed word after folding."""
    return not _state.find_matches(_fold(text))


def is_name_allowed(name: str) -> bool:
    """Entry-side validation for typed names: length + profanity."""
    if name is None:
        return False
    trimmed = name.strip()
    if len(trimmed) < 2 or len(trimmed) > 50:
        return False
    return is_clean(trimmed)


def filter_text(text: str) -> str:
    """Masks listed words with '*'; strips markup ('<' '>') plus
    invisible/control characters. Use for every user-provided string
    before it reaches a label."""
    if not text:
        return text

    folded = _fold(text)
    matches = _state.find_matches(folded)

    mask = [False] * len(text)
    for s, e in matches:
        for i in range(s, e):
            for j in range(folded.map[i], folded.run_end[i] + 1):
                mask[j] = True

    out = []
    i = 0
    while i < len(text):
        c = text[i]
        if c == '<':
            close = text.find('>', i + 1)
            if close > i:
                i = close + 1
                continue
        if c == '>':
            i += 1
            continue
        if _MARK(c) in _DROP_CATS:
            i += 1
            continue
        out.append(MASK_CHAR if mask[i] else c)
        i += 1
    return ''.join(out)


def filter_name(name: str, fallback: str = 'Player') -> str:
    """filter_text() for a display name; falls back to a neutral name when
    the mask leaves nothing readable (e.g. the whole name is a slur)."""
    filtered = filter_text(name)
    if not filtered:
        return fallback
    return filtered if any(c.isalnum() for c in filtered) else fallback


def sanitize_imported_name(raw: str):
    """Turns an external name (Steam persona) into a valid in-game name —
    collapsed whitespace, length cap, letters/digits plus '_'/'-'.
    Returns None when nothing usable remains or the result fails the
    filter; the caller then keeps the server-assigned default name."""
    if not raw:
        return None

    out = []
    pending_space = False
    for c in raw.strip():
        if c.isspace():
            pending_space = True
            continue
        if not (c.isalnum() or c in '_-'):
            continue
        if pending_space and out:
            out.append(' ')
        pending_space = False
        out.append(c)

    name = ''.join(out)
    if len(name) > MAX_NAME_LENGTH:
        name = name[:MAX_NAME_LENGTH].rstrip()
    return name if is_name_allowed(name) else None
