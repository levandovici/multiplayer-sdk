#pragma once

#include "ModerationData.h"
#include "ModerationFold.h"

#include <algorithm>
#include <cstdint>
#include <stdexcept>
#include <string>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace michitai {
namespace multiplayer {
namespace moderation {

/// Local fast-path profanity screen — hash-only wordlist.
///
/// Mirrors the PHP reference (src/text_filter.php) and the .NET TextFilter.
/// The embedded list (kModerationDat) is an MFIL v1 blob built by
/// scripts/build_moderation_dat.php — salted FNV-1a-64 hashes plus
/// per-position run requirements, no readable words. Local checks give
/// instant feedback; the server stays authoritative (register/rename and
/// Moderation::censor).
///
/// Matching is evasion-resistant: lowercase + compatibility decomposition,
/// leet/diacritic/Cyrillic/Greek confusables folded to Latin, combining
/// marks and separators stripped, same-letter runs collapsed, digits
/// resolve as ambiguity classes ('1' -> i/l, '8' -> b/p/v), wildcards
/// ('*', '_', ...) act as "any one letter".
///
/// Tiers:
///   severe   — substring over the whole folded string ("w e e d" flags)
///   words>=5 — substring inside a token
///   words<5  — whole-token equality only ("noob" flags, "noble" doesn't)
///   allow    — folded-string exemptions ("grape", "therapist", ...)
///
/// Input is UTF-8; internally everything is processed as codepoints.
/// Not thread-safe for concurrent scans (shared scratch state) — the same
/// limitation as the C#/PHP implementations.
class TextFilter {
public:
    /// Display-name length cap — matches the virtual keyboard.
    static constexpr int MAX_NAME_LENGTH = 16;

    /// True when the text contains no listed word after folding.
    static bool isClean(const std::string& text) {
        Folded f;
        fold(text, f);
        std::vector<Span> spans;
        findMatches(f, spans);
        return spans.empty();
    }

    /// Entry-side validation for typed names: length + profanity.
    static bool isNameAllowed(const std::string& name) {
        std::string trimmed = trim(name);
        // The server accepts 2..50 chars; the keyboard caps MAX_NAME_LENGTH.
        std::vector<uint32_t> cps;
        decodeUtf8(trimmed, cps, nullptr);
        if (cps.size() < 2 || cps.size() > 50)
            return false;
        return isClean(trimmed);
    }

    /// Masks listed words with '*' and strips markup ('<' '>') plus
    /// invisible/control characters. Use for every user-provided string
    /// before it reaches a label.
    static std::string filter(const std::string& text) {
        if (text.empty())
            return text;

        std::vector<uint32_t> cps;
        std::vector<uint32_t> byteOff;
        decodeUtf8(text, cps, &byteOff);

        Folded f;
        foldCps(cps, f);
        std::vector<Span> spans;
        findMatches(f, spans);

        std::vector<char> mask(text.size(), 0);
        for (const Span& sp : spans)
            for (int i = sp.start; i < sp.end; i++)
                for (int j = f.map[i]; j <= f.runEnd[i]; j++)
                    for (uint32_t b = byteOff[j]; b < byteOff[j] + utf8Len(cps[j]); b++)
                        mask[b] = 1;

        std::string out;
        out.reserve(text.size());
        for (size_t i = 0; i < cps.size(); i++) {
            uint32_t c = cps[i];

            // Drop whole <...> runs so names cannot inject markup tags.
            if (c == '<') {
                size_t j = i + 1;
                while (j < cps.size() && cps[j] != '>')
                    j++;
                if (j < cps.size()) {
                    i = j;
                    continue;
                }
            }
            if (c == '>')
                continue;
            if (inRanges(kDropRanges, count(kDropRanges) / 2, c))
                continue;

            if (mask[byteOff[i]])
                out.push_back('*');
            else
                appendUtf8(out, c);
        }
        return out;
    }

    /// filter() for a display name; falls back to a neutral name when the
    /// mask leaves nothing readable (e.g. the whole name is a slur).
    static std::string filterName(const std::string& name,
                                  const std::string& fallback = "Player") {
        std::string filtered = filter(name);
        if (filtered.empty())
            return fallback;
        std::vector<uint32_t> cps;
        decodeUtf8(filtered, cps, nullptr);
        for (uint32_t c : cps)
            if (inRanges(kKeepRanges, count(kKeepRanges) / 2, c))
                return filtered;
        return fallback;
    }

