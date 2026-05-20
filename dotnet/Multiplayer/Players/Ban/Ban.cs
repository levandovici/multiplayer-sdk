using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Players.Ban
{
    /// <summary>
    /// Static class providing utility methods for ban-related operations.
    /// </summary>
    public static class Ban
    {
        /// <summary>
        /// Checks if an API response indicates the player is banned.
        /// </summary>
        /// <param name="response">The API response to check.</param>
        /// <returns>True if the error message indicates the player is banned, false otherwise.</returns>
        public static bool IsBanned(ApiResponse response)
        {
            if (!response.Success && response.Error != null)
            {
                return response.Error.Contains("You are banned");
            }
            return false;
        }
    }
}
