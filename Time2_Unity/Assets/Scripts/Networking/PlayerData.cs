public struct PlayerData
{
    public Role PlayerRole { get;  set; }
    public ulong ClientId { get; private set; }

    public PlayerData(Role playerRole, ulong clientId)
    {
        PlayerRole = playerRole;
        ClientId = clientId;
    }
}

