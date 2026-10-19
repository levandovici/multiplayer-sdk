#pragma once

/// Minimal Steam client helper for the C++ SDK: initializes the Steamworks
/// API, pumps callbacks, and mints single-use web auth tickets for
/// Players::registerPlayerWithSteam. Auth-only by design.
///
/// Unlike the unity/.NET SDKs there is no shim here — this is a thin wrapper
/// over the real Steamworks SDK. Requirements:
///   - Steamworks SDK >= 1.57 on your include path (the directory containing
///     steam/steam_api.h — i.e. <sdk>/public)
///   - link redistributable_bin/win64/steam_api64.lib (or the per-platform lib)
///   - ship steam_api64.dll next to the executable, plus a steam_appid.txt
///     when not launching through Steam (480/Spacewar works for development)
///   - the Steam client installed and logged in, otherwise init() == false
///
/// Interface names/struct layouts verified against SDK 1.65.

#include <steam/steam_api.h>
#include <chrono>
#include <cstdint>
#include <string>
#include <thread>

namespace michitai::multiplayer::steam {

/// Lowercase hex encoding of exactly `length` ticket bytes. The response
/// buffer is padded to k_nCubTicketMaxLength — only m_cubTicket bytes are
/// meaningful, never hex-encode the whole array.
inline std::string HexEncodeTicket(const uint8* data, uint32 length)
{
    static const char* digits = "0123456789abcdef";
    std::string out;
    out.reserve(static_cast<size_t>(length) * 2);
    for (uint32 i = 0; i < length; ++i)
    {
        out.push_back(digits[data[i] >> 4]);
        out.push_back(digits[data[i] & 0x0F]);
    }
    return out;
}

/// Owns the Steam callback registration and exposes a blocking-style
/// GetAuthTicketHex for tools and simple clients. Games with their own
/// frame loop can instead call RunFrame() per frame and drive the async
/// flow themselves.
class SteamTicket
{
public:
    SteamTicket() = default;
    ~SteamTicket() { Shutdown(); }

    SteamTicket(const SteamTicket&) = delete;
    SteamTicket& operator=(const SteamTicket&) = delete;

    /// SteamAPI_Init. Idempotent — returns true once initialized; false means
    /// no Steam (client not running / no steam_appid.txt / no DLL). The game
    /// decides the offline or guest policy, not the SDK.
    bool Init()
    {
        if (m_initialized) return true;
        if (!SteamAPI_Init()) return false;
        m_user = SteamUser();
        m_initialized = m_user != nullptr;
        return m_initialized;
    }

    /// True after a successful Init.
    bool IsAvailable() const { return m_initialized; }

    /// Local user's SteamID64, or 0 when unavailable.
    uint64 SteamId() const
    {
        return m_initialized ? m_user->GetSteamID().ConvertToUint64() : 0;
    }

    /// Local user's Steam persona name (usable as the register name).
    std::string PersonaName() const
    {
        if (!m_initialized) return {};
        const char* name = SteamFriends()->GetPersonaName();
        return name != nullptr ? std::string(name) : std::string();
    }

    /// Language the user picked for this game in the Steam client, as a
    /// Steam language name ("english", "schinese", "brazilian", …) — empty
    /// when unavailable. Optional interface; nullptr-safe.
    std::string GameLanguage() const
    {
        if (!m_initialized) return {};
        const ISteamApps* apps = SteamApps();
        if (apps == nullptr) return {};
        const char* lang = apps->GetCurrentGameLanguage();
        return lang != nullptr ? std::string(lang) : std::string();
    }

    /// Mints a fresh single-use web auth ticket and returns its hex encoding.
    /// Pumps SteamAPI_RunCallbacks internally while waiting — call from a
    /// thread where blocking ~seconds is acceptable. The ticket handle is
    /// canceled before returning; the hex stays valid for one server-side
    /// AuthenticateUserTicket call — never cache or retry a ticket.
    ///
    /// identity must match what the server passes as steam_identity —
    /// keep the default "michitai".
    bool GetAuthTicketHex(std::string& outHex,
                          const std::string& identity = "michitai",
                          int timeoutMs = 10000)
    {
        if (!m_initialized) return false;

        m_ticketDone = false;
        m_ticketResult = k_EResultNone;
        m_ticketHex.clear();
        m_pendingHandle = m_user->GetAuthTicketForWebApi(identity.c_str());
        if (m_pendingHandle == k_HAuthTicketInvalid) return false;

        const auto deadline =
            std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (!m_ticketDone && std::chrono::steady_clock::now() < deadline)
        {
            SteamAPI_RunCallbacks();
            std::this_thread::sleep_for(std::chrono::milliseconds(10));
        }

        const HAuthTicket handle = m_pendingHandle;
        m_pendingHandle = k_HAuthTicketInvalid;
        m_user->CancelAuthTicket(handle); // always cancel — the hex is already captured

        if (!m_ticketDone || m_ticketResult != k_EResultOK) return false;
        outHex = m_ticketHex;
        return true;
    }

    /// Pumps Steam callbacks — games should call this once per frame instead
    /// of relying on GetAuthTicketHex's internal pump.
    static void RunFrame() { SteamAPI_RunCallbacks(); }

    /// Shuts the Steam API down. Safe to call more than once.
    void Shutdown()
    {
        if (!m_initialized) return;
        m_initialized = false;
        m_user = nullptr;
        SteamAPI_Shutdown();
    }

private:
    bool m_initialized = false;
    ISteamUser* m_user = nullptr;
    bool m_ticketDone = false;
    EResult m_ticketResult = k_EResultNone;
    HAuthTicket m_pendingHandle = k_HAuthTicketInvalid;
    std::string m_ticketHex;

    STEAM_CALLBACK(SteamTicket, OnTicketForWebApi, GetTicketForWebApiResponse_t);
};

inline void SteamTicket::OnTicketForWebApi(GetTicketForWebApiResponse_t* p)
{
    if (p->m_hAuthTicket != m_pendingHandle) return; // not our request
    m_ticketResult = p->m_eResult;
    if (p->m_eResult == k_EResultOK)
        m_ticketHex = HexEncodeTicket(p->m_rgubTicket, p->m_cubTicket);
    m_ticketDone = true;
}

} // namespace michitai::multiplayer::steam
