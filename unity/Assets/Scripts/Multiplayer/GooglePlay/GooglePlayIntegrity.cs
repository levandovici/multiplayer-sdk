using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.GooglePlay
{
    /// <summary>
    /// Play Integrity token minting for the backend ownership check —
    /// the Google-side analogue of Steam's CheckAppOwnership step. The token
    /// is decoded server-side (playintegrity.googleapis.com) where the
    /// verdicts are judged; the client never sees or trusts the result.
    /// Returns null off Android.
    /// </summary>
    public static class GooglePlayIntegrity
    {
        /// <summary>
        /// Requests a classic integrity token with a random nonce. The nonce
        /// binds the token to this request; the server additionally validates
        /// the request details before trusting the licensing verdict.
        /// </summary>
        public static async Task<string> RequestTokenAsync()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            byte[] raw = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(raw);
            }
            string nonce = Convert.ToBase64String(raw);

            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject manager = new AndroidJavaClass(
                    "com.google.android.play.integrity.IntegrityManagerFactory")
                .CallStatic<AndroidJavaObject>("create", activity);

            AndroidJavaObject request = new AndroidJavaObject(
                    "com.google.android.play.integrity.IntegrityTokenRequest$Builder")
                .Call<AndroidJavaObject>("setNonce", nonce)
                .Call<AndroidJavaObject>("build");

            using (AndroidJavaObject result = await GmsTask.Await(
                manager.Call<AndroidJavaObject>("requestIntegrityToken", request)))
            {
                return result.Call<string>("token");
            }
#else
            await Task.CompletedTask;
            return null;
#endif
        }
    }
}