    /// Turns an external name (Steam persona) into a valid in-game name —
    /// collapsed whitespace, length cap, letters/digits plus '_'/'-'.
    /// Returns an empty string when nothing usable remains or the result
    /// fails the filter; the caller keeps the server-assigned default.
    static std::string sanitizeImportedName(const std::string& raw) {
        if (raw.empty())
            return "";

        std::vector<uint32_t> cps;
        decodeUtf8(trim(raw), cps, nullptr);

        std::vector<uint32_t> out;
        bool pendingSpace = false;
        for (uint32_t c : cps) {
            if (inRanges(kSpaceRanges, count(kSpaceRanges) / 2, c)) {
                pendingSpace = true;
                continue;
            }
            if (!inRanges(kKeepRanges, count(kKeepRanges) / 2, c)
                    && c != '_' && c != '-') {
                continue;
            }
            if (pendingSpace && !out.empty())
                out.push_back(' ');
            pendingSpace = false;
            out.push_back(c);
        }

        if (out.size() > MAX_NAME_LENGTH) {
            out.resize(MAX_NAME_LENGTH);
            while (!out.empty() && inRanges(kSpaceRanges, count(kSpaceRanges) / 2, out.back()))
                out.pop_back();
        }

        std::string name;
        for (uint32_t c : out)
            appendUtf8(name, c);
        return isNameAllowed(name) ? name : "";
    }

    /// Re-reads the embedded word list.
    static void reload() {
        state().loaded = false;
        ensureLoaded();
    }

private:
    static constexpr uint32_t SENTINEL = 0x01; // wildcard: any one letter
    static constexpr uint32_t DIGIT_BASE = 0x10;
    static constexpr uint64_t FNV_OFFSET = 0xcbf29ce484222325ULL;
    static constexpr uint64_t FNV_PRIME  = 0x100000001b3ULL;
    static constexpr int VARIANT_BUDGET = 4096;

    struct Folded {
        std::vector<uint32_t> text;
        std::vector<int> map;        // pos -> source cp index (first of run)
        std::vector<int> runEnd;     // pos -> source cp index (last of run)
        std::vector<int> run;        // pos -> collapsed repeat count
        std::vector<int> token;      // pos -> token id
        std::vector<int> tokenStart; // token id -> first pos
    };

    struct Span { int start, end; };

    struct State {
        std::unordered_map<uint64_t, std::vector<uint8_t>> severe, wordSub, wordExact;
        std::unordered_map<uint64_t, std::vector<uint8_t>> allow;
        int severeMax = 0, wordSubMax = 0, wordExactMax = 0;
        uint64_t seed = FNV_OFFSET;
        std::vector<uint32_t> alphabet, wildAscii, wildMark;
        std::unordered_set<uint32_t> inAlphabet;
        int slotRun[64];
        // How each emitted slot was satisfied: 0 literal letter, 1 digit
        // marker, 2 sentinel substitute. A substitute is legal only between
        // two literal edges of the word — "v*pe" keeps "vape" because the
        // '*' sits between real 'f' and 'c', while "Ora-35" guessing a word
        // on "-3" glue is a false positive.
        uint8_t slotKind[64];
        // Reachable run ends per slot depth, so the longest run emits
        // first (recursion clobbers shared scratch — rows are per slot).
        int emitEnd[64 * 64];
        int emitSum[64 * 64];
        uint8_t emitKind[64 * 64];
        int budget = 0;
        bool overflow = false;
        bool subs = false;
        bool loaded = false;
    };

    static State& state() {
        static State s;
        return s;
    }

    // ---- utf8 helpers ----------------------------------------------------

    static int utf8Len(uint32_t cp) {
        return cp < 0x80 ? 1 : cp < 0x800 ? 2 : cp < 0x10000 ? 3 : 4;
    }

    static void appendUtf8(std::string& out, uint32_t cp) {
        if (cp < 0x80) {
            out.push_back((char) cp);
        } else if (cp < 0x800) {
            out.push_back((char) (0xC0 | (cp >> 6)));
            out.push_back((char) (0x80 | (cp & 0x3F)));
        } else if (cp < 0x10000) {
            out.push_back((char) (0xE0 | (cp >> 12)));
            out.push_back((char) (0x80 | ((cp >> 6) & 0x3F)));
            out.push_back((char) (0x80 | (cp & 0x3F)));
        } else {
            out.push_back((char) (0xF0 | (cp >> 18)));
            out.push_back((char) (0x80 | ((cp >> 12) & 0x3F)));
            out.push_back((char) (0x80 | ((cp >> 6) & 0x3F)));
            out.push_back((char) (0x80 | (cp & 0x3F)));
        }
    }

