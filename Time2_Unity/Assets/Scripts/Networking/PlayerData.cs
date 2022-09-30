public struct PlayerData
{
    public Role PlayerRole { get; private set; }
    public ulong ClientId { get; private set; }

    public PlayerData(Role playerRole, ulong clientId)
    {
        PlayerRole = playerRole;
        ClientId = clientId;
    }
}

