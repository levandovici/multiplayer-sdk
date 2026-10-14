namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Request body for accepting/declining a lobby or room invite. On accept
    /// an optional player_data object is stored with the new member.
    /// </summary>
    internal class FriendInviteRespondRequest<T> where T : class, new()
    {
        public string? Invite_id { get; set; }

        public T? Player_data { get; set; }

        public FriendInviteRespondRequest(string inviteId, T? playerData = null)
        {
            this.Invite_id = inviteId;
            this.Player_data = playerData;
        }
    }
}