    static void decodeUtf8(const std::string& s, std::vector<uint32_t>& cps,
                           std::vector<uint32_t>* byteOff) {
        size_t i = 0, n = s.size();
        while (i < n) {
            unsigned char b = (unsigned char) s[i];
            uint32_t cp;
            int len;
            if (b < 0x80) { cp = b; len = 1; }
            else if ((b & 0xE0) == 0xC0) { cp = b & 0x1F; len = 2; }
            else if ((b & 0xF0) == 0xE0) { cp = b & 0x0F; len = 3; }
            else if ((b & 0xF8) == 0xF0) { cp = b & 0x07; len = 4; }
            else { cp = 0xFFFD; len = 1; }
            if (i + len > n) { cp = 0xFFFD; len = 1; }
            for (int k = 1; k < len; k++) {
                unsigned char cb = (unsigned char) s[i + k];
                if ((cb & 0xC0) != 0x80) { cp = 0xFFFD; len = 1; break; }
                cp = (cp << 6) | (cb & 0x3F);
            }
            cps.push_back(cp);
            if (byteOff)
                byteOff->push_back((uint32_t) i);
            i += len;
        }
    }

    static std::string trim(const std::string& s) {
        size_t a = s.find_first_not_of(" \t\r\n");
        if (a == std::string::npos)
            return "";
        size_t b = s.find_last_not_of(" \t\r\n");
        return s.substr(a, b - a + 1);
    }

    template <size_t N>
    static constexpr size_t count(const uint32_t (&)[N]) { return N; }

    static bool inRanges(const uint32_t* rs, size_t nPairs, uint32_t cp) {
        size_t lo = 0, hi = nPairs;
        while (lo < hi) {
            size_t mid = (lo + hi) / 2;
            if (cp < rs[mid * 2])
                hi = mid;
            else if (cp > rs[mid * 2 + 1])
                lo = mid + 1;
            else
                return true;
        }
        return false;
    }

    // map lookup: returns [ptr,len] of the folded replacement, or empty
    static std::pair<const unsigned char*, int> mapLookup(uint32_t cp) {
        const uint32_t n = (uint32_t) (sizeof(kMapCp) / sizeof(kMapCp[0]));
        uint32_t lo = 0, hi = n;
        while (lo < hi) {
            uint32_t mid = (lo + hi) / 2;
            if (cp < kMapCp[mid]) hi = mid;
            else if (cp > kMapCp[mid]) lo = mid + 1;
            else return { kMapBlob + kMapOff[mid], kMapLen[mid] };
        }
        return { nullptr, 0 };
    }

    // ---- emit (mirrors tf_emit; tables from ModerationFold.h) -------------

    static void emitFolded(uint32_t cp, std::vector<uint32_t>& out) {
        auto rep = mapLookup(cp);
        if (rep.first != nullptr) {
            // replacement bytes are folded letters or marker/sentinel bytes
            const unsigned char* p = rep.first;
            for (int i = 0; i < rep.second; ) {
                unsigned char b = p[i];
                if (b < 0x80) { out.push_back(b); i += 1; }
                else if ((b & 0xE0) == 0xC0) {
                    out.push_back(((b & 0x1F) << 6) | (p[i + 1] & 0x3F)); i += 2;
                } else if ((b & 0xF0) == 0xE0) {
                    out.push_back(((b & 0x0F) << 12) | ((p[i + 1] & 0x3F) << 6)
                                  | (p[i + 2] & 0x3F)); i += 3;
                } else {
                    out.push_back(((b & 0x07) << 18) | ((p[i + 1] & 0x3F) << 12)
                                  | ((p[i + 2] & 0x3F) << 6) | (p[i + 3] & 0x3F));
                    i += 4;
                }
            }
            return;
        }
        if (inRanges(kKeepRanges, count(kKeepRanges) / 2, cp))
            out.push_back(cp);
    }

    // ---- fold --------------------------------------------------------------

    static void fold(const std::string& text, Folded& r) {
        std::vector<uint32_t> cps;
        decodeUtf8(text, cps, nullptr);
        foldCps(cps, r);
    }

