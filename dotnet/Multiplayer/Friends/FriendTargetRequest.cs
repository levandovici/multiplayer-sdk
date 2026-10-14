namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Request body for friend actions — targets a player by id or tag.
    /// Null members are omitted from the JSON payload.
    /// </summary>
    internal class FriendTargetRequest
    {
        public int? Player_id { get; set; }

        public string? Player_tag { get; set; }

        public FriendTargetRequest(int? playerId = null, string? playerTag = null)
        {
            this.Player_id = playerId;
            this.Player_tag = playerTag;
        }
    }
}
