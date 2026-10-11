using UnityEngine;

namespace Michitai.Multiplayer.Steam
{
    /// <summary>
    /// Self-created MonoBehaviour that pumps Steam callbacks once per frame and
    /// shuts the Steam API down on application quit. Instantiated automatically
    /// before the first scene loads — no scene setup or manual wiring required.
    /// </summary>
    public sealed class SteamBehaviour : MonoBehaviour
    {
        static SteamBehaviour _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureCreated()
        {
            if (_instance != null) return;
            var go = new GameObject(nameof(SteamBehaviour));
            _instance = go.AddComponent<SteamBehaviour>();
            DontDestroyOnLoad(go);
        }

        void Update()
        {
            SteamTicket.RunFrame();
        }

        void OnApplicationQuit()
        {
            SteamTicket.Shutdown();
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
    }
}