    static void foldCps(const std::vector<uint32_t>& cps, Folded& r) {
        int tokenId = -1;
        bool boundary = true;
        uint32_t lastLetter = 0; // last non-sentinel char — see the mark rule
        int markRun = 0;    // consecutive identical digit markers at the tail

        for (size_t i = 0; i < cps.size(); i++) {
            uint32_t c = cps[i];

            bool silent = std::binary_search(kSilent,
                kSilent + count(kSilent), c);
            if (silent)
                continue;

            if (inRanges(kMarkRanges, count(kMarkRanges) / 2, c)) {
                // Combining marks are essential vowels/tones in Thai,
                // Devanagari, Arabic — kept after a non-ASCII base inside
                // the same token; sentinels don't reset the base, so a mark
                // split from its letter by a separator still survives.
                // After ASCII they are zalgo noise.
                if (!boundary && !r.text.empty() && lastLetter > 0x7F) {
                    if (r.text.back() == c) {
                        r.runEnd.back() = (int) i;
                        r.run.back()++;
                    } else {
                        r.text.push_back(c);
                        r.map.push_back((int) i);
                        r.runEnd.push_back((int) i);
                        r.run.push_back(1);
                        r.token.push_back(tokenId);
                        lastLetter = c;
                    }
                }
                continue;
            }

            std::vector<uint32_t> piece;
            emitFolded(c, piece);
            if (piece.empty()) {
                boundary = true;
                continue;
            }

            if (boundary) {
                tokenId++;
                r.tokenStart.push_back((int) r.text.size());
                boundary = false;
            }

            for (uint32_t e : piece) {
                // Runs collapse inside a token only — "ww ee ee dd" still
                // becomes "w e e d". Digit markers stay separate so "d11d0"
                // can feed different slots ('i' then 'l') — except past
                // three in a row, where a long digit tail would make the
                // bounded matcher enumerate exponentially.
                bool isMark = e >= DIGIT_BASE && e < DIGIT_BASE + 10;
                if (!r.text.empty() && r.text.back() == e
                        && r.token.back() == tokenId) {
                    if (!isMark || markRun >= 3) {
                        r.runEnd.back() = (int) i;
                        r.run.back()++;
                        continue;
                    }
                    markRun++;
                } else {
                    markRun = isMark ? 1 : 0;
                }
                r.text.push_back(e);
                r.map.push_back((int) i);
                r.runEnd.push_back((int) i);
                r.run.push_back(1);
                r.token.push_back(tokenId);
                if (e != SENTINEL)
                    lastLetter = e;
            }
        }
    }

    // True when the sentinel's token holds any non-ASCII letter — only
    // then can a wildcard plausibly stand in for a combining mark.
    static bool tokenWide(const Folded& f, int pos) {
        int t = f.token[pos];
        for (int i = pos; i >= 0 && f.token[i] == t; i--)
            if (f.text[i] > 0x7F)
                return true;
        for (size_t i = (size_t) pos + 1; i < f.text.size() && f.token[i] == t; i++)
            if (f.text[i] > 0x7F)
                return true;
        return false;
    }

    // ---- binary reader ------------------------------------------------------

    struct Reader {
        const unsigned char* d;
        size_t n;
        size_t off = 0;
        uint32_t u16() {
            uint32_t v = d[off] | (d[off + 1] << 8);
            off += 2;
            return v;
        }
        uint32_t u32() {
            uint32_t v = d[off] | (d[off + 1] << 8) | (d[off + 2] << 16)
                         | ((uint32_t) d[off + 3] << 24);
            off += 4;
            return v;
        }
        uint64_t u64() {
            uint64_t v = 0;
            for (int i = 0; i < 8; i++)
                v |= (uint64_t) d[off + i] << (8 * i);
            off += 8;
            return v;
        }
    };

    static uint64_t fnvByte(uint64_t h, uint8_t b) {
        return (h ^ b) * FNV_PRIME;
    }

    static uint64_t fnvCp(uint64_t h, uint32_t cp) {
        std::string tmp;
        appendUtf8(tmp, cp);
        for (unsigned char b : tmp)
            h = fnvByte(h, b);
        return h;
    }

    static uint64_t fnvRange(uint64_t h, const Folded& f, int s, int e) {
        for (int i = s; i < e; i++)
            h = fnvCp(h, f.text[i]);
        return h;
    }

    static void ensureLoaded() {
        State& st = state();
        if (st.loaded)
            return;
        st.loaded = true;
        st.severe.clear();
        st.wordSub.clear();
        st.wordExact.clear();
        st.allow.clear();
        st.severeMax = st.wordSubMax = st.wordExactMax = 0;
        st.alphabet.clear();
        st.wildAscii.clear();
        st.wildMark.clear();
        st.inAlphabet.clear();
        st.seed = FNV_OFFSET;

        try {
            loadTable(kModerationDat, kModerationDatLen);
        } catch (...) {
            // Corrupt data disables the local filter — the server-side
            // check still applies on register/rename.
        }
    }

