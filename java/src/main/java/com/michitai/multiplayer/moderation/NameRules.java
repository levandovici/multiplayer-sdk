package com.michitai.multiplayer.moderation;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Per-game character policy for display and lobby names, configured in the
 * developer dashboard and published inside the public game data blob as
 * {@code name_rules}. Deserialize it with your game data class:
 * <pre>{@code
 * public class MyGameData {
 *     @JsonProperty("name_rules") public NameRules nameRules;
 * }
 * }</pre>
 * The server enforces the policy at registration, rename and room creation
 * regardless of what the client sends — {@link #validate} mirrors the same
 * checks locally so input fields can give live feedback. When no rules are
 * configured the Latin-letters-only default applies.
 */
public class NameRules {
    /** Latin-letters-only fallback for games without configured rules. */
    public static final NameRules DEFAULT = new NameRules();

    /** Allow Latin letters (a-z, accented variants, extended blocks). */
    @JsonProperty("latin") public boolean latin = true;
    /** Allow Cyrillic letters (а-я). */
    @JsonProperty("cyrillic") public boolean cyrillic;
    /** Allow digits (0-9 and other decimal digits). */
    @JsonProperty("digits") public boolean digits;
    /** Allow whitespace characters. */
    @JsonProperty("space") public boolean space;
    /** Allow _ underscore. */
    @JsonProperty("underscore") public boolean underscore;
    /** Allow - hyphen (ASCII and Unicode hyphens). */
    @JsonProperty("hyphen") public boolean hyphen;
    /** Allow any other character (other scripts, punctuation, emoji). */
    @JsonProperty("symbols") public boolean symbols;

    /**
     * Letter-case policy: "any", "lower", "upper", "capitalize" (only the
     * first letter may be uppercase) or "capitalize_strict" (first letter
     * must be uppercase, the rest lowercase).
     */
    @JsonProperty("case") public String letterCase = "any";

    /**
     * Checks a name against these rules. Returns a human-readable reason when
     * the name violates the policy, or null when it is acceptable. Profanity
     * is a separate concern — combine with TextFilter/Moderation.censor
     * (the server checks both).
     */
    public String validate(String name) {
        return validate(name, this);
    }

    /** Checks a name against the given rules (or the default policy). */
    public static String validate(String name, NameRules rules) {
        if (rules == null) {
            rules = DEFAULT;
        }
        if (name == null || name.isEmpty()) {
            return "Name is required";
        }

        int upper = 0, lower = 0;
        Boolean firstLetterUpper = null;
        for (int i = 0; i < name.length();) {
            int cp = name.codePointAt(i);
            int width = Character.charCount(cp);
            String cls = classify(name, i, cp);
            if (!rules.allows(cls)) {
                return "Name cannot contain " + classLabel(cls);
            }
            if (cls.equals("latin") || cls.equals("cyrillic")) {
                if (Character.isUpperCase(cp)) {
                    upper++;
                    if (firstLetterUpper == null) {
                        firstLetterUpper = true;
                    }
                } else {
                    lower++;
                    if (firstLetterUpper == null) {
                        firstLetterUpper = false;
                    }
                }
            }
            i += width;
        }

        switch (rules.letterCase == null ? "any" : rules.letterCase) {
            case "lower":
                if (upper > 0) {
                    return "Name must be lowercase";
                }
                break;
            case "upper":
                if (lower > 0) {
                    return "Name must be uppercase";
                }
                break;
            case "capitalize":
                if (upper > 1 || (upper == 1 && !firstLetterUpper)) {
                    return "Only the first letter of the name may be uppercase";
                }
                break;
            case "capitalize_strict":
                if (upper != 1 || !Boolean.TRUE.equals(firstLetterUpper)) {
                    return "Name must start with a capital letter followed by lowercase";
                }
                break;
            default:
                break;
        }

        return null;
    }

    private boolean allows(String cls) {
        switch (cls) {
            case "latin": return latin;
            case "cyrillic": return cyrillic;
            case "digits": return digits;
            case "space": return space;
            case "underscore": return underscore;
            case "hyphen": return hyphen;
            default: return symbols;
        }
    }

    private static String classLabel(String cls) {
        switch (cls) {
            case "latin": return "Latin letters";
            case "cyrillic": return "Cyrillic letters";
            case "digits": return "digits";
            case "space": return "spaces";
            case "underscore": return "underscores";
            case "hyphen": return "hyphens";
            default: return "symbols";
        }
    }

    private static String classify(String s, int i, int cp) {
        // Script=Latin ≈ letters inside the Latin Unicode blocks; combining
        // marks and punctuation inside those blocks are Inherited/Common.
        if (Character.isLetter(cp) && isLatinRange(cp)) {
            return "latin";
        }
        // Script=Cyrillic covers the whole Cyrillic blocks including
        // combining marks.
        if ((cp >= 0x0400 && cp <= 0x052F)
                || (cp >= 0x2DE0 && cp <= 0x2DFF)
                || (cp >= 0xA640 && cp <= 0xA69F)
                || (cp >= 0x1C80 && cp <= 0x1C8F)) {
            return "cyrillic";
        }
        if (Character.isDigit(cp)) { // decimal digits only, like \p{Nd}
            return "digits";
        }
        int type = Character.getType(cp); // Zs/Zl/Zp plus C0 whitespace, like the server
        if (type == Character.SPACE_SEPARATOR
                || type == Character.LINE_SEPARATOR
                || type == Character.PARAGRAPH_SEPARATOR
                || (cp >= 0x09 && cp <= 0x0D) || cp == 0x85) {
            return "space";
        }
        if (cp == '_') {
            return "underscore";
        }
        if (cp == '-' || (cp >= 0x2010 && cp <= 0x2015)) {
            return "hyphen";
        }

        return "symbols";
    }

    private static boolean isLatinRange(int cp) {
        return (cp >= 'A' && cp <= 'Z')
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
