#pragma once

#include <cwctype>
#include <string>
#include <vector>

namespace michitai {
namespace multiplayer {
namespace moderation {

/// Per-game character policy for display and lobby names, configured in the
/// developer dashboard and published inside the public game data blob as
/// "name_rules". The server enforces the policy at registration, rename and
/// room creation regardless of what the client sends — validate() mirrors
/// the same checks locally so input fields can give live feedback. When no
/// rules are configured the Latin-letters-only default applies.
///
/// Profanity is a separate concern — combine with TextFilter::isClean /
/// Moderation::censor (the server checks both).
struct NameRules {
    bool latin = true;      ///< a-z, accented variants, extended blocks
    bool cyrillic = false;  ///< а-я
    bool digits = false;    ///< 0-9 and other decimal digits
    bool space = false;     ///< whitespace characters
    bool underscore = false;///< _ underscore
    bool hyphen = false;    ///< - hyphen (ASCII and Unicode hyphens)
    bool symbols = false;   ///< other scripts, punctuation, emoji

    /// Letter-case policy: "any", "lower", "upper", "capitalize" (only the
    /// first letter may be uppercase) or "capitalize_strict" (first letter
    /// must be uppercase, the rest lowercase).
    std::string letterCase = "any";

    /// Latin-letters-only fallback for games without configured rules.
    static const NameRules& defaultRules() {
        static const NameRules kDefault;
        return kDefault;
    }

    /// Fills this struct from a raw game-data map (missing keys keep the
    /// default values; unknown "case" values keep "any").
    static NameRules fromJson(
        const std::vector<std::pair<std::string, std::string>>& flat) {
        NameRules r;
        for (const auto& kv : flat) {
            const std::string& k = kv.first;
            bool v = kv.second == "true" || kv.second == "1";
            if (k == "latin") r.latin = v;
            else if (k == "cyrillic") r.cyrillic = v;
            else if (k == "digits") r.digits = v;
            else if (k == "space") r.space = v;
            else if (k == "underscore") r.underscore = v;
            else if (k == "hyphen") r.hyphen = v;
            else if (k == "symbols") r.symbols = v;
            else if (k == "case" && isCaseValue(kv.second)) r.letterCase = kv.second;
        }
        return r;
    }

    /// Checks a name against these rules. Returns a human-readable reason
    /// when the name violates the policy, or an empty string when valid.
    std::string validate(const std::string& name) const {
        if (name.empty()) {
            return "Name is required";
        }

        std::vector<uint32_t> cps;
        decodeUtf8(name, cps);

        int upper = 0, lower = 0;
        int firstLetterUpper = -1; // -1 unset, 0 false, 1 true
        for (uint32_t cp : cps) {
            Class cls = classify(cp);
            if (!allows(cls)) {
                return "Name cannot contain " + classLabel(cls);
            }
            if (cls == Class::Latin || cls == Class::Cyrillic) {
                bool up = isUpperCp(cp);
                if (firstLetterUpper < 0) {
                    firstLetterUpper = up ? 1 : 0;
                }
                if (up) upper++; else lower++;
            }
        }

        if (letterCase == "lower" && upper > 0) {
            return "Name must be lowercase";
        }
        if (letterCase == "upper" && lower > 0) {
            return "Name must be uppercase";
        }
        if (letterCase == "capitalize"
                && (upper > 1 || (upper == 1 && firstLetterUpper == 0))) {
            return "Only the first letter of the name may be uppercase";
        }
        if (letterCase == "capitalize_strict"
                && (upper != 1 || firstLetterUpper != 1)) {
            return "Name must start with a capital letter followed by lowercase";
        }

        return std::string();
    }

private:
    enum class Class { Latin, Cyrillic, Digits, Space, Underscore, Hyphen, Symbols };

    static bool isCaseValue(const std::string& v) {
        return v == "any" || v == "lower" || v == "upper"
            || v == "capitalize" || v == "capitalize_strict";
    }

    bool allows(Class cls) const {
        switch (cls) {
            case Class::Latin: return latin;
            case Class::Cyrillic: return cyrillic;
            case Class::Digits: return digits;
            case Class::Space: return space;
            case Class::Underscore: return underscore;
            case Class::Hyphen: return hyphen;
            default: return symbols;
        }
    }

    static std::string classLabel(Class cls) {
        switch (cls) {
            case Class::Latin: return "Latin letters";
            case Class::Cyrillic: return "Cyrillic letters";
            case Class::Digits: return "digits";
            case Class::Space: return "spaces";
            case Class::Underscore: return "underscores";
            case Class::Hyphen: return "hyphens";
            default: return "symbols";
        }
    }

    static bool inRanges(const uint32_t* rs, size_t nPairs, uint32_t cp) {
        size_t lo = 0, hi = nPairs;
        while (lo < hi) {
            size_t mid = (lo + hi) / 2;
            uint32_t a = rs[mid * 2], b = rs[mid * 2 + 1];
            if (cp < a) hi = mid;
            else if (cp > b) lo = mid + 1;
            else return true;
        }
        return false;
    }

