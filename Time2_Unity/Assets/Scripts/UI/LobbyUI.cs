using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button changeRolesButton;
    [SerializeField] private TextMeshProUGUI roomCodeTMP;

    public string RoomCode
    {
        set => roomCodeTMP.text = $"{value}";
    }
    

    private NetworkList<LobbyPlayerState> _lobbyPlayers;

    private void Awake()
    {
        _lobbyPlayers = new NetworkList<LobbyPlayerState>();
        RoomCode = RelayManager.RoomCode;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
        }

        if (IsServer)
        {
            startGameButton.gameObject.SetActive(true);
            changeRolesButton.gameObject.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private bool IsEveryoneReady()
    {
        if (_lobbyPlayers.Count < 2)
        {
            return false;
        }

        foreach (var player in _lobbyPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    private void HandleClientConnected(ulong clientId)
    {
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);


        _lobbyPlayers.Add(new LobbyPlayerState(
            clientId,
            $"Jogador {playerData.ClientId + 1}",
            playerData.PlayerRole,
            false
        ));
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < _lobbyPlayers.Count; i++)
        {
            if (_lobbyPlayers[i].ClientId == clientId)
            {
                _lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _lobbyPlayers.Count; i++)
        {
            if (_lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                _lobbyPlayers[i] = new LobbyPlayerState(
                    _lobbyPlayers[i].ClientId,
                    _lobbyPlayers[i].PlayerName,
                    _lobbyPlayers[i].PlayerRole,
                    !_lobbyPlayers[i].IsReady
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

        if (!IsEveryoneReady()) { return; }

        ServerGameNetPortal.Instance.StartGame();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ChangeRolesServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _lobbyPlayers.Count; i++)
        {
            var role = _lobbyPlayers[i].PlayerRole == Role.Explorer ? Role.StoryTeller : Role.Explorer;
            ServerGameNetPortal.Instance.SetRole(_lobbyPlayers[i].ClientId, role);
            _lobbyPlayers[i] = new LobbyPlayerState(
                    _lobbyPlayers[i].ClientId,
                    _lobbyPlayers[i].PlayerName,
                    role,
                    _lobbyPlayers[i].IsReady
                );
        }
    }

    public void OnLeaveClicked()
    {
        GameNetPortal.Instance.RequestDisconnect();
    }

    public void OnReadyClicked()
    {
        ToggleReadyServerRpc();
    }
    
    public void OnChageRolesClicked()
    {
        ChangeRolesServerRpc();
    }

    public void OnStartGameClicked()
    {
        StartGameServerRpc();
    }
    
    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            if (_lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(_lobbyPlayers[i]);
            }
            else
            {
                lobbyPlayerCards[i].DisableDisplay();
            }
        }

        if(IsHost)
        {
            startGameButton.interactable = IsEveryoneReady();
            changeRolesButton.interactable = IsEveryoneReady();
        }
    }
}