    static void loadTable(const unsigned char* d, size_t dlen) {
        if (dlen < 26 || d[0] != 'M' || d[1] != 'F' || d[2] != 'I' || d[3] != 'L')
            throw std::runtime_error("bad magic");

        State& st = state();
        Reader r{d, dlen, 4};
        uint32_t version = r.u16();
        r.u16(); // flags, reserved
        if (version != 1)
            throw std::runtime_error("unsupported version");

        for (int i = 0; i < 16; i++)
            st.seed = fnvByte(st.seed, d[r.off + i]);
        r.off += 16;

        uint32_t alen = r.u16();
        std::vector<uint32_t> alpha;
        decodeUtf8(std::string((const char*) d + r.off, alen), alpha, nullptr);
        r.off += alen;
        st.alphabet = alpha;
        for (uint32_t c : alpha)
            st.inAlphabet.insert(c);

        // Sentinel slots enumerate ASCII letters, plus combining marks
        // only when the token holds non-ASCII letters — marks cover
        // separators dropped inside a grapheme cluster, and keeping them
        // out of Latin tokens holds the branch factor (and the variant
        // budget) small for names like "a_b_c".
        for (uint32_t c : alpha) {
            if (c < 0x80)
                st.wildAscii.push_back(c);
            else if (inRanges(kMarkRanges, count(kMarkRanges) / 2, c))
                st.wildMark.push_back(c);
        }

        loadSection(r, st.severe, st.severeMax);
        loadSection(r, st.wordSub, st.wordSubMax);
        loadSection(r, st.wordExact, st.wordExactMax);

        uint32_t nAllow = r.u32();
        for (uint32_t i = 0; i < nAllow; i++) {
            uint64_t h = r.u64();
            int n = r.d[r.off++];
            std::vector<uint8_t> runs(r.d + r.off, r.d + r.off + n);
            r.off += n;
            st.allow.emplace(h, std::move(runs));
        }
    }

    // An allow hit needs the same folded text AND enough input run per
    // position: "shiite" is safe, but the same fold with a single 'i'
    // ("shite") is a listed word, not an allow hit.
    static bool allowOk(const State& st, const Folded& f, int s, int e) {
        auto it = st.allow.find(fnvRange(st.seed, f, s, e));
        if (it == st.allow.end() || (int) it->second.size() != e - s)
            return false;
        const std::vector<uint8_t>& need = it->second;
        for (int i = s; i < e; i++)
            if (f.run[i] < (need[i - s] & 0x3F))
                return false;
        return true;
    }

    static void loadSection(Reader& r,
            std::unordered_map<uint64_t, std::vector<uint8_t>>& map,
            int& maxLen) {
        uint32_t cnt = r.u32();
        for (uint32_t i = 0; i < cnt; i++) {
            uint64_t h = r.u64();
            int n = r.d[r.off++];
            if (n == 0 || n > 64) {   // beyond scratch arrays
                r.off += n;
                continue;
            }
            std::vector<uint8_t> runs(r.d + r.off, r.d + r.off + n);
            r.off += n;
            map.emplace(h, std::move(runs));
            if (n > maxLen)
                maxLen = n;
        }
    }

    // ---- hash-scan matcher ------------------------------------------------
    //
    // Matching enumerates the letter strings the folded input can resolve
    // to: a literal position yields its char, a digit marker every letter
    // in its class, a sentinel every alphabet letter (wildcard slots satisfy
    // only run-1 pattern positions). A slot consumes every consecutive input
    // position that can carry its letter, summing runs. A per-start budget
    // caps variant storms; overflow just ends the local check.

    static bool hasLetter(const Folded& f, const uint8_t* kind,
                          const std::vector<std::string>& cls,
                          int j, uint32_t l) {
        if (kind[j] == 0)
            return f.text[j] == l;
        if (kind[j] == 1)
            return cls[j].find((char) l) != std::string::npos;
        return false;
    }

    static bool runsOk(State& st, int slots, const std::vector<uint8_t>& need) {
        for (int k = 0; k < slots; k++) {
            int run = need[k] & 0x3F;
            if (st.slotKind[k] == 2) {
                if (run > 1)
                    return false;
            } else if ((need[k] & 0x80) && st.slotKind[k] == 0) {
                // A digit-derived slot of a listed entry can't be filled by
                // a real letter — "4 2 0" stays digit-only.
                return false;
            } else if (st.slotRun[k] < run) {
                return false;
            }
        }
        return true;
    }

