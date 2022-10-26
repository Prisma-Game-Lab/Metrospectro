using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameNetPortal : Singleton<ServerGameNetPortal>
{
    private const int MaxPlayers = 2;

    private Dictionary<string, PlayerData> _clientData;
    private Dictionary<ulong, string> _clientIdToGuid;
    private Dictionary<ulong, int> _clientSceneMap;
    private bool _gameInProgress;

    private const int MaxConnectionPayload = 1024;

    private GameNetPortal _gameNetPortal;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _gameNetPortal = GetComponent<GameNetPortal>();
        _gameNetPortal.OnNetworkReadied += HandleNetworkReadied;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;

        _clientData = new();
        _clientIdToGuid = new();
        _clientSceneMap = new();
    }

    private void OnDestroy()
    {
        if (_gameNetPortal == null) { return; }

        _gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    }

    public PlayerData? GetPlayerByRole(Role role)
    {
        foreach (var playerData in _clientData.Values)
        {
            if (playerData.PlayerRole == role) return playerData;
        }
        return null;
    }

    public PlayerData GetPlayerData(ulong clientId)
    {
        if (_clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (_clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                Debug.LogWarning($"Nenhum jogador com o id: {clientId}");
            }
        }
        else
        {
            Debug.LogWarning($"Nenhum jogador com o id: {clientId}");
        }

        return new PlayerData();
    }

    public void StartGame()
    {
        _gameInProgress = true;

        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void EndGame()
    {
        _gameInProgress = false;

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void HandleNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsServer) { return; }

        _gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        _gameNetPortal.OnClientSceneChanged += HandleClientSceneChanged;

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

        if (NetworkManager.Singleton.IsHost)
        {
            _clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        _clientSceneMap.Remove(clientId);

        if (_clientIdToGuid.TryGetValue(clientId, out string guid))
        {
            _clientIdToGuid.Remove(clientId);

            if (_clientData[guid].ClientId == clientId)
            {
                _clientData.Remove(guid);
            }
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            _gameNetPortal.OnUserDisconnectRequested -= HandleUserDisconnectRequested;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            _gameNetPortal.OnClientSceneChanged -= HandleClientSceneChanged;
        }
    }

    private void HandleClientSceneChanged(ulong clientId, int sceneIndex)
    {
        _clientSceneMap[clientId] = sceneIndex;
    }

    private void HandleUserDisconnectRequested()
    {
        HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);

        NetworkManager.Singleton.Shutdown();

        ClearData();

        SceneManager.LoadScene("MainMenu");
    }

    private void HandleServerStarted()
    {
        if (!NetworkManager.Singleton.IsHost) { return; }

        string clientGuid = Guid.NewGuid().ToString();

        _clientData.Add(clientGuid, new PlayerData(Role.Explorer, NetworkManager.Singleton.LocalClientId));
        _clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);
    }

    private void ClearData()
    {
        _clientData.Clear();
        _clientIdToGuid.Clear();
        _clientSceneMap.Clear();

        _gameInProgress = false;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        var connectionData = request.Payload;
        
        if (connectionData.Length > MaxConnectionPayload)
        {
            response.CreatePlayerObject = false;
            response.PlayerPrefabHash = 0;
            response.Approved = false;
            response.Position = null;
            response.Rotation = null;
            return;
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            response.CreatePlayerObject = false;
            response.PlayerPrefabHash = null;
            response.Approved = true;
            response.Position = null;
            response.Rotation = null;
            return;
        }

        string payload = Encoding.UTF8.GetString(connectionData);
        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        ConnectStatus gameReturnStatus = ConnectStatus.Success;

        if (_gameInProgress)
        {
            gameReturnStatus = ConnectStatus.GameInProgress;
        }
        else if (_clientData.Count >= MaxPlayers)
        {
            gameReturnStatus = ConnectStatus.ServerFull;
        }

        if (gameReturnStatus == ConnectStatus.Success)
        {
            _clientSceneMap[clientId] = connectionPayload.clientScene;
            _clientIdToGuid[clientId] = connectionPayload.clientGUID;
            _clientData[connectionPayload.clientGUID] = new PlayerData(Role.StoryTeller, clientId);
        }

        response.CreatePlayerObject = false;
        response.PlayerPrefabHash = 0;
        response.Approved = true;
        response.Position = null;
        response.Rotation = null;

        _gameNetPortal.ServerToClientConnectResult(clientId, gameReturnStatus);

        if (gameReturnStatus != ConnectStatus.Success)
        {
            StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
        }
    }

    private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
    {
        _gameNetPortal.ServerToClientSetDisconnectReason(clientId, reason);

        yield return new WaitForSeconds(0);

        KickClient(clientId);
    }

    private void KickClient(ulong clientId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (networkObject != null)
        {
            networkObject.Despawn(true);
        }

        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    public void SetRole(ulong clientId, Role role)
    {
        _clientData[_clientIdToGuid[clientId]] = new PlayerData(
            role,
            clientId);
    }
}
