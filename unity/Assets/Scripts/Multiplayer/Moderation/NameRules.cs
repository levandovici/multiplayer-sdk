using System;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Per-game character policy for display and lobby names, configured in
    /// the developer dashboard and published inside the public game data blob
    /// as <c>name_rules</c>. Deserialize it with your game data class:
    /// <code>
    /// [Serializable]
    /// public class MyGameData
    /// {
    ///     public NameRules name_rules;
    /// }
    /// </code>
    /// The server enforces the policy at registration, rename and room
    /// creation regardless of what the client sends — ValidateName mirrors the
    /// same checks locally so input fields can give live feedback. When no
    /// rules are configured the Latin-letters-only default applies.
    /// </summary>
    [Serializable]
    public class NameRules
    {
        /// <summary>Allow Latin letters (a-z, accented variants, extended blocks).</summary>
        public bool latin = true;
        /// <summary>Allow Cyrillic letters (а-я).</summary>
        public bool cyrillic;
        /// <summary>Allow digits (0-9 and other decimal digits).</summary>
        public bool digits;
        /// <summary>Allow whitespace characters.</summary>
        public bool space;
        /// <summary>Allow _ underscore.</summary>
        public bool underscore;
        /// <summary>Allow - hyphen (ASCII and Unicode hyphens).</summary>
        public bool hyphen;
        /// <summary>Allow any other character (other scripts, punctuation, emoji).</summary>
        public bool symbols;

        /// <summary>
        /// Letter-case policy: "any", "lower", "upper", "capitalize" (only the
        /// first letter may be uppercase) or "capitalize_strict" (first letter
        /// must be uppercase, the rest lowercase).
        /// </summary>
        public string @case = "any";

        /// <summary>Latin-letters-only fallback for games without configured rules.</summary>
        public static readonly NameRules Default = new NameRules();

        /// <summary>
        /// Checks a name against these rules. Returns a human-readable reason
        /// when the name violates the policy, or null when it is acceptable.
        /// Profanity is a separate concern — combine with TextFilter.IsClean /
        /// Moderation.CensorAsync (the server checks both).
        /// </summary>
        public string Validate(string name) => ValidateName(name, this);

        /// <summary>Checks a name against the given rules (or the default policy).</summary>
        public static string ValidateName(string name, NameRules rules)
        {
            if (rules == null)
                rules = Default;
            if (string.IsNullOrEmpty(name))
                return "Name is required";

            int upper = 0, lower = 0;
            bool? firstLetterUpper = null;
            for (int i = 0; i < name.Length; i += char.IsSurrogatePair(name, i) ? 2 : 1)
            {
                int cp = char.ConvertToUtf32(name, i);
                string cls = Classify(name, i, cp);
                if (!rules.Allows(cls))
                    return "Name cannot contain " + ClassLabel(cls);
                if (cls == "latin" || cls == "cyrillic")
                {
                    if (char.IsUpper(name, i))
                    {
                        upper++;
                        if (firstLetterUpper == null) firstLetterUpper = true;
                    }
                    else
                    {
                        lower++;
                        if (firstLetterUpper == null) firstLetterUpper = false;
                    }
                }
            }

            switch (rules.@case)
            {
                case "lower" when upper > 0:
                    return "Name must be lowercase";
                case "upper" when lower > 0:
                    return "Name must be uppercase";
                case "capitalize" when upper > 1 || (upper == 1 && firstLetterUpper == false):
                    return "Only the first letter of the name may be uppercase";
                case "capitalize_strict" when upper != 1 || firstLetterUpper != true:
                    return "Name must start with a capital letter followed by lowercase";
            }

            return null;
        }

        private bool Allows(string cls)
        {
            switch (cls)
            {
                case "latin": return latin;
                case "cyrillic": return cyrillic;
                case "digits": return digits;
                case "space": return space;
                case "underscore": return underscore;
                case "hyphen": return hyphen;
                default: return symbols;
            }
        }

        private static string ClassLabel(string cls)
        {
            switch (cls)
            {
                case "latin": return "Latin letters";
                case "cyrillic": return "Cyrillic letters";
                case "digits": return "digits";
                case "space": return "spaces";
                case "underscore": return "underscores";
                case "hyphen": return "hyphens";
                default: return "symbols";
            }
        }

        private static string Classify(string s, int i, int cp)
        {
            // Script=Latin ≈ letters inside the Latin Unicode blocks (the ASCII
            // letters plus Latin-1/extended ranges); combining marks and
            // punctuation inside those blocks are Script=Inherited/Common.
            if (char.IsLetter(s, i) && IsLatinRange(cp))
                return "latin";
            // Script=Cyrillic covers the whole Cyrillic blocks including
            // combining marks.
            if ((cp >= 0x0400 && cp <= 0x052F)
                || (cp >= 0x2DE0 && cp <= 0x2DFF)
                || (cp >= 0xA640 && cp <= 0xA69F)
                || (cp >= 0x1C80 && cp <= 0x1C8F))
                return "cyrillic";
            if (char.IsDigit(s, i)) // decimal digits only, like \p{Nd}
                return "digits";
            if (char.IsWhiteSpace(s, i)) // Zs/Zl/Zp plus C0 whitespace, like the server
                return "space";
            if (cp == '_')
                return "underscore";
            if (cp == '-' || (cp >= 0x2010 && cp <= 0x2015))
                return "hyphen";

            return "symbols";
        }

        private static bool IsLatinRange(int cp)
            => (cp >= 'A' && cp <= 'Z')
                || (cp >= 'a' && cp <= 'z')
                || (cp >= 0x00C0 && cp <= 0x02AF)
                || (cp >= 0x02B0 && cp <= 0x02FF)
                || (cp >= 0x1D00 && cp <= 0x1DBF)
                || (cp >= 0x1E00 && cp <= 0x1EFF)
                || (cp >= 0x2C60 && cp <= 0x2C7F)
                || (cp >= 0xA720 && cp <= 0xA7FF)
                || (cp >= 0xAB30 && cp <= 0xAB6F)
                || (cp >= 0xFB00 && cp <= 0xFB06)
                || (cp >= 0xFF21 && cp <= 0xFF3A)
                || (cp >= 0xFF41 && cp <= 0xFF5A)
                || (cp >= 0x10780 && cp <= 0x107BA)
                || (cp >= 0x1DF00 && cp <= 0x1DF1E);
    }
}