    // Script=Latin ≈ chars inside the Latin Unicode blocks (the ranges are
    // almost exclusively letters; unassigned/exotic codepoints may classify
    // here where the server says symbols — a negligible, safe-side gap).
    static const uint32_t kLatinRanges[28];
    // Script=Cyrillic covers whole blocks including combining marks.
    static const uint32_t kCyrillicRanges[8];
    // \p{Nd} — every decimal-digit range.
    static const uint32_t kDigitRanges[128];

    static Class classify(uint32_t cp) {
        if (inRanges(kLatinRanges, sizeof(kLatinRanges) / (2 * sizeof(uint32_t)), cp)) {
            return Class::Latin;
        }
        if (inRanges(kCyrillicRanges, sizeof(kCyrillicRanges) / (2 * sizeof(uint32_t)), cp)) {
            return Class::Cyrillic;
        }
        if (inRanges(kDigitRanges, sizeof(kDigitRanges) / (2 * sizeof(uint32_t)), cp)) {
            return Class::Digits;
        }
        if ((cp >= 0x09 && cp <= 0x0D) || cp == 0x20 || cp == 0x85 || cp == 0xA0
                || (cp >= 0x1680 && cp <= 0x1680) || (cp >= 0x2000 && cp <= 0x200A)
                || cp == 0x2028 || cp == 0x2029 || cp == 0x202F || cp == 0x205F
                || cp == 0x3000) {
            return Class::Space;
        }
        if (cp == '_') {
            return Class::Underscore;
        }
        if (cp == '-' || (cp >= 0x2010 && cp <= 0x2015)) {
            return Class::Hyphen;
        }

        return Class::Symbols;
    }

    // \p{Lu}: a char is uppercase-form iff lowercasing changes it. wchar_t is
    // 16-bit on Windows — astral cased letters (Latin Extended-G) classify
    // as lowercase there; harmless since they need "symbols" enabled anyway.
    static bool isUpperCp(uint32_t cp) {
        if (cp == 0 || cp > 0xFFFF) {
            return false;
        }
        wchar_t wc = (wchar_t) cp;
        return std::towlower(wc) != wc;
    }

    static void decodeUtf8(const std::string& s, std::vector<uint32_t>& cps) {
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
            i += len;
        }
    }
};

inline const uint32_t NameRules::kLatinRanges[] = {
    0x41,0x5A, 0x61,0x7A, 0xC0,0x2AF, 0x2B0,0x2FF,
    0x1D00,0x1DBF, 0x1E00,0x1EFF, 0x2C60,0x2C7F, 0xA720,0xA7FF,
    0xAB30,0xAB6F, 0xFB00,0xFB06, 0xFF21,0xFF3A, 0xFF41,0xFF5A,
    0x10780,0x107BA, 0x1DF00,0x1DF1E,
};
inline const uint32_t NameRules::kCyrillicRanges[] = {
    0x400,0x52F, 0x2DE0,0x2DFF, 0xA640,0xA69F, 0x1C80,0x1C8F,
};
inline const uint32_t NameRules::kDigitRanges[] = {
    0x30,0x39, 0x660,0x669, 0x6F0,0x6F9, 0x7C0,0x7C9,
    0x966,0x96F, 0x9E6,0x9EF, 0xA66,0xA6F, 0xAE6,0xAEF,
    0xB66,0xB6F, 0xBE6,0xBEF, 0xC66,0xC6F, 0xCE6,0xCEF,
    0xD66,0xD6F, 0xDE6,0xDEF, 0xE50,0xE59, 0xED0,0xED9,
    0xF20,0xF29, 0x1040,0x1049, 0x1090,0x1099, 0x17E0,0x17E9,
    0x1810,0x1819, 0x1946,0x194F, 0x19D0,0x19D9, 0x1A80,0x1A89,
    0x1A90,0x1A99, 0x1B50,0x1B59, 0x1BB0,0x1BB9, 0x1C40,0x1C49,
    0x1C50,0x1C59, 0xA620,0xA629, 0xA8D0,0xA8D9, 0xA900,0xA909,
    0xA9D0,0xA9D9, 0xA9F0,0xA9F9, 0xAA50,0xAA59, 0xABF0,0xABF9,
    0xFF10,0xFF19, 0x104A0,0x104A9, 0x10D30,0x10D39, 0x11066,0x1106F,
    0x110F0,0x110F9, 0x11136,0x1113F, 0x111D0,0x111D9, 0x112F0,0x112F9,
    0x11450,0x11459, 0x114D0,0x114D9, 0x11650,0x11659, 0x116C0,0x116C9,
    0x11730,0x11739, 0x118E0,0x118E9, 0x11950,0x11959, 0x11C50,0x11C59,
    0x11D50,0x11D59, 0x11DA0,0x11DA9, 0x11F50,0x11F59, 0x16A60,0x16A69,
    0x16AC0,0x16AC9, 0x16B50,0x16B59, 0x1D7CE,0x1D7FF, 0x1E140,0x1E149,
    0x1E2F0,0x1E2F9, 0x1E4F0,0x1E4F9, 0x1E950,0x1E959, 0x1FBF0,0x1FBF9,
};

} // namespace moderation
} // namespace multiplayer
} // namespace michitai
