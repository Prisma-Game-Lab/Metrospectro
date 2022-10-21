using Unity.Netcode;
using UnityEngine;


public class SpawnerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject explorerPrefab;
    private NetworkObject _spawnedExplorer;
    
    [SerializeField] private GameObject storytellerPrefab;
    private NetworkObject _spawnedStoryteller;

    public override void OnNetworkSpawn()
    {
        if (!IsServer || explorerPrefab == null || storytellerPrefab == null)
        {
            return;
        }
        var explorerInstance = Instantiate(explorerPrefab);
        var storytellerInstance = Instantiate(storytellerPrefab);

        var exp = ServerGameNetPortal.Instance.GetPlayerByRole(Role.Explorer);
        var str = ServerGameNetPortal.Instance.GetPlayerByRole(Role.StoryTeller);
        if (!str.HasValue || !exp.HasValue) return;

        _spawnedExplorer = explorerInstance.GetComponent<NetworkObject>();
        _spawnedExplorer.SpawnWithOwnership(exp.Value.ClientId, destroyWithScene:true);
        
        _spawnedStoryteller = storytellerInstance.GetComponent<NetworkObject>();
        _spawnedStoryteller.SpawnWithOwnership(str.Value.ClientId, destroyWithScene:true);

    }
}