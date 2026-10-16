package com.michitai.multiplayer.moderation;

import java.text.Normalizer;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Base64;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

/**
 * Local fast-path profanity screen — hash-only wordlist.
 *
 * Mirrors the PHP reference (src/text_filter.php) and the .NET TextFilter.
 * The embedded list ({@link ModerationData#BASE64}) is an MFIL v1 blob built
 * by scripts/build_moderation_dat.php — salted FNV-1a-64 hashes plus
 * per-position run requirements, no readable words. Local checks give
 * instant feedback; the server stays authoritative (register/rename and
 * {@link Moderation#censor}).
 *
 * Matching is evasion-resistant: lowercase + NFKC, leet/diacritic/Cyrillic/
 * Greek confusables folded to Latin, combining marks and separators
 * stripped, same-letter runs collapsed, digits resolve as ambiguity
 * classes ('1' -> i/l, '8' -> b/p/v), wildcards ('*', '_', ...) act as
 * "any one letter".
 *
 * Tiers:
 *   severe   — substring over the whole folded string ("w e e d" flags)
 *   words>=5 — substring inside a token
 *   words<5  — whole-token equality only ("noob" flags, "noble" doesn't)
 *   allow    — folded-string exemptions ("grape", "therapist", ...)
 */
public final class TextFilter {

    /** Display-name length cap — matches the virtual keyboard. */
    public static final int MAX_NAME_LENGTH = 16;

    private static final char SENTINEL = '\u0001'; // wildcard: matches any one letter
    private static final char MASK_CHAR = '*';
    private static final char DIGIT_BASE = '\u0010';
    private static final int VARIANT_BUDGET = 4096;

    private static final long FNV_OFFSET = 0xcbf29ce484222325L;
    private static final long FNV_PRIME  = 0x100000001b3L;

    // Confusable/leet map — applied to the lowercased NFKC form of each
    // input char AND to word-list entries, so both sides fold identically.
    private static final Map<Character, String> SUBST = new HashMap<>();
    private static final Set<Character> WILDCARD = new HashSet<>();
    private static final Set<Character> SILENT = new HashSet<>();
    private static final Map<Character, String> DIGIT_CLASS = new HashMap<>();