    // A match needs grounded anchors — input positions that are not
    // wildcards: three when a wildcard substitutes, two when separators
    // were merely skipped, one otherwise. Digit markers count — a marker
    // resolves to a small fixed class, so "455" spelling a short entry is
    // deliberate, not a guess. With fewer, sentinels alone could spell a
    // short entry: "-9-" resolves on wildcards without touching a letter.
    static bool literalOk(State& st, const uint8_t* kind, int start, int end,
            int slots, const std::vector<uint8_t>& need) {
        int lit = 0;
        int ground = 0;
        int lastLit = -1;
        bool anySent = false;
        for (int i = start; i < end; i++) {
            if (kind[i] == 0) {
                lit++;
                ground++;
                lastLit = i;
            } else if (kind[i] == 2) {
                anySent = true;
            } else {
                ground++;
            }
        }
        // A separator anywhere before the last letter is woven evasion,
        // so the "-digits" tail can no longer pass as name glue
        // ("k_u_n_7"). A bare digit inside the head is NOT enough
        // ("e2e-99" stays clean — timestamp handles carry digits
        // without any separator trickery).
        bool sentBeforeLit = false;
        for (int i = start; i < lastLit; i++) {
            if (kind[i] == 2) {
                sentBeforeLit = true;
                break;
            }
        }
        bool anyWild = false;
        bool allWildMark = true;
        bool allDig = true;
        for (int k = 0; k < slots; k++) {
            if (st.slotKind[k] == 2) {
                anyWild = true;
                if ((need[k] & 0x40) == 0)
                    allWildMark = false;
            }
            if ((need[k] & 0x80) == 0)
                allDig = false;
        }
        // An entry whose slots are all digit-derived is already exact
        // when spelled in digits, so glue between them still flags.
        if (!(lit == 0 && allDig)) {
            if (ground < (anyWild ? (allWildMark ? 2 : 3) : anySent ? 2 : 1))
                return false;
            // "-digits" after the last real letter is name glue, not
            // evasion ("Xena-21" can't spell a word) — but only when the
            // separators all precede the digits AND the word used
            // no digit substitution. A separator woven between digits
            // ("5_h_1_7") or anywhere earlier in the word is evasion,
            // so the tail rule lifts.
            bool glue = false, interleaved = false, seenMarker = false;
            for (int i = lastLit + 1; i < end; i++) {
                if (kind[i] == 1) {
                    seenMarker = true;
                } else if (kind[i] == 2) {
                    if (seenMarker) {
                        interleaved = true;
                        break;
                    }
                    glue = true;
                }
            }
            if (glue && !interleaved && !sentBeforeLit)
                return false;
        }
        return true;
    }

