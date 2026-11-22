using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.GooglePlay
{
    /// <summary>
    /// Minimal Google Play Games Services v2 bridge for the Android build.
    /// Talks to the play-services-games-v2 AAR through JNI so no extra C#
    /// package is needed — the dependency comes in via the custom Gradle
    /// template. Off Android every entry point degrades to a clean
    /// unavailable/null so shared code paths keep working.
    /// </summary>
    public static class GooglePlaySignIn
    {
        /// <summary>Last sign-in error, for offline-reason logging.</summary>
        public static string LastError { get; private set; }

        private static bool _initialized;
        private static bool _available;

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject _activity;
        private static AndroidJavaObject _signInClient;
        private static AndroidJavaObject _playersClient;

        private static AndroidJavaObject Activity()
        {
            if (_activity == null)
            {
                _activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _activity;
        }

        private static AndroidJavaObject SignInClient()
        {
            if (_signInClient == null)
            {
                _signInClient = new AndroidJavaClass("com.google.android.gms.games.PlayGames")
                    .CallStatic<AndroidJavaObject>("getGamesSignInClient", Activity());
            }
            return _signInClient;
        }

        private static AndroidJavaObject PlayersClient()
        {
            if (_playersClient == null)
            {
                _playersClient = new AndroidJavaClass("com.google.android.gms.games.PlayGames")
                    .CallStatic<AndroidJavaObject>("getPlayersClient", Activity());
            }
            return _playersClient;
        }
#endif

        /// <summary>
        /// Runs PlayGamesSdk.initialize(). Idempotent — the v2 SDK performs
        /// silent sign-in itself, so a true result only means the native SDK
        /// is alive; call EnsureSignedIn before trusting an account.
        /// </summary>
        public static bool Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_initialized) return _available;

            try
            {
                new AndroidJavaClass("com.google.android.gms.games.PlayGamesSdk")
                    .CallStatic("initialize", Activity());
                _available = true;
            }
            catch (Exception e)
            {
                LastError = e.Message;
                _available = false;
            }

            _initialized = true;
            return _available;
#else
            return false;
#endif
        }

        /// <summary>
        /// True when a Play Games account is signed in. Triggers the explicit
        /// signIn() flow once when the silent sign-in hasn't completed yet.
        /// </summary>
        public static async Task<bool> EnsureSignedInAsync()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Initialize()) return false;

            try
            {
                using (AndroidJavaObject result = await GmsTask.Await(
                    SignInClient().Call<AndroidJavaObject>("isAuthenticated")))
                {
                    if (result.Call<bool>("isAuthenticated")) return true;
                }

                using (AndroidJavaObject result = await GmsTask.Await(
                    SignInClient().Call<AndroidJavaObject>("signIn")))
                {
                    return result.Call<bool>("isAuthenticated");
                }
            }
            catch (Exception e)
            {
                LastError = e.Message;
                Debug.LogWarning($"[Online] Play Games sign-in failed: {e.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// The app-scoped Play Games player_id of the signed-in account —
        /// same value the backend derives from the server auth code. Null
        /// when signed out or off Android.
        /// </summary>
        public static async Task<string> CurrentPlayerIdAsync()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // getCurrentPlayerId returns Task<String> — the getResult() value
            // is a boxed java.lang.String, unboxed via toString.
            using (AndroidJavaObject result = await GmsTask.Await(
                PlayersClient().Call<AndroidJavaObject>("getCurrentPlayerId")))
            {
                return result?.Call<string>("toString");
            }
#else
            return null;
#endif
        }

        /// <summary>
        /// Display name of the signed-in account (PlayersClient.getCurrentPlayer
        /// → Player.getDisplayName). Used only as the initial persona on
        /// account creation. Null when unavailable.
        /// </summary>
        public static async Task<string> CurrentDisplayNameAsync()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaObject player = await GmsTask.Await(
                PlayersClient().Call<AndroidJavaObject>("getCurrentPlayer")))
            {
                return player?.Call<string>("getDisplayName");
            }
#else
            return null;
#endif
        }

        /// <summary>
        /// Mints the single-use server auth code (requestServerSideAccess)
        /// the backend exchanges for the verified identity. serverClientId is
        /// the OAuth *web* client ID of the backend, not an Android client.
        /// </summary>
        public static async Task<string> RequestServerAuthCodeAsync(string serverClientId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaObject result = await GmsTask.Await(
                SignInClient().Call<AndroidJavaObject>("requestServerSideAccess", serverClientId, false)))
            {
                return result.Call<string>("getAuthCode");
            }
#else
            return null;
#endif
        }
    }

    /// <summary>
    /// Awaits a com.google.android.gms.tasks.Task via an OnCompleteListener
    /// proxy. The no-executor overload posts the callback on the Android main
    /// thread, which Unity's AndroidJavaProxy marshals back safely.
    /// </summary>
    internal static class GmsTask
    {
        public static Task<AndroidJavaObject> Await(AndroidJavaObject task)
        {
            var listener = new Listener();
            task.Call<AndroidJavaObject>("addOnCompleteListener", listener);
            return listener.Task;
        }

        private class Listener : AndroidJavaProxy
        {
            private readonly TaskCompletionSource<AndroidJavaObject> _tcs =
                new TaskCompletionSource<AndroidJavaObject>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            public Task<AndroidJavaObject> Task => _tcs.Task;

            public Listener() : base("com.google.android.gms.tasks.OnCompleteListener") { }

            // Java signature: void onComplete(Task<T> task)
            public void onComplete(AndroidJavaObject task)
            {
                try
                {
                    if (task.Call<bool>("isSuccessful"))
                    {
                        _tcs.TrySetResult(task.Call<AndroidJavaObject>("getResult"));
                    }
                    else
                    {
                        using (AndroidJavaObject ex = task.Call<AndroidJavaObject>("getException"))
                        {
                            _tcs.TrySetException(new Exception(
                                ex != null ? ex.Call<string>("getMessage") : "GMS task failed"));
                        }
                    }
                }
                catch (Exception e)
                {
                    _tcs.TrySetException(e);
                }
            }
        }
    }
}