    static {
        // leet / symbol lookalikes (digits fold through DIGIT_CLASS markers)
        String[][] leet = {
            {"@", "a"}, {"$", "s"}, {"€", "e"}, {"£", "l"}, {"¥", "y"},
            {"(", "c"},
        };
        for (String[] kv : leet) SUBST.put(kv[0].charAt(0), kv[1]);

        // diacritic folds — "Scheiße", "baisé", "ş"-evasions
        String[][] dia = {
            {"à", "a"}, {"á", "a"}, {"â", "a"}, {"ã", "a"}, {"ä", "a"}, {"å", "a"},
            {"è", "e"}, {"é", "e"}, {"ê", "e"}, {"ë", "e"}, {"ě", "e"},
            {"ì", "i"}, {"í", "i"}, {"î", "i"}, {"ï", "i"},
            {"ò", "o"}, {"ó", "o"}, {"ô", "o"}, {"õ", "o"}, {"ö", "o"}, {"ø", "o"},
            {"ù", "u"}, {"ú", "u"}, {"û", "u"}, {"ü", "u"}, {"ű", "u"},
            {"ý", "y"}, {"ÿ", "y"},
            {"ç", "c"}, {"ć", "c"}, {"č", "c"},
            {"ñ", "n"}, {"ń", "n"}, {"ň", "n"},
            {"š", "s"}, {"ś", "s"}, {"ş", "s"}, {"ș", "s"},
            {"ž", "z"}, {"ź", "z"}, {"ż", "z"},
            {"ğ", "g"}, {"ı", "i"}, {"ł", "l"}, {"đ", "d"}, {"ð", "d"},
            {"ţ", "t"}, {"ț", "t"}, {"ť", "t"}, {"þ", "th"}, {"ß", "ss"},
            {"æ", "ae"}, {"œ", "oe"}, {"ů", "u"}, {"ř", "r"},
            {"ď", "d"}, {"ę", "e"}, {"ą", "a"}, {"ė", "e"}, {"ų", "u"}, {"ū", "u"},
            {"ī", "i"}, {"ĺ", "l"}, {"ļ", "l"}, {"ő", "o"}, {"ă", "a"},
            {"ĉ", "c"}, {"ŭ", "u"},
        };
        for (String[] kv : dia) SUBST.put(kv[0].charAt(0), kv[1]);

        // Cyrillic → Latin phonetic — catches "наркотик"/"narkotik" cross-script
        // evasion since word-list entries fold the same way.
        String[][] cyr = {
            {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
            {"е", "e"}, {"ё", "e"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"},
            {"й", "i"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
            {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
            {"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "c"}, {"ч", "ch"},
            {"ш", "sh"}, {"щ", "sh"}, {"ы", "y"}, {"э", "e"}, {"ю", "yu"},
            {"я", "ya"}, {"і", "i"}, {"ї", "yi"}, {"є", "e"}, {"ґ", "g"},
            {"ў", "u"},
        };
        for (String[] kv : cyr) SUBST.put(kv[0].charAt(0), kv[1]);

        // Greek → Latin phonetic
        String[][] gre = {
            {"α", "a"}, {"β", "v"}, {"γ", "g"}, {"δ", "d"}, {"ε", "e"},
            {"ζ", "z"}, {"η", "i"}, {"θ", "th"}, {"ι", "i"}, {"κ", "k"},
            {"λ", "l"}, {"μ", "m"}, {"ν", "n"}, {"ξ", "x"}, {"ο", "o"},
            {"π", "p"}, {"ρ", "r"}, {"σ", "s"}, {"ς", "s"}, {"τ", "t"},
            {"υ", "u"}, {"φ", "f"}, {"χ", "h"}, {"ψ", "ps"}, {"ω", "o"},
        };
        for (String[] kv : gre) SUBST.put(kv[0].charAt(0), kv[1]);

        for (char c : "*?#%&_-~!+$|•°=.".toCharArray()) WILDCARD.add(c);
        for (char c : "'’‘`´ъь\"".toCharArray()) SILENT.add(c);

        String[][] digits = {
            {"0", "o"}, {"1", "il"}, {"2", "tzs"}, {"3", "eza"}, {"4", "a"},
            {"5", "s"}, {"6", "bg"}, {"7", "thv"}, {"8", "bpv"}, {"9", "gds"},
        };
        for (String[] kv : digits) DIGIT_CLASS.put(kv[0].charAt(0), kv[1]);
    }

    private static boolean isMarker(char c) {
        return c >= DIGIT_BASE && c < DIGIT_BASE + 10;
    }

    // ---- loaded state --------------------------------------------------

    private static boolean loaded;
    private static final Map<Long, byte[]> severe = new HashMap<>();
    private static final Map<Long, byte[]> wordSub = new HashMap<>();
    private static final Map<Long, byte[]> wordExact = new HashMap<>();
    private static final Map<Long, byte[]> allow = new HashMap<>();
    private static int severeMax, wordSubMax, wordExactMax;
    private static long seed = FNV_OFFSET;
    private static char[] alphabet = new char[0];
    private static char[] wildAscii = new char[0];
    private static char[] wildMark = new char[0];
    private static final boolean[] inAlphabet = new boolean[1 << 16];

    /** Normalized form: separator-free, run-collapsed, with sentinels. */
    private static final class Folded {
        String text = "";
        int[] map = new int[0];        // pos -> original index (first of run)
        int[] runEnd = new int[0];     // pos -> original index (last of run)
        int[] run = new int[0];        // pos -> collapsed repeat count
        int[] token = new int[0];      // pos -> token id
        int[] tokenStart = new int[0]; // token id -> first pos
    }

    private static final class Span {
        int start, end; // folded coordinates, end exclusive
        Span(int s, int e) { start = s; end = e; }
    }

    private TextFilter() {}

    // ---- public API -----------------------------------------------------

    /** True when the text contains no listed word after folding. */
    public static synchronized boolean isClean(String text) {
        return findMatches(fold(text)).isEmpty();
    }

    /** Entry-side validation for typed names: length + profanity. */
    public static boolean isNameAllowed(String name) {
        if (name == null)
            return false;
        String trimmed = name.trim();
        // The server accepts 2..50 chars; the keyboard caps at MAX_NAME_LENGTH.
        if (trimmed.length() < 2 || trimmed.length() > 50)
            return false;
        return isClean(trimmed);
    }

    /**
     * Masks listed words with '*' and strips markup ('<' '>') plus
     * invisible/control characters. Use for every user-provided string
     * before it reaches a label.
     */
    public static synchronized String filter(String text) {
        if (text == null || text.isEmpty())
            return text;

        Folded folded = fold(text);
        List<Span> matches = findMatches(folded);

        boolean[] mask = new boolean[text.length()];
        for (Span span : matches)
            for (int i = span.start; i < span.end; i++)
                for (int j = folded.map[i]; j <= folded.runEnd[i]; j++)
                    mask[j] = true;

        StringBuilder sb = new StringBuilder(text.length());
        for (int i = 0; i < text.length(); i++) {
            char c = text.charAt(i);

            // Drop whole <...> runs so names cannot inject markup tags.
            if (c == '<') {
                int close = text.indexOf('>', i + 1);
                if (close > i) {
                    i = close;
                    continue;
                }
            }

            if (c == '>')
                continue;

            int cat = Character.getType(c);
            if (cat == Character.CONTROL || cat == Character.FORMAT
                    || cat == Character.SURROGATE || cat == Character.PRIVATE_USE
                    || cat == Character.UNASSIGNED) {
                continue;
            }

            sb.append(mask[i] ? MASK_CHAR : c);
        }
        return sb.toString();
    }

    /**
     * filter() for a display name; falls back to a neutral name when the
     * mask leaves nothing readable (e.g. the whole name is a slur).
     */
    public static String filterName(String name, String fallback) {
        String filtered = filter(name);
        if (filtered == null || filtered.isEmpty())
            return fallback;
        for (int i = 0; i < filtered.length(); i++)
            if (Character.isLetterOrDigit(filtered.charAt(i)))
                return filtered;
        return fallback;
    }

    /**
     * Turns an external name (Steam persona) into a valid in-game name —
     * collapsed whitespace, length cap, letters/digits plus '_'/'-'.
     * Returns null when nothing usable remains or the result fails the
     * filter; the caller then keeps the server-assigned default name.
     */
    public static String sanitizeImportedName(String raw) {
        if (raw == null || raw.isEmpty())
            return null;

        StringBuilder sb = new StringBuilder(raw.length());
        boolean pendingSpace = false;
        for (int i = 0; i < raw.trim().length(); i++) {
            char c = raw.trim().charAt(i);
            if (Character.isWhitespace(c)) {
                pendingSpace = true;
                continue;
            }
            if (!Character.isLetterOrDigit(c) && c != '_' && c != '-')
                continue;
            if (pendingSpace && sb.length() > 0)
                sb.append(' ');
            pendingSpace = false;
            sb.append(c);
        }

        String name = sb.toString();
        if (name.length() > MAX_NAME_LENGTH)
            name = name.substring(0, MAX_NAME_LENGTH).replaceAll("\\s+$", "");
        return isNameAllowed(name) ? name : null;
    }

    /** Re-reads the embedded word list. */
    public static synchronized void reload() {
        loaded = false;
        ensureLoaded();
    }

    // ---- binary reader ---------------------------------------------------

    private static int u16(byte[] d, int[] off) {
        int v = (d[off[0]] & 0xFF) | ((d[off[0] + 1] & 0xFF) << 8);
        off[0] += 2;
        return v;
    }

    private static long u32(byte[] d, int[] off) {
        long v = 0;
        for (int i = 0; i < 4; i++)
            v |= (long) (d[off[0] + i] & 0xFF) << (8 * i);
        off[0] += 4;
        return v;
    }

    private static long u64(byte[] d, int[] off) {
        long v = 0;
        for (int i = 0; i < 8; i++)
            v |= (long) (d[off[0] + i] & 0xFF) << (8 * i);
        off[0] += 8;
        return v;
    }

    private static void ensureLoaded() {
        if (loaded)
            return;
        loaded = true;
        severe.clear();
        wordSub.clear();
        wordExact.clear();
        allow.clear();
        severeMax = wordSubMax = wordExactMax = 0;
        alphabet = new char[0];
        Arrays.fill(inAlphabet, false);
        seed = FNV_OFFSET;

        try {
            loadTable(Base64.getDecoder().decode(ModerationData.BASE64));
        } catch (Exception e) {
            // Corrupt/missing data disables the local filter — the
            // server-side check still applies on register/rename.
        }
    }

    private static void loadTable(byte[] d) throws IllegalArgumentException {
        if (d.length < 26 || d[0] != 'M' || d[1] != 'F' || d[2] != 'I' || d[3] != 'L')
            throw new IllegalArgumentException("bad magic");

        int[] off = {4};
        int version = u16(d, off);
        u16(d, off); // flags, reserved
        if (version != 1)
            throw new IllegalArgumentException("unsupported version " + version);

        for (int i = 0; i < 16; i++)
            seed = fnvByte(seed, d[off[0] + i]);
        off[0] += 16;

        int alen = u16(d, off);
        alphabet = new String(d, off[0], alen, java.nio.charset.StandardCharsets.UTF_8).toCharArray();
        off[0] += alen;
        for (char c : alphabet)
            inAlphabet[c] = true;

        // Sentinel slots ("any one letter") enumerate ASCII letters, plus
        // combining marks only when the token holds non-ASCII letters —
        // marks cover separators dropped inside a grapheme cluster, and
        // keeping them out of Latin tokens holds the branch factor (and
        // the variant budget) small for names like "a_b_c".
        List<Character> ascii = new ArrayList<>();
        List<Character> marks = new ArrayList<>();
        for (char c : alphabet) {
            if (c < '')
                ascii.add(c);
            else if (Character.getType(c) == Character.NON_SPACING_MARK
                    || Character.getType(c) == Character.COMBINING_SPACING_MARK)
                marks.add(c);
        }
        wildAscii = new char[ascii.size()];
        for (int i = 0; i < ascii.size(); i++)
            wildAscii[i] = ascii.get(i);
        wildMark = new char[marks.size()];
        for (int i = 0; i < marks.size(); i++)
            wildMark[i] = marks.get(i);

        int[] severeMaxBox = {severeMax};
        loadSection(d, off, severe, severeMaxBox);
        severeMax = severeMaxBox[0];
        int[] wordSubMaxBox = {wordSubMax};
        loadSection(d, off, wordSub, wordSubMaxBox);
        wordSubMax = wordSubMaxBox[0];
        int[] wordExactMaxBox = {wordExactMax};
        loadSection(d, off, wordExact, wordExactMaxBox);
        wordExactMax = wordExactMaxBox[0];

        long nAllow = u32(d, off);
        for (long i = 0; i < nAllow; i++) {
            long h = u64(d, off);
            int ns = d[off[0]++] & 0xFF;
            byte[] runs = Arrays.copyOfRange(d, off[0], off[0] + ns);
            off[0] += ns;
            allow.put(h, runs);
        }
    }

    // An allow hit needs the same folded text AND enough input run per
    // position: "shiite" is safe, but the same fold with a single 'i'
    // ("shite") is a listed word, not an allow hit.
    private static boolean allowOk(Folded f, int s, int e) {
        byte[] need = allow.get(fnvString(seed, f.text.substring(s, e)));
        if (need == null || need.length != e - s)
            return false;
        for (int i = s; i < e; i++) {
            if (f.run[i] < (need[i - s] & 0x3F))
                return false;
        }
        return true;
    }

    private static void loadSection(byte[] d, int[] off,
            Map<Long, byte[]> map, int[] maxLen) {
        long count = u32(d, off);
        for (long i = 0; i < count; i++) {
            long h = u64(d, off);
            int n = d[off[0]++] & 0xFF;
            if (n == 0 || n > slotRun.length) { // beyond scratch arrays
                off[0] += n;
                continue;
            }
            byte[] runs = Arrays.copyOfRange(d, off[0], off[0] + n);
            off[0] += n;
            map.putIfAbsent(h, runs);
            if (n > maxLen[0])
                maxLen[0] = n;
        }
    }

    private static long fnvByte(long h, byte b) {
        return (h ^ (b & 0xFF)) * FNV_PRIME;
    }

    // FNV-1a-64 over the UTF-8 bytes of a folded letter — matches the PHP
    // generator hashing the UTF-8 string bytes directly.
    private static long fnvChar(long h, char c) {
        if (c < '\u0080')
            return fnvByte(h, (byte) c);
        if (c < '\u0800')
            return fnvByte(fnvByte(h, (byte) (0xC0 | (c >> 6))), (byte) (0x80 | (c & 0x3F)));
        return fnvByte(fnvByte(fnvByte(h, (byte) (0xE0 | (c >> 12))),
            (byte) (0x80 | ((c >> 6) & 0x3F))), (byte) (0x80 | (c & 0x3F)));
    }

    private static long fnvString(long h, String s) {
        for (int i = 0; i < s.length(); i++)
            h = fnvChar(h, s.charAt(i));
        return h;
    }

    // ---- fold ------------------------------------------------------------

    private static Folded fold(String text) {
        Folded result = new Folded();
        if (text == null || text.isEmpty())
            return result;

        StringBuilder sb = new StringBuilder(text.length());
        List<Integer> map = new ArrayList<>(text.length());
        List<Integer> runEnd = new ArrayList<>(text.length());
        List<Integer> run = new ArrayList<>(text.length());
        List<Integer> token = new ArrayList<>(text.length());
        List<Integer> tokenStarts = new ArrayList<>();

        StringBuilder piece = new StringBuilder(4);
        int tokenId = -1;
        boolean boundary = true;
        char lastLetter = '\0'; // last non-sentinel char — see the mark rule
        int markRun = 0;    // consecutive identical digit markers at the tail

        for (int i = 0; i < text.length(); i++) {
            char c = text.charAt(i);

            if (SILENT.contains(Character.toLowerCase(c)))
                continue;

            int cat = Character.getType(c);
            if (cat == Character.NON_SPACING_MARK || cat == Character.COMBINING_SPACING_MARK) {
                // Combining marks are essential vowels/tones in Thai,
                // Devanagari, Arabic etc. — keep them after a non-ASCII base
                // inside the same token ("บ้า"); after ASCII output they are
                // zalgo noise and get dropped ("v ä p e" -> "vape").
                if (!boundary && sb.length() > 0 && lastLetter > '\u007F') {
                    char lower = Character.toLowerCase(c);
                    if (sb.charAt(sb.length() - 1) == lower) {
                        runEnd.set(runEnd.size() - 1, i);
                        run.set(run.size() - 1, run.get(run.size() - 1) + 1);
                    } else {
                        sb.append(lower);
                        map.add(i);
                        runEnd.add(i);
                        run.add(1);
                        token.add(tokenId);
                        lastLetter = lower;
                    }
                }
                continue;
            }

            piece.setLength(0);
            emitFolded(c, piece);
            if (piece.length() == 0) {
                boundary = true;
                continue;
            }

            if (boundary) {
                tokenId++;
                tokenStarts.add(sb.length());
                boundary = false;
            }

            for (int p = 0; p < piece.length(); p++) {
                char e = piece.charAt(p);
                // Runs of the same folded letter collapse to one, but only
                // inside a token — "ww ee ee dd" still becomes "w e e d".
                // Digit markers stay separate so "d11d0" can feed
                // different slots ('i' then 'l') — except past three in a
                // row, where a long digit tail would make the bounded
                // matcher enumerate exponentially ("e2e-999…9-1").
                if (sb.length() > 0 && sb.charAt(sb.length() - 1) == e
                        && token.get(token.size() - 1) == tokenId) {
                    if (!isMarker(e) || markRun >= 3) {
                        runEnd.set(runEnd.size() - 1, i);
                        run.set(run.size() - 1, run.get(run.size() - 1) + 1);
                        continue;
                    }
                    markRun++;
                } else {
                    markRun = isMarker(e) ? 1 : 0;
                }
                sb.append(e);
                map.add(i);
                runEnd.add(i);
                run.add(1);
                token.add(tokenId);
                if (e != SENTINEL)
                    lastLetter = e;
            }
        }

        result.text = sb.toString();
        result.map = map.stream().mapToInt(Integer::intValue).toArray();
        result.runEnd = runEnd.stream().mapToInt(Integer::intValue).toArray();
        result.run = run.stream().mapToInt(Integer::intValue).toArray();
        result.token = token.stream().mapToInt(Integer::intValue).toArray();
        result.tokenStart = tokenStarts.stream().mapToInt(Integer::intValue).toArray();
        return result;
    }

    private static void emitFolded(char c, StringBuilder sb) {
        // Single-char compatibility decomposition: fullwidth letters,
        // ligatures (ﬁ -> fi), circled digits, superscripts.
        String norm = Normalizer.normalize(String.valueOf(c), Normalizer.Form.NFKC);
        for (int i = 0; i < norm.length(); i++) {
            char lower = Character.toLowerCase(norm.charAt(i));

            // Leet/lookalike folds win over the wildcard set — '$' means 's'
            // in "pu$$y", only falling back to a wildcard elsewhere.
            String repl = SUBST.get(lower);
            if (repl != null) {
                sb.append(repl);
                continue;
            }
            if (DIGIT_CLASS.containsKey(lower)) {
                sb.append((char) (DIGIT_BASE + lower - '0'));
                continue;
            }
            if (WILDCARD.contains(lower)) {
                sb.append(SENTINEL);
                continue;
            }
            if (Character.isLetterOrDigit(lower)) {
                sb.append(lower);
            }
        }
    }

    // ---- hash-scan matcher ------------------------------------------------
    //
    // The list stores salted FNV-1a-64 hashes of folded letter strings plus
    // per-position run requirements, so matching enumerates the letter
    // strings the folded input can resolve to: a literal position yields
    // its char, a digit marker yields every letter in its class, and a
    // sentinel yields every alphabet letter (a wildcard slot that satisfies
    // only run-1 pattern positions). A pattern slot consumes every
    // consecutive input position that can carry its letter, summing their
    // runs — "w e e d" still matches "weed", "w33d" matches "weed".
    //
    // Enumeration depth is the pattern length; branching only happens at
    // markers/sentinels. A per-start budget caps adversarial variant
    // storms; overflow just ends the local check instead of flagging.

    private static final int[] slotRun = new int[64];
    // Reachable run ends per slot depth, so the longest run emits
    // first (recursion clobbers shared scratch — rows are per slot).
    private static final int[] emitEnd = new int[64 * 64];
    private static final int[] emitSum = new int[64 * 64];
    private static final byte[] emitKind = new byte[64 * 64];
    // How each emitted slot was satisfied: 0 literal letter, 1 digit marker,
    // 2 sentinel substitute. A substitute is legal only between two literal
    // edges of the word — "v*pe" keeps "vape" because the '*' sits between
    // real 'f' and 'c', while "Ora-35" guessing a word on "-3" glue is a
    // false positive.
    private static final byte[] slotKind = new byte[64];
    // True when the sentinel's token holds any non-ASCII letter — only
    // then can a wildcard plausibly stand in for a combining mark.
    private static boolean tokenWide(Folded f, int pos) {
        int t = f.token[pos];
        for (int i = pos; i >= 0 && f.token[i] == t; i--)
            if (f.text.charAt(i) > '\u007F')
                return true;
        for (int i = pos + 1; i < f.text.length() && f.token[i] == t; i++)
            if (f.text.charAt(i) > '\u007F')
                return true;
        return false;
    }

    private static int budget;
    private static boolean overflow;
    private static boolean subs;

    // Does folded position j offer letter l? Letters: equality; markers:
    // class membership; sentinels never join a letter run.
    private static boolean hasLetter(Folded f, byte[] kind, String[] cls, int j, char l) {
        if (kind[j] == 0)
            return f.text.charAt(j) == l;
        if (kind[j] == 1)
            return cls[j].indexOf(l) >= 0;
        return false;
    }

    private static boolean runsOk(int slots, byte[] need) {
        for (int k = 0; k < slots; k++) {
            int run = need[k] & 0x3F;
            if (slotKind[k] == 2) {
                if (run > 1)
                    return false;
            } else if ((need[k] & 0x80) != 0 && slotKind[k] == 0) {
                // A digit-derived slot of a listed entry can't be filled by
                // a real letter — "4 2 0" stays digit-only.
                return false;
            } else if (slotRun[k] < run) {
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
    private static boolean literalOk(byte[] kind, int start, int end,
            int slots, byte[] need) {
        int lit = 0;
        int ground = 0;
        int lastLit = -1;
        boolean anySent = false;
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
        boolean sentBeforeLit = false;
        for (int i = start; i < lastLit; i++) {
            if (kind[i] == 2) {
                sentBeforeLit = true;
                break;
            }
        }
        boolean anyWild = false;
        boolean allWildMark = true;
        boolean allDig = true;
        for (int k = 0; k < slots; k++) {
            if (slotKind[k] == 2) {
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
            boolean glue = false, interleaved = false, seenMarker = false;
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

    private static void enumerate(Folded f, int pos, byte[] kind, String[] cls,
            int slot, long h, int start,
            Map<Long, byte[]> map, int maxLen, int requireEnd,
            List<Span> spans) {
        int n = f.text.length();

        if (slot > 0 && (requireEnd < 0 || pos == requireEnd)
                && kind[start] != 2 && kind[pos - 1] != 2) {
            byte[] need = map.get(h);
            if (need != null && need.length == slot && runsOk(slot, need)
                    && literalOk(kind, start, pos, slot, need))
                spans.add(new Span(start, pos));
        }

        if (overflow || slot == maxLen || pos >= n)
            return;

        if (--budget < 0) {
            overflow = true;
            return;
        }

        if (kind[pos] == 2) {          // sentinel: junk to skip, or a wildcard slot
            // Junk first — "w_e_e_d" spells its word with the '_'s skipped,
            // and the cheap path must run before the 26-way wildcard
            // enumeration drains the variant budget.
            enumerate(f, pos + 1, kind, cls, slot, h,
                    start, map, maxLen, requireEnd, spans);
            // Substitution only onto a literal-bounded slot: the previous
            // slot must have ended on a real letter (the right edge is
            // enforced when the next slot emits — markers can't follow a
            // substitute).
            if (subs && slot > 0 && slotKind[slot - 1] == 0) {
                int nWild = wildAscii.length;
                if (tokenWide(f, pos))
                    nWild += wildMark.length;
                for (int a = 0; a < nWild; a++) {
                    char w = a < wildAscii.length ? wildAscii[a] : wildMark[a - wildAscii.length];
                    slotRun[slot] = 0;
                    slotKind[slot] = 2;
                    enumerate(f, pos + 1, kind, cls, slot + 1, fnvChar(h, w),
                            start, map, maxLen, requireEnd, spans);
                    if (overflow)
                        return;
                }
            }
            return;
        }

        if (kind[pos] == 0) {          // literal letter
            char l = f.text.charAt(pos);
            if (!inAlphabet[l])
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
                emitEnd[b + cnt] = j;
                emitSum[b + cnt] = sum;
                emitKind[b + cnt] = kind[j - 1];
                cnt++;
            }
            for (int k = cnt - 1; k >= 0; k--) {
                slotRun[slot] = emitSum[b + k];
                slotKind[slot] = emitKind[b + k];
                enumerate(f, emitEnd[b + k], kind, cls, slot + 1,
                        fnvChar(h, l), start, map, maxLen, requireEnd, spans);
                if (overflow)
                    return;
            }
            return;
        }

        // A digit marker can't be the next slot's first position after a
        // substitute — that would make the substituted edge a guess both
        // ways. Separators interleaved with markers mid-word are legal
        // ("w_3_3_d"); a glued "-digits" tail is rejected by the hit
        // check instead.
        if (slot > 0 && slotKind[slot - 1] == 2)
            return;

        for (int ci = 0; ci < cls[pos].length(); ci++) { // marker → class letters
            char l = cls[pos].charAt(ci);
            if (!inAlphabet[l])
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
                emitEnd[b + cnt] = j;
                emitSum[b + cnt] = sum;
                emitKind[b + cnt] = kind[j - 1];
                cnt++;
            }
            for (int k = cnt - 1; k >= 0; k--) {
                slotRun[slot] = emitSum[b + k];
                slotKind[slot] = emitKind[b + k];
                enumerate(f, emitEnd[b + k], kind, cls, slot + 1,
                        fnvChar(h, l), start, map, maxLen, requireEnd, spans);
                if (overflow)
                    return;
            }
        }
    }

    private static void scanPos(Folded f, int start, byte[] kind, String[] cls,
            Map<Long, byte[]> map, int maxLen, int requireEnd,
            List<Span> spans) {
        // A match can't start on a wildcard — "D_ape" flagging "vape" on a
        // leading '_' is a false positive, and enumerating from a sentinel
        // burns the whole variant budget on a guaranteed no-hit.
        if (maxLen == 0 || kind[start] == 2)
            return;

        // Pass 1 is skip-only (no wildcard substitutions) — it is cheap,
        // so real skip-evasions resolve before the bounded enumeration can
        // exhaust the variant budget on substitution storms.
        subs = false;
        budget = VARIANT_BUDGET;
        overflow = false;
        enumerate(f, start, kind, cls, 0, seed, start,
                map, maxLen, requireEnd, spans);

        subs = true;
        budget = VARIANT_BUDGET;
        overflow = false;
        enumerate(f, start, kind, cls, 0, seed, start,
                map, maxLen, requireEnd, spans);

        // On budget exhaustion the local check just stops — enumerating
        // can't prove a match either way and "u_n_i_t_e_d"-shaped names are
        // legitimate. The authoritative server check stays unbounded.
    }

    private static List<Span> findMatches(Folded f) {
        ensureLoaded();

        List<Span> spans = new ArrayList<>();
        int n = f.text.length();
        if (n == 0)
            return spans;

        // Per-position kind: 0 literal letter, 1 digit marker, 2 sentinel.
        byte[] kind = new byte[n];
        String[] cls = new String[n];
        for (int i = 0; i < n; i++) {
            char c = f.text.charAt(i);
            if (c == SENTINEL) {
                kind[i] = 2;
            } else if (isMarker(c)) {
                kind[i] = 1;
                cls[i] = DIGIT_CLASS.get((char) ('0' + c - DIGIT_BASE));
            }
        }

        boolean[] allowTok = new boolean[f.tokenStart.length];
        for (int t = 0; t < allowTok.length; t++) {
            int s = f.tokenStart[t];
            int e = t + 1 < f.tokenStart.length ? f.tokenStart[t + 1] : n;
            allowTok[t] = allowOk(f, s, e);
        }

        if (!allowOk(f, 0, n)) {
            for (int p = 0; p < n; p++) {
                if (kind[p] == 0 && !inAlphabet[f.text.charAt(p)])
                    continue;
                scanPos(f, p, kind, cls, severe, severeMax, -1, spans);
            }

            // Long words may only start a match at a token boundary —
            // "s t u p i d" flags but a stray "stupid" run-in doesn't need
            // to (the token scan already covers intra-token affixes).
            for (int t = 0; t < f.tokenStart.length; t++) {
                int s = f.tokenStart[t];
                int e = t + 1 < f.tokenStart.length ? f.tokenStart[t + 1] : n;
                for (int p = s; p < e; p++) {
                    if (p != s && allowTok[t])
                        continue;
                    if (kind[p] == 0 && !inAlphabet[f.text.charAt(p)])
                        continue;
                    scanPos(f, p, kind, cls, wordSub, wordSubMax, -1, spans);
                }
            }

            // Short words flag only when the whole collapsed string equals
            // the word — "n o o b" -> "noob" flags, "noble" does not.
            scanPos(f, 0, kind, cls, wordExact, wordExactMax, n, spans);
        }

        // Short words inside a non-allowlisted token — skipped entirely for
        // allowlisted tokens, which is what keeps "seaweed" clean despite
        // "weed". (The wordSub interior scan lives above.)
        for (int t = 0; t < f.tokenStart.length; t++) {
            if (allowTok[t])
                continue;
            int start = f.tokenStart[t];
            int end = t + 1 < f.tokenStart.length ? f.tokenStart[t + 1] : n;
            scanPos(f, start, kind, cls, wordExact, wordExactMax, end, spans);
        }

        // A match lying fully inside an allowlisted token is safe — e.g.
        // "weed" inside "seaweed" — even when the whole string isn't listed.
        // Tokens span separators and digits ("weed-0" is one token), so the
        // same rescue applies to the containing literal letter run.
        spans.removeIf(sp -> {
            if (f.token[sp.start] == f.token[sp.end - 1]
                    && allowTok[f.token[sp.start]])
                return true;
            if (kind[sp.start] != 0)
                return false;
            int rs = sp.start, re = sp.start + 1;
            while (rs > 0 && kind[rs - 1] == 0) rs--;
            while (re < n && kind[re] == 0) re++;
            return sp.end <= re && allowOk(f, rs, re);
        });

        // Mask the whole token(s) a match touches — leaving "W***D" behind
        // defeats the purpose.
        for (int i = 0; i < spans.size(); i++) {
            Span span = spans.get(i);
            int first = f.token[span.start];
            int last = f.token[span.end - 1];
            span.start = f.tokenStart[first];
            span.end = last + 1 < f.tokenStart.length ? f.tokenStart[last + 1] : n;
        }

        return spans;
    }
}