    static void enumerate(State& st, const Folded& f, int pos,
            const uint8_t* kind, const std::vector<std::string>& cls,
            int slot, uint64_t h, int start,
            const std::unordered_map<uint64_t, std::vector<uint8_t>>& map,
            int maxLen, int requireEnd, std::vector<Span>& spans) {
        int n = (int) f.text.size();

        if (slot > 0 && (requireEnd < 0 || pos == requireEnd)
                && kind[start] != 2 && kind[pos - 1] != 2) {
            auto it = map.find(h);
            if (it != map.end() && (int) it->second.size() == slot
                    && runsOk(st, slot, it->second)
                    && literalOk(st, kind, start, pos, slot, it->second)) {
                spans.push_back({start, pos});
            }
        }

        if (st.overflow || slot == maxLen || pos >= n)
            return;

        if (--st.budget < 0) {
            st.overflow = true;
            return;
        }

        if (kind[pos] == 2) {          // sentinel: junk to skip, or a wildcard slot
            // Junk first — "w_e_e_d" spells its word with the '_'s skipped,
            // and the cheap path must run before the 26-way wildcard
            // enumeration drains the variant budget.
            enumerate(st, f, pos + 1, kind, cls, slot, h,
                      start, map, maxLen, requireEnd, spans);
            // Substitution only onto a literal-bounded slot: the previous
            // slot must have ended on a real letter (the right edge is
            // enforced when the next slot emits — markers can't follow a
            // substitute).
            if (st.subs && slot > 0 && st.slotKind[slot - 1] == 0) {
                size_t nWild = st.wildAscii.size();
                if (tokenWide(f, pos))
                    nWild += st.wildMark.size();
                for (size_t a = 0; a < nWild; a++) {
                    uint32_t w = a < st.wildAscii.size()
                        ? st.wildAscii[a] : st.wildMark[a - st.wildAscii.size()];
                    st.slotRun[slot] = 0;
                    st.slotKind[slot] = 2;
                    enumerate(st, f, pos + 1, kind, cls, slot + 1, fnvCp(h, w),
                              start, map, maxLen, requireEnd, spans);
                    if (st.overflow)
                        return;
                }
            }
            return;
        }

        if (kind[pos] == 0) {          // literal letter
            uint32_t l = f.text[pos];
            if (st.inAlphabet.find(l) == st.inAlphabet.end())
                return;
            // Emit at every reachable run end, longest first: sentinels
            // inside a run are junk ("w-e-e-d" counts the two e's) while
            // the cut-before-sentinel emission leaves it free to
            // substitute the next slot ("v*pe"). Longest-first keeps deep
            // runs ahead of the wildcard substitution storm when the
            // variant budget is tight.
            int j = pos, sum = 0, cnt = 0, b = slot * 64;
            while (j < n && cnt < 64) {
                if (kind[j] == 2) {
                    j++;
                    continue;
                }
                if (!hasLetter(f, kind, cls, j, l))
                    break;
                sum += f.run[j];
                j++;
                st.emitEnd[b + cnt] = j;
                st.emitSum[b + cnt] = sum;
                st.emitKind[b + cnt] = kind[j - 1];
                cnt++;
            }
            for (int k = cnt - 1; k >= 0; k--) {
                st.slotRun[slot] = st.emitSum[b + k];
                st.slotKind[slot] = st.emitKind[b + k];
                enumerate(st, f, st.emitEnd[b + k], kind, cls, slot + 1,
                          fnvCp(h, l), start, map, maxLen, requireEnd, spans);
                if (st.overflow)
                    return;
            }
            return;
        }

        // A digit marker can't be the next slot's first position after a
        // substitute — that would make the substituted edge a guess both
        // ways. Separators interleaved with markers mid-word are legal
        // ("w_3_3_d"); a glued "-digits" tail is rejected by the hit
        // check instead.
        if (slot > 0 && st.slotKind[slot - 1] == 2)
            return;

        for (char lc : cls[pos]) {     // digit marker → every class letter
            uint32_t l = (unsigned char) lc;
            if (st.inAlphabet.find(l) == st.inAlphabet.end())
                continue;
            int j = pos, sum = 0, cnt = 0, b = slot * 64;
            while (j < n && cnt < 64) {
                if (kind[j] == 2) {
                    j++;
                    continue;
                }
                if (!hasLetter(f, kind, cls, j, l))
                    break;
                sum += f.run[j];
                j++;
                st.emitEnd[b + cnt] = j;
                st.emitSum[b + cnt] = sum;
                st.emitKind[b + cnt] = kind[j - 1];
                cnt++;
            }
            for (int k = cnt - 1; k >= 0; k--) {
                st.slotRun[slot] = st.emitSum[b + k];
                st.slotKind[slot] = st.emitKind[b + k];
                enumerate(st, f, st.emitEnd[b + k], kind, cls, slot + 1,
                          fnvCp(h, l), start, map, maxLen, requireEnd, spans);
                if (st.overflow)
                    return;
            }
        }
    }

    static void scanPos(State& st, const Folded& f, int start,
            const uint8_t* kind, const std::vector<std::string>& cls,
            const std::unordered_map<uint64_t, std::vector<uint8_t>>& map,
            int maxLen, int requireEnd, std::vector<Span>& spans) {
        // A match can't start on a wildcard — "D_ape" flagging "vape" on a
        // leading '_' is a false positive, and enumerating from a sentinel
        // burns the whole variant budget on a guaranteed no-hit.
        if (maxLen == 0 || kind[start] == 2)
            return;

        // Pass 1 is skip-only (no wildcard substitutions) — it is cheap,
        // so real skip-evasions resolve before the bounded enumeration can
        // exhaust the variant budget on substitution storms.
        st.subs = false;
        st.budget = VARIANT_BUDGET;
        st.overflow = false;
        enumerate(st, f, start, kind, cls, 0, st.seed, start,
                  map, maxLen, requireEnd, spans);

        st.subs = true;
        st.budget = VARIANT_BUDGET;
        st.overflow = false;
        enumerate(st, f, start, kind, cls, 0, st.seed, start,
                  map, maxLen, requireEnd, spans);

        // On budget exhaustion the local check just stops — enumerating
        // can't prove a match either way and "u_n_i_t_e_d"-shaped names are
        // legitimate. The authoritative server check stays unbounded.
    }

