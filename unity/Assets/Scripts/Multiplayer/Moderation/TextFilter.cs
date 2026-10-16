using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Client-side profanity screen for player-visible text — display names,
    /// imported Steam persona names and any future free text (chat). The game
    /// targets ages 12+, so every name that reaches another player's screen is
    /// checked.
    ///
    /// This is the fast local filter for instant feedback; the server still
    /// validates names authoritatively on register/rename and via
    /// <see cref="Moderation.Censor"/>.
    ///
    /// Matching is evasion-resistant. Text is folded to a normalized form:
    /// lowercase, NFKC compatibility, leetspeak/diacritic/Cyrillic/Greek
    /// confusables mapped to Latin, combining marks and separators stripped,
    /// runs of the same letter collapsed ("weeeeed" -> "weed"), and wildcard
    /// characters ('*', '_', '-') treated as "any letter". The result is
    /// compared against the salted-hash list embedded in
    /// <see cref="ModerationData"/>:
    ///
    ///   severe — substring-matched on the whole collapsed string, so spaced
    ///            evasions ("w e e d") and compounds ("weedface") are caught;
    ///   words  — per-token matching: exact for stems under 5 chars ("noob"
    ///            flags but "noble" does not), substring for longer ones. The
    ///            whole collapsed string is also checked so "n o o b" still flags;
    ///   allow  — collapsed strings that never flag ("grape", "therapist",
    ///            "peacock" ...), protecting words that contain bad roots.
    ///
    /// Filter() additionally strips rich-text markup ('<' '>') and
    /// control/format/zero-width characters so a crafted name can neither
    /// inject markup nor reorder a label (RTL override).
    /// </summary>
    public static class TextFilter
    {
        /// <summary>Display-name length cap — matches the virtual keyboard.</summary>
        public const int MaxNameLength = 16;

        private const char Sentinel = '\u0001';   // wildcard: matches any one letter
        private const char MaskChar = '*';

        // The shipped word list is hash-only (ModerationData.Base64, built
        // by scripts/build_moderation_dat.php in the API repo) — a public
        // distribution must not carry readable profanity. Each banned entry
        // is a salted FNV-1a-64 of one letter-resolved folded variant plus
        // the per-position run requirements; the allow tier hashes the
        // folded string verbatim (equality check, like the original string
        // table).
        private static bool _loaded;
        private static readonly Dictionary<ulong, byte[]> _severe = new Dictionary<ulong, byte[]>();
        private static readonly Dictionary<ulong, byte[]> _wordSub = new Dictionary<ulong, byte[]>();
        private static readonly Dictionary<ulong, byte[]> _wordExact = new Dictionary<ulong, byte[]>();
        private static readonly Dictionary<ulong, byte[]> _allow = new Dictionary<ulong, byte[]>();
        private static int _severeMax, _wordSubMax, _wordExactMax;
        private static ulong _seed = FnvOffset;      // FNV state after the file salt
        private static char[] _alphabet = new char[0];
        private static char[] _wildAscii = new char[0];
        private static char[] _wildMark = new char[0];
        private static readonly bool[] _inAlphabet = new bool[1 << 16];

        private const ulong FnvOffset = 14695981039346656037UL; // 0xcbf29ce484222325
        private const ulong FnvPrime = 1099511628211UL;         // 0x100000001b3

        /// <summary>
        /// Normalized form of a string: separator-free, run-collapsed, with
        /// sentinels. Positions map back to the original text so matches can be
        /// masked.
        /// </summary>
        private sealed class Folded
        {
            public string Text = "";
            public int[] Map = new int[0];        // pos -> original index (first char of run)
            public int[] RunEnd = new int[0];     // pos -> original index (last char of run)
            public int[] Run = new int[0];        // pos -> collapsed repeat count
            public int[] Token = new int[0];      // pos -> token id
            public int[] TokenStart = new int[0]; // token id -> first pos
        }

        private struct Span
        {
            public int Start;
            public int End; // exclusive, in folded coordinates
        }

        // Confusable/leet map — applied to the lowercased NFKC form of each
        // input char AND to word-list entries, so both sides fold identically.
        private static readonly Dictionary<char, string> Subst = new Dictionary<char, string>
        {
            // leet / symbol lookalikes (digits fold through DigitClass markers —
            // one digit can stand for several letters)
            { '@', "a" }, { '$', "s" }, { '€', "e" }, { '£', "l" }, { '¥', "y" },
            { '(', "c" },

            // diacritic folds — "Scheiße", "baisé", "ş"-evasions
            { 'à', "a" }, { 'á', "a" }, { 'â', "a" }, { 'ã', "a" }, { 'ä', "a" }, { 'å', "a" },
            { 'è', "e" }, { 'é', "e" }, { 'ê', "e" }, { 'ë', "e" }, { 'ě', "e" },
            { 'ì', "i" }, { 'í', "i" }, { 'î', "i" }, { 'ï', "i" },
            { 'ò', "o" }, { 'ó', "o" }, { 'ô', "o" }, { 'õ', "o" }, { 'ö', "o" }, { 'ø', "o" },
            { 'ù', "u" }, { 'ú', "u" }, { 'û', "u" }, { 'ü', "u" }, { 'ű', "u" },
            { 'ý', "y" }, { 'ÿ', "y" },
            { 'ç', "c" }, { 'ć', "c" }, { 'č', "c" },
            { 'ñ', "n" }, { 'ń', "n" }, { 'ň', "n" },
            { 'š', "s" }, { 'ś', "s" }, { 'ş', "s" }, { 'ș', "s" },
            { 'ž', "z" }, { 'ź', "z" }, { 'ż', "z" },
            { 'ğ', "g" }, { 'ı', "i" }, { 'ł', "l" }, { 'đ', "d" }, { 'ð', "d" },
            { 'ţ', "t" }, { 'ț', "t" }, { 'ť', "t" }, { 'þ', "th" }, { 'ß', "ss" },
            { 'æ', "ae" }, { 'œ', "oe" }, { 'ů', "u" }, { 'ř', "r" },
            { 'ď', "d" }, { 'ę', "e" }, { 'ą', "a" }, { 'ė', "e" }, { 'ų', "u" }, { 'ū', "u" },
            { 'ī', "i" }, { 'ĺ', "l" }, { 'ļ', "l" }, { 'ő', "o" }, { 'ă', "a" },
            { 'ĉ', "c" }, { 'ŭ', "u" },

            // Cyrillic → Latin phonetic — catches "наркотик"/"narkotik" cross-script
            // evasion since word-list entries fold the same way.
            { 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "g" }, { 'д', "d" },
            { 'е', "e" }, { 'ё', "e" }, { 'ж', "zh" }, { 'з', "z" }, { 'и', "i" },
            { 'й', "i" }, { 'к', "k" }, { 'л', "l" }, { 'м', "m" }, { 'н', "n" },
            { 'о', "o" }, { 'п', "p" }, { 'р', "r" }, { 'с', "s" }, { 'т', "t" },
            { 'у', "u" }, { 'ф', "f" }, { 'х', "h" }, { 'ц', "c" }, { 'ч', "ch" },
            { 'ш', "sh" }, { 'щ', "sh" }, { 'ы', "y" }, { 'э', "e" }, { 'ю', "yu" },
            { 'я', "ya" }, { 'і', "i" }, { 'ї', "yi" }, { 'є', "e" }, { 'ґ', "g" },
            { 'ў', "u" },

            // Greek → Latin phonetic
            { 'α', "a" }, { 'β', "v" }, { 'γ', "g" }, { 'δ', "d" }, { 'ε', "e" },
            { 'ζ', "z" }, { 'η', "i" }, { 'θ', "th" }, { 'ι', "i" }, { 'κ', "k" },
            { 'λ', "l" }, { 'μ', "m" }, { 'ν', "n" }, { 'ξ', "x" }, { 'ο', "o" },
            { 'π', "p" }, { 'ρ', "r" }, { 'σ', "s" }, { 'ς', "s" }, { 'τ', "t" },
            { 'υ', "u" }, { 'φ', "f" }, { 'χ', "h" }, { 'ψ', "ps" }, { 'ω', "o" },
        };

        // Treated as "any single letter" during matching — '*' and '?' are the
        // classic self-censor dodge ("v*pe"); '_' and '-' are name glue.
        // Display output passes them through unchanged unless inside a masked span.
        private static readonly HashSet<char> Wildcard = new HashSet<char>
        { '*', '?', '#', '%', '&', '_', '-', '~', '!', '+', '$', '|', '•', '°', '=', '.' };

        // Dropped without breaking a token — combining marks (zalgo evasion)
        // are handled by category; soft signs and apostrophes fold away so
        // "дон't" and "don't" stay single tokens.
        private static readonly HashSet<char> Silent = new HashSet<char>
        { '\'', '’', '‘', '`', '´', 'ъ', 'ь', '"' };

        // Digits are ambiguous substitutions: '1' reads as 'i' or 'l', '3' as
        // 'e' or Cyrillic З ('z'), '8' as 'b', Cyrillic В ('v') or 'p'... A
        // digit folds to a private marker char (DigitBase + digit value) and
        // HasLetter lets a marker satisfy any letter in its class — so
        // "w33d", "n00b", "v4p3" and "dru95" all match their base words.
        private static readonly Dictionary<char, string> DigitClass = new Dictionary<char, string>
        {
            { '0', "o" }, { '1', "il" }, { '2', "tzs" }, { '3', "eza" },
            { '4', "a" }, { '5', "s" }, { '6', "bg" }, { '7', "thv" },
            { '8', "bpv" }, { '9', "gds" },
        };

        private const char DigitBase = '\u0010';

        private static bool IsMarker(char c)
        {
            return c >= DigitBase && c < DigitBase + 10;
        }



        /// <summary>True when the text contains no listed word after folding.</summary>
        public static bool IsClean(string text)
        {
            return FindMatches(Fold(text)).Count == 0;
        }

        /// <summary>Entry-side validation for typed names: length + profanity.</summary>
        public static bool IsNameAllowed(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string trimmed = name.Trim();

            // The server accepts 2..50 chars; the keyboard caps at MaxNameLength.
            if (trimmed.Length < 2 || trimmed.Length > 50)
                return false;

            return IsClean(trimmed);
        }

        /// <summary>
        /// Masks listed words inside the text with '*' and strips rich-text
        /// markup plus invisible/control characters. Use for every
        /// user-provided string before it reaches a label.
        /// </summary>
        public static string Filter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            Folded folded = Fold(text);
            List<Span> matches = FindMatches(folded);

            var mask = new bool[text.Length];

            foreach (Span span in matches)
            {
                for (int i = span.Start; i < span.End; i++)
                {
                    for (int j = folded.Map[i]; j <= folded.RunEnd[i]; j++)
                    {
                        mask[j] = true;
                    }
                }
            }

            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Drop whole <...> runs so names cannot inject markup tags.
                if (c == '<')
                {
                    int close = text.IndexOf('>', i + 1);
                    if (close > i)
                        i = close;
                    continue;
                }

                if (c == '>')
                    continue;

                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);

                if (cat == UnicodeCategory.Control || cat == UnicodeCategory.Format
                    || cat == UnicodeCategory.Surrogate || cat == UnicodeCategory.PrivateUse
                    || cat == UnicodeCategory.OtherNotAssigned)
                {
                    continue;
                }

                sb.Append(mask[i] ? MaskChar : c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Filter() for a display name; falls back to a neutral name when the
        /// mask leaves nothing readable (e.g. the whole name is a slur).
        /// </summary>
        public static string FilterName(string name, string fallback = "Player")
        {
            string filtered = Filter(name);

            if (string.IsNullOrEmpty(filtered))
                return fallback;

            foreach (char c in filtered)
            {
                if (char.IsLetterOrDigit(c))
                    return filtered;
            }

            return fallback;
        }

        /// <summary>
        /// Turns an external name (Steam persona) into a valid in-game name —
        /// collapsed whitespace, length cap, letters/digits plus '_'/'-'.
        /// Returns null when nothing usable remains or the result fails the
        /// filter; the caller then keeps the server-assigned default name.
        /// </summary>
        public static string SanitizeImportedName(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return null;

            var sb = new StringBuilder(raw.Length);
            bool pendingSpace = false;

            foreach (char c in raw.Trim())
            {
                if (char.IsWhiteSpace(c))
                {
                    pendingSpace = true;
                    continue;
                }

                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                    continue;

                if (pendingSpace && sb.Length > 0)
                    sb.Append(' ');

                pendingSpace = false;
                sb.Append(c);
            }

            string name = sb.ToString();

            if (name.Length > MaxNameLength)
                name = name.Substring(0, MaxNameLength).TrimEnd();

            return IsNameAllowed(name) ? name : null;
        }

        /// <summary>Re-reads the embedded word list.</summary>
        public static void Reload()
        {
            _loaded = false;
            EnsureLoaded();
        }



        private static void EnsureLoaded()
        {
            if (_loaded)
                return;

            _loaded = true;
            _severe.Clear();
            _wordSub.Clear();
            _wordExact.Clear();
            _allow.Clear();
            _severeMax = _wordSubMax = _wordExactMax = 0;
            _alphabet = new char[0];
            Array.Clear(_inAlphabet, 0, _inAlphabet.Length);
            _seed = FnvOffset;

            try
            {
                LoadTable(Convert.FromBase64String(ModerationData.Base64));
            }
            catch (Exception e)
            {
                // Corrupt/missing data disables the local filter — the
                // server-side check still applies on register/rename.
                Debug.LogWarning($"[TextFilter] embedded moderation data parse failed: {e.Message}");
            }
        }

        private static int U16(byte[] d, ref int off)
        {
            int v = d[off] | (d[off + 1] << 8);
            off += 2;
            return v;
        }

        private static uint U32(byte[] d, ref int off)
        {
            uint v = (uint)(d[off] | (d[off + 1] << 8) | (d[off + 2] << 16) | (d[off + 3] << 24));
            off += 4;
            return v;
        }

        private static ulong U64(byte[] d, ref int off)
        {
            ulong v = 0;
            for (int i = 0; i < 8; i++)
                v |= (ulong)d[off + i] << (8 * i);
            off += 8;
            return v;
        }

        // Binary word list — see scripts/build_moderation_dat.php for the format.
        private static void LoadTable(byte[] d)
        {
            if (d.Length < 26 || d[0] != 'M' || d[1] != 'F' || d[2] != 'I' || d[3] != 'L')
                throw new FormatException("bad magic");

            int off = 4;
            int version = U16(d, ref off);
            U16(d, ref off); // flags, reserved
            if (version != 1)
                throw new FormatException($"unsupported version {version}");

            for (int i = 0; i < 16; i++)
                _seed = FnvByte(_seed, d[off + i]);
            off += 16;

            int alen = U16(d, ref off);
            _alphabet = Encoding.UTF8.GetString(d, off, alen).ToCharArray();
            off += alen;
            foreach (char c in _alphabet)
                _inAlphabet[c] = true;

            // Sentinel slots ("any one letter") enumerate ASCII letters plus
            // combining marks: wildcard substitution is mostly a Latin-script
            // evasion ("v_pe"), while marks cover separators dropped inside a
            // grapheme cluster. The smaller branch factor keeps names like
            // "a_b_c" inside the variant budget.
            _wildAscii = Array.FindAll(_alphabet, c => c < '\u0080');
        _wildMark = Array.FindAll(_alphabet,
            c => char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark
                || char.GetUnicodeCategory(c) == UnicodeCategory.SpacingCombiningMark);

            LoadSection(d, ref off, _severe, ref _severeMax);
            LoadSection(d, ref off, _wordSub, ref _wordSubMax);
            LoadSection(d, ref off, _wordExact, ref _wordExactMax);

            uint nAllow = U32(d, ref off);
            for (uint i = 0; i < nAllow; i++)
            {
                ulong h = U64(d, ref off);
                int ns = d[off++];
                var runs = new byte[ns];
                Array.Copy(d, off, runs, 0, ns);
                off += ns;
                _allow[h] = runs;
            }
        }

        // An allow hit needs the same folded text AND enough input run per
        // position: "shiite" is safe, but the same fold with a single 'i'
        // ("shite") is a listed word, not an allow hit.
        private static bool AllowOk(Folded f, int s, int e)
        {
            if (!_allow.TryGetValue(FnvString(_seed, f.Text.Substring(s, e - s)), out byte[] need)
                    || need.Length != e - s)
                return false;
            for (int i = s; i < e; i++)
            {
                if (f.Run[i] < (need[i - s] & 0x3F))
                    return false;
            }
            return true;
        }

        private static void LoadSection(byte[] d, ref int off,
            Dictionary<ulong, byte[]> map, ref int maxLen)
        {
            uint count = U32(d, ref off);
            for (uint i = 0; i < count; i++)
            {
                ulong h = U64(d, ref off);
                int n = d[off++];
                if (n == 0 || n > _slotRun.Length) // beyond scratch arrays — can't match anyway
                {
                    off += n;
                    continue;
                }
                byte[] runs = new byte[n];
                Buffer.BlockCopy(d, off, runs, 0, n);
                off += n;
                if (!map.ContainsKey(h))
                    map.Add(h, runs);
                if (n > maxLen)
                    maxLen = n;
            }
        }

        private static ulong FnvByte(ulong h, byte b)
        {
            return unchecked((h ^ b) * FnvPrime);
        }

        // FNV-1a-64 over the UTF-8 bytes of a folded letter — matches the PHP
        // generator hashing the UTF-8 string bytes directly.
        private static ulong FnvChar(ulong h, char c)
        {
            if (c < '\u0080')
                return FnvByte(h, (byte)c);
            if (c < '\u0800')
                return FnvByte(FnvByte(h, (byte)(0xC0 | (c >> 6))), (byte)(0x80 | (c & 0x3F)));
            return FnvByte(FnvByte(FnvByte(h, (byte)(0xE0 | (c >> 12))),
                (byte)(0x80 | ((c >> 6) & 0x3F))), (byte)(0x80 | (c & 0x3F)));
        }

        private static ulong FnvString(ulong h, string s)
        {
            foreach (char c in s)
                h = FnvChar(h, c);
            return h;
        }



        private static Folded Fold(string text)
        {
            var result = new Folded();

            if (string.IsNullOrEmpty(text))
                return result;

            var sb = new StringBuilder(text.Length);
            var map = new List<int>(text.Length);
            var runEnd = new List<int>(text.Length);
            var run = new List<int>(text.Length);
            var token = new List<int>(text.Length);
            var tokenStarts = new List<int>();

            var piece = new StringBuilder(4);
            int tokenId = -1;
            bool boundary = true;
            char lastLetter = ' '; // last non-sentinel char — see the mark rule
            int markRun = 0;    // consecutive identical digit markers at the tail

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (Silent.Contains(char.ToLowerInvariant(c)))
                    continue;

                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);

                if (cat == UnicodeCategory.NonSpacingMark || cat == UnicodeCategory.SpacingCombiningMark)
                {
                    // Combining marks are essential vowels/tones in Thai,
                    // Devanagari, Arabic etc. — keep them after a non-ASCII base
                    // inside the same token so e.g. "บ้า" doesn't collapse to a
                    // bare consonant. After ASCII output they are zalgo noise and
                    // get dropped ("v a ̈ p e" -> "vape").
                    if (!boundary && sb.Length > 0 && lastLetter > '\u007F')
                    {
                        char lower = char.ToLowerInvariant(c);

                        if (sb[sb.Length - 1] == lower)
                        {
                            runEnd[runEnd.Count - 1] = i;
                            run[run.Count - 1]++;
                        }
                        else
                        {
                            sb.Append(lower);
                            map.Add(i);
                            runEnd.Add(i);
                            run.Add(1);
                            token.Add(tokenId);
                            lastLetter = lower;
                        }
                    }

                    continue;
                }

                piece.Length = 0;
                EmitFolded(c, piece);

                if (piece.Length == 0)
                {
                    boundary = true;
                    continue;
                }

                if (boundary)
                {
                    tokenId++;
                    tokenStarts.Add(sb.Length);
                    boundary = false;
                }

                for (int p = 0; p < piece.Length; p++)
                {
                    char e = piece[p];

                    // Runs of the same folded letter collapse to one, but only
                    // inside a token — "ww ee ee dd" still becomes "w e e d".
                    // Digit markers stay separate so "d11d0" can feed
                    // different slots ('i' then 'l') — except past three in a
                    // row, where a long digit tail would make the bounded
                    // matcher enumerate exponentially ("e2e-999…9-1").
                    if (sb.Length > 0 && sb[sb.Length - 1] == e && token[token.Count - 1] == tokenId)
                    {
                        if (!IsMarker(e) || markRun >= 3)
                        {
                            runEnd[runEnd.Count - 1] = i;
                            run[run.Count - 1]++;
                            continue;
                        }
                        markRun++;
                    }
                    else
                    {
                        markRun = IsMarker(e) ? 1 : 0;
                    }

                    sb.Append(e);
                    map.Add(i);
                    runEnd.Add(i);
                    run.Add(1);
                    token.Add(tokenId);
                    if (e != Sentinel)
                        lastLetter = e;
                }
            }

            result.Text = sb.ToString();
            result.Map = map.ToArray();
            result.RunEnd = runEnd.ToArray();
            result.Run = run.ToArray();
            result.Token = token.ToArray();
            result.TokenStart = tokenStarts.ToArray();
            return result;
        }

        private static void EmitFolded(char c, StringBuilder sb)
        {
            // Single-char compatibility decomposition: fullwidth letters,
            // ligatures (ﬁ -> fi), circled digits, superscripts.
            string norm = c.ToString().Normalize(NormalizationForm.FormKC);

            foreach (char n in norm)
            {
                char lower = char.ToLowerInvariant(n);

                // Leet/lookalike folds win over the wildcard set — '$' means 's'
                // in "pu$$y", only falling back to a wildcard elsewhere.
                if (Subst.TryGetValue(lower, out string repl))
                {
                    sb.Append(repl);
                    continue;
                }

                if (DigitClass.ContainsKey(lower))
                {
                    sb.Append((char)(DigitBase + lower - '0'));
                    continue;
                }

                if (Wildcard.Contains(lower))
                {
                    sb.Append(Sentinel);
                    continue;
                }

                if (char.IsLetterOrDigit(lower))
                {
                    sb.Append(lower);
                }
            }
        }

        // ---------------- hash-scan matcher ----------------
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

        private static readonly int[] _slotRun = new int[64];
        // How each emitted slot was satisfied: 0 literal letter, 1 digit
        // marker, 2 sentinel substitute. A substitute is legal only between
        // two literal edges of the word — "v*pe" keeps "vape" because the
        // '*' sits between real 'f' and 'c', while "Ora-35" guessing a word
        // on "-3" glue is a false positive.
        private static readonly byte[] _slotKind = new byte[64];
        // Reachable run ends per slot depth, so the longest run emits
        // first (recursion clobbers shared scratch — rows are per slot).
        private static readonly int[] _emitEnd = new int[64 * 64];
        private static readonly int[] _emitSum = new int[64 * 64];
        private static readonly byte[] _emitKind = new byte[64 * 64];
        // True when the sentinel's token holds any non-ASCII letter — only
        // then can a wildcard plausibly stand in for a combining mark.
        private static bool TokenWide(Folded f, int pos)
        {
            int t = f.Token[pos];
            for (int i = pos; i >= 0 && f.Token[i] == t; i--)
                if (f.Text[i] > '\u007F')
                    return true;
            for (int i = pos + 1; i < f.Text.Length && f.Token[i] == t; i++)
                if (f.Text[i] > '\u007F')
                    return true;
            return false;
        }

        private static int _budget;
        private static bool _overflow;
        private static bool _subs;
        private const int VariantBudget = 4096;

        // Does folded position j offer letter l? Letters: equality; markers:
        // class membership; sentinels never join a letter run.
        private static bool HasLetter(Folded f, byte[] kind, string[] cls, int j, char l)
        {
            if (kind[j] == 0)
                return f.Text[j] == l;
            if (kind[j] == 1)
                return cls[j].IndexOf(l) >= 0;
            return false;
        }

        private static bool RunsOk(int slots, byte[] need)
        {
            for (int k = 0; k < slots; k++)
            {
                int run = need[k] & 0x3F;
                if (_slotKind[k] == 2)
                {
                    if (run > 1)
                        return false;
                }
                else if ((need[k] & 0x80) != 0 && _slotKind[k] == 0)
                {
                    // A digit-derived slot of a listed entry can't be
                    // filled by a real letter — "4 2 0" stays digit-only.
                    return false;
                }
                else if (_slotRun[k] < run)
                {
                    return false;
                }
            }
            return true;
        }

        // A match needs grounded anchors — input positions that are not
        // wildcards: three when a wildcard substitutes, two when separators
        // were merely skipped, one otherwise. Digit markers count — a
        // marker resolves to a small fixed class, so "455" spelling a
        // short entry is deliberate, not a guess. With fewer, sentinels
        // alone could spell a short entry: "-9-" resolves on wildcards
        // without touching a letter — which is how random names like
        // "e2e-1234567890-1" would flag.
        private static bool LiteralOk(byte[] kind, int start, int end,
            int slots, byte[] need)
        {
            int lit = 0;
            int ground = 0;
            int lastLit = -1;
            bool anySent = false;
            bool seenMarker = false;
            for (int i = start; i < end; i++)
            {
                if (kind[i] == 0)
                {
                    lit++;
                    ground++;
                    lastLit = i;
                }
                else if (kind[i] == 2)
                {
                    anySent = true;
                }
                else
                {
                    ground++;
                }
            }
            // A separator anywhere before the last letter is woven evasion,
            // so the "-digits" tail can no longer pass as name glue
            // ("k_u_n_7").
            bool sentBeforeLit = false;
            for (int i = start; i < lastLit; i++)
            {
                if (kind[i] == 2)
                {
                    sentBeforeLit = true;
                    break;
                }
            }
            bool anyWild = false;
            bool allWildMark = true;
            bool allDig = true;
            for (int k = 0; k < slots; k++)
            {
                if (_slotKind[k] == 2)
                {
                    anyWild = true;
                    if ((need[k] & 0x40) == 0)
                        allWildMark = false;
                }
                if ((need[k] & 0x80) == 0)
                    allDig = false;
            }
            if (!(lit == 0 && allDig))
            {
                if (ground < (anyWild ? (allWildMark ? 2 : 3) : anySent ? 2 : 1))
                    return false;
                // "-digits" after the last real letter is name glue, not
                // evasion ("Xena-21" can't spell a word) — but only when
                // the separators all precede the digits AND the word used
                // no digit substitution. A separator woven between digits
                // ("5_h_1_7") or anywhere earlier in the word is evasion,
                // so the tail rule lifts.
                bool glue = false, interleaved = false;
                seenMarker = false;
                for (int i = lastLit + 1; i < end; i++)
                {
                    if (kind[i] == 1)
                    {
                        seenMarker = true;
                    }
                    else if (kind[i] == 2)
                    {
                        if (seenMarker)
                        {
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

        private static void Enum(Folded f, int pos, byte[] kind, string[] cls, int slot,
            ulong h, int start, Dictionary<ulong, byte[]> map,
            int maxLen, int requireEnd, List<Span> spans)
        {
            int n = f.Text.Length;

            if (slot > 0 && (requireEnd < 0 || pos == requireEnd)
                && kind[start] != 2 && kind[pos - 1] != 2
                && map.TryGetValue(h, out byte[] need) && need.Length == slot
                && RunsOk(slot, need) && LiteralOk(kind, start, pos, slot, need))
            {
                spans.Add(new Span { Start = start, End = pos });
            }

            if (_overflow || slot == maxLen || pos >= n)
                return;

            if (--_budget < 0)
            {
                _overflow = true;
                return;
            }

            if (kind[pos] == 2)        // sentinel: junk to skip, or a wildcard slot
            {
                // Junk first — "w_e_e_d" spells its word with the '_'s skipped,
                // and the cheap path must run before the 26-way wildcard
                // enumeration drains the variant budget.
                Enum(f, pos + 1, kind, cls, slot, h, start, map, maxLen,
                    requireEnd, spans);
                // Substitution only onto a literal-bounded slot: the previous
                // slot must have ended on a real letter (the right edge is
                // enforced when the next slot emits — markers can't follow a
                // substitute).
                if (_subs && slot > 0 && _slotKind[slot - 1] == 0)
                {
                    int nWild = _wildAscii.Length;
                    if (TokenWide(f, pos))
                        nWild += _wildMark.Length;
                    for (int a = 0; a < nWild; a++)
                    {
                        char w = a < _wildAscii.Length ? _wildAscii[a] : _wildMark[a - _wildAscii.Length];
                        _slotRun[slot] = 0;
                        _slotKind[slot] = 2;
                        Enum(f, pos + 1, kind, cls, slot + 1, FnvChar(h, w),
                            start, map, maxLen, requireEnd, spans);
                        if (_overflow)
                            return;
                    }
                }
                return;
            }

            if (kind[pos] == 0)        // literal letter
            {
                char l = f.Text[pos];
                if (!_inAlphabet[l])
                    return;
                // Emit at every reachable run end, longest first: sentinels
                // inside a run are junk ("w-e-e-d" counts the two e's) while
                // the cut-before-sentinel emission leaves it free to
                // substitute the next slot ("v*pe"). Longest-first keeps
                // deep runs ahead of the wildcard substitution storm when
                // the variant budget is tight.
                int j = pos, sum = 0, cnt = 0, b = slot * 64;
                while (j < n && cnt < 64)
                {
                    if (kind[j] == 2)
                    {
                        j++;
                        continue;
                    }
                    if (!HasLetter(f, kind, cls, j, l))
                        break;
                    sum += f.Run[j];
                    j++;
                    _emitEnd[b + cnt] = j;
                    _emitSum[b + cnt] = sum;
                    _emitKind[b + cnt] = kind[j - 1];
                    cnt++;
                }
                for (int k = cnt - 1; k >= 0; k--)
                {
                    _slotRun[slot] = _emitSum[b + k];
                    _slotKind[slot] = _emitKind[b + k];
                    Enum(f, _emitEnd[b + k], kind, cls, slot + 1,
                        FnvChar(h, l), start, map, maxLen, requireEnd, spans);
                    if (_overflow)
                        return;
                }
                return;
            }

            // A digit marker can't be the next slot's first position after a
            // substitute — that would make the substituted edge a guess both
            // ways. Separators interleaved with markers mid-word are legal
            // ("w_3_3_d"); a glued "-digits" tail is rejected by the hit
            // check instead.
            if (slot > 0 && _slotKind[slot - 1] == 2)
                return;

            foreach (char l in cls[pos])   // digit marker → every class letter
            {
                if (!_inAlphabet[l])
                    continue;
                int j = pos, sum = 0, cnt = 0, b = slot * 64;
                while (j < n && cnt < 64)
                {
                    if (kind[j] == 2)
                    {
                        j++;
                        continue;
                    }
                    if (!HasLetter(f, kind, cls, j, l))
                        break;
                    sum += f.Run[j];
                    j++;
                    _emitEnd[b + cnt] = j;
                    _emitSum[b + cnt] = sum;
                    _emitKind[b + cnt] = kind[j - 1];
                    cnt++;
                }
                for (int k = cnt - 1; k >= 0; k--)
                {
                    _slotRun[slot] = _emitSum[b + k];
                    _slotKind[slot] = _emitKind[b + k];
                    Enum(f, _emitEnd[b + k], kind, cls, slot + 1,
                        FnvChar(h, l), start, map, maxLen, requireEnd, spans);
                    if (_overflow)
                        return;
                }
            }
        }

        private static void ScanPos(Folded f, int start, byte[] kind, string[] cls,
            Dictionary<ulong, byte[]> map, int maxLen, int requireEnd, List<Span> spans)
        {
            // A match can't start on a wildcard — "D_ape" flagging "vape" on
            // a leading '_' is a false positive, and enumerating from a
            // sentinel burns the whole variant budget on a guaranteed no-hit.
            if (maxLen == 0 || kind[start] == 2)
                return;

            // Pass 1 is skip-only (no wildcard substitutions) — it is cheap,
            // so real skip-evasions resolve before the bounded enumeration can
            // exhaust the variant budget on substitution storms.
            _subs = false;
            _budget = VariantBudget;
            _overflow = false;
            Enum(f, start, kind, cls, 0, _seed, start, map, maxLen, requireEnd, spans);

            _subs = true;
            _budget = VariantBudget;
            _overflow = false;
            Enum(f, start, kind, cls, 0, _seed, start, map, maxLen, requireEnd, spans);

            // On budget exhaustion the local check just stops — enumerating
            // can't prove a match either way and "u_n_i_t_e_d"-shaped names are
            // legitimate. The authoritative server check stays unbounded.
        }

        private static List<Span> FindMatches(Folded f)
        {
            EnsureLoaded();

            var spans = new List<Span>();

            if (f.Text.Length == 0)
                return spans;

            int n = f.Text.Length;

            // Per-position kind: 0 literal letter, 1 digit marker, 2 sentinel.
            var kind = new byte[n];
            var cls = new string[n];
            for (int i = 0; i < n; i++)
            {
                char c = f.Text[i];
                if (c == Sentinel)
                {
                    kind[i] = 2;
                }
                else if (IsMarker(c))
                {
                    kind[i] = 1;
                    cls[i] = DigitClass[(char)('0' + c - DigitBase)];
                }
            }

            var allowTok = new bool[f.TokenStart.Length];
            for (int t = 0; t < allowTok.Length; t++)
            {
                int s = f.TokenStart[t];
                int e = t + 1 < f.TokenStart.Length ? f.TokenStart[t + 1] : n;
                allowTok[t] = AllowOk(f, s, e);
            }

            if (!AllowOk(f, 0, n))
            {
                for (int p = 0; p < n; p++)
                {
                    if (kind[p] == 0 && !_inAlphabet[f.Text[p]])
                        continue;
                    ScanPos(f, p, kind, cls, _severe, _severeMax, -1, spans);
                }

                // Long words may only start a match at a token boundary —
                // "s t u p i d" flags but a stray "stupid" run-in doesn't need
                // to (the token scan already covers intra-token affixes).
                for (int t = 0; t < f.TokenStart.Length; t++)
                {
                    int s = f.TokenStart[t];
                    int e = t + 1 < f.TokenStart.Length ? f.TokenStart[t + 1] : n;
                    for (int p = s; p < e; p++)
                    {
                        if (p != s && allowTok[t])
                            continue;
                        if (kind[p] == 0 && !_inAlphabet[f.Text[p]])
                            continue;
                        ScanPos(f, p, kind, cls, _wordSub, _wordSubMax, -1, spans);
                    }
                }

                // Short words flag only when the whole collapsed string equals
                // the word — "n o o b" -> "noob" flags, "noble" does not.
                ScanPos(f, 0, kind, cls, _wordExact, _wordExactMax, n, spans);
            }

            // Short words inside a non-allowlisted token — skipped entirely for
            // allowlisted tokens, which is what keeps "seaweed" clean despite
            // "weed". (The wordSub interior scan lives above.)
            for (int t = 0; t < f.TokenStart.Length; t++)
            {
                if (allowTok[t])
                    continue;
                int start = f.TokenStart[t];
                int end = t + 1 < f.TokenStart.Length ? f.TokenStart[t + 1] : f.Text.Length;
                ScanPos(f, start, kind, cls, _wordExact, _wordExactMax, end, spans);
            }

            // A match lying fully inside an allowlisted token is safe — e.g.
            // "weed" inside "seaweed" — even when the whole string isn't
            // listed. Tokens span separators and digits ("grape-0" is one
            // token), so the same rescue applies to the containing literal
            // letter run.
            for (int i = spans.Count - 1; i >= 0; i--)
            {
                Span span = spans[i];

                bool allowed = f.Token[span.Start] == f.Token[span.End - 1]
                    && allowTok[f.Token[span.Start]];
                if (!allowed && kind[span.Start] == 0)
                {
                    int rs = span.Start, re = span.Start + 1;
                    while (rs > 0 && kind[rs - 1] == 0) rs--;
                    while (re < n && kind[re] == 0) re++;
                    allowed = span.End <= re
                        && AllowOk(f, rs, re);
                }
                if (allowed)
                    spans.RemoveAt(i);
            }

            // Mask the whole token(s) a match touches — leaving "W***D" behind
            // defeats the purpose.
            for (int i = 0; i < spans.Count; i++)
            {
                Span span = spans[i];
                int first = f.Token[span.Start];
                int last = f.Token[span.End - 1];
                span.Start = f.TokenStart[first];
                span.End = last + 1 < f.TokenStart.Length ? f.TokenStart[last + 1] : f.Text.Length;
                spans[i] = span;
            }

            return spans;
        }
    }
}
