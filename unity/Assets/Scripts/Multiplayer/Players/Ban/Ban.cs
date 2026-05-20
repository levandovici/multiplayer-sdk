using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Players.Ban
{
    /// <summary>
    /// Static class for ban-related operations
    /// </summary>
    public static class Ban
    {
        /// <summary>
        /// Check if an API response indicates the player is banned
        /// </summary>
        /// <param name="response">The API response</param>
        /// <returns>True if the error indicates the player is banned</returns>
        public static bool IsBanned(ApiResponse response)
        {
            if (!response.success && !string.IsNullOrEmpty(response.error))
            {
                return response.error.Contains("You are banned");
            }
            return false;
        }
    }
}