    static void findMatches(Folded& f, std::vector<Span>& out) {
        ensureLoaded();
        State& st = state();

        int n = (int) f.text.size();
        if (n == 0)
            return;

        // Per-position kind: 0 literal letter, 1 digit marker, 2 sentinel.
        std::vector<uint8_t> kind(n, 0);
        std::vector<std::string> cls(n);
        for (int i = 0; i < n; i++) {
            uint32_t c = f.text[i];
            if (c == SENTINEL) {
                kind[i] = 2;
            } else if (c >= DIGIT_BASE && c < DIGIT_BASE + 10) {
                kind[i] = 1;
                static const char* kDigitCls[10] = {
                    "o", "il", "tzs", "eza", "a", "s", "bg", "thv", "bpv", "gds"
                };
                cls[i] = kDigitCls[c - DIGIT_BASE];
            }
        }

        std::vector<char> allowTok(f.tokenStart.size(), 0);
        for (size_t t = 0; t < f.tokenStart.size(); t++) {
            int s = f.tokenStart[t];
            int e = t + 1 < f.tokenStart.size() ? f.tokenStart[t + 1] : n;
            allowTok[t] = allowOk(st, f, s, e) ? 1 : 0;
        }

        std::vector<Span> spans;
        if (!allowOk(st, f, 0, n)) {
            for (int p = 0; p < n; p++) {
                if (kind[p] == 0
                        && st.inAlphabet.find(f.text[p]) == st.inAlphabet.end())
                    continue;
                scanPos(st, f, p, kind.data(), cls, st.severe, st.severeMax,
                        -1, spans);
            }

            // Long words start matches only at token boundaries.
            for (size_t t = 0; t < f.tokenStart.size(); t++) {
                int s = f.tokenStart[t];
                int e = t + 1 < f.tokenStart.size() ? f.tokenStart[t + 1] : n;
                for (int p = s; p < e; p++) {
                    if (p != s && allowTok[t])
                        continue;
                    if (kind[p] == 0
                            && st.inAlphabet.find(f.text[p]) == st.inAlphabet.end())
                        continue;
                    scanPos(st, f, p, kind.data(), cls, st.wordSub,
                            st.wordSubMax, -1, spans);
                }
            }

            // Short words flag only when the whole collapsed string equals
            // the word — "n o o b" -> "noob" flags, "noble" does not.
            scanPos(st, f, 0, kind.data(), cls, st.wordExact, st.wordExactMax,
                    n, spans);
        }

        // Short words inside a non-allowlisted token — skipped entirely for
        // allowlisted tokens (keeps "seaweed" clean despite "weed").
        for (size_t t = 0; t < f.tokenStart.size(); t++) {
            if (allowTok[t])
                continue;
            int start = f.tokenStart[t];
            int end = t + 1 < f.tokenStart.size() ? f.tokenStart[t + 1] : n;
            scanPos(st, f, start, kind.data(), cls, st.wordExact,
                    st.wordExactMax, end, spans);
        }

        // A match fully inside an allowlisted token is safe — e.g. "weed"
        // inside "seaweed". Tokens span separators and digits ("grape-0" is
        // one token), so the same rescue applies to the containing literal
        // letter run.
        std::vector<Span> kept;
        for (const Span& sp : spans) {
            if (f.token[sp.start] == f.token[sp.end - 1]
                    && allowTok[f.token[sp.start]]) {
                continue;
            }
            if (kind[sp.start] == 0) {
                int rs = sp.start, re = sp.start + 1;
                while (rs > 0 && kind[rs - 1] == 0) rs--;
                while (re < n && kind[re] == 0) re++;
                if (sp.end <= re && allowOk(st, f, rs, re)) {
                    continue;
                }
            }
            kept.push_back(sp);
        }

        // Mask the whole token(s) a match touches — leaving "W***D" behind
        // defeats the purpose.
        for (Span& sp : kept) {
            int first = f.token[sp.start];
            int last = f.token[sp.end - 1];
            sp.start = f.tokenStart[first];
            sp.end = last + 1 < (int) f.tokenStart.size()
                         ? f.tokenStart[last + 1] : n;
        }

        out.swap(kept);
    }
};

} // namespace moderation
} // namespace multiplayer
} // namespace michitai
