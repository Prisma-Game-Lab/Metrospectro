using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameNetPortal : MonoBehaviour
{
    private int maxPlayers = 2;

    public static ServerGameNetPortal Instance => instance;
    private static ServerGameNetPortal instance;

    private Dictionary<string, PlayerData> clientData;
    private Dictionary<ulong, string> clientIdToGuid;
    private Dictionary<ulong, int> clientSceneMap;
    private bool gameInProgress;

    private const int MaxConnectionPayload = 1024;

    private GameNetPortal gameNetPortal;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameNetPortal = GetComponent<GameNetPortal>();
        gameNetPortal.OnNetworkReadied += HandleNetworkReadied;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;

        clientData = new Dictionary<string, PlayerData>();
        clientIdToGuid = new Dictionary<ulong, string>();
        clientSceneMap = new Dictionary<ulong, int>();
    }

    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

        gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    }

    public PlayerData? GetPlayerData(ulong clientId)
    {
        if (clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                Debug.LogWarning($"No player data found for client id: {clientId}");
            }
        }
        else
        {
            Debug.LogWarning($"No client guid found for client id: {clientId}");
        }

        return null;
    }

    public void StartGame()
    {
        gameInProgress = true;

        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void EndRound()
    {
        gameInProgress = false;

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void HandleNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsServer) { return; }

        gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        gameNetPortal.OnClientSceneChanged += HandleClientSceneChanged;

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

        if (NetworkManager.Singleton.IsHost)
        {
            clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        clientSceneMap.Remove(clientId);

        if (clientIdToGuid.TryGetValue(clientId, out string guid))
        {
            clientIdToGuid.Remove(clientId);

            if (clientData[guid].ClientId == clientId)
            {
                clientData.Remove(guid);
            }
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            gameNetPortal.OnUserDisconnectRequested -= HandleUserDisconnectRequested;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            gameNetPortal.OnClientSceneChanged -= HandleClientSceneChanged;
        }
    }

    private void HandleClientSceneChanged(ulong clientId, int sceneIndex)
    {
        clientSceneMap[clientId] = sceneIndex;
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

        clientData.Add(clientGuid, new PlayerData(Role.Explorer, NetworkManager.Singleton.LocalClientId));
        clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);
    }

    private void ClearData()
    {
        clientData.Clear();
        clientIdToGuid.Clear();
        clientSceneMap.Clear();

        gameInProgress = false;
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

        if (gameInProgress)
        {
            gameReturnStatus = ConnectStatus.GameInProgress;
        }
        else if (clientData.Count >= maxPlayers)
        {
            gameReturnStatus = ConnectStatus.ServerFull;
        }

        if (gameReturnStatus == ConnectStatus.Success)
        {
            clientSceneMap[clientId] = connectionPayload.clientScene;
            clientIdToGuid[clientId] = connectionPayload.clientGUID;
            clientData[connectionPayload.clientGUID] = new PlayerData(Role.StoryTeller, clientId);
        }

        response.CreatePlayerObject = false;
        response.PlayerPrefabHash = 0;
        response.Approved = true;
        response.Position = null;
        response.Rotation = null;

        gameNetPortal.ServerToClientConnectResult(clientId, gameReturnStatus);

        if (gameReturnStatus != ConnectStatus.Success)
        {
            StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
        }
    }

    private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
    {
        gameNetPortal.ServerToClientSetDisconnectReason(clientId, reason);

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
}
