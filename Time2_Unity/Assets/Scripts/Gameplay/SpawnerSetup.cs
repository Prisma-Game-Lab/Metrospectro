using Unity.Netcode;
using UnityEngine;

public class SpawnerSetup : NetworkBehaviour
{
    public GameObject PrefabToSpawn;
    public bool DestroyWithSpawner;        
    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;

    public override void OnNetworkSpawn()
    {
        // Only the server spawns, clients will disable this component on their side
        enabled = IsServer;            
        if (!enabled || PrefabToSpawn == null)
        {
            return;
        }
        // Instantiate the GameObject Instance
        m_PrefabInstance = Instantiate(PrefabToSpawn);

        var exp = ServerGameNetPortal.Instance.GetPlayerByRole(Role.Explorer);
        if (!exp.HasValue) return;
        // Get the instance's NetworkObject and Spawn
        m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
        m_SpawnedNetworkObject.SpawnWithOwnership(exp.Value.ClientId, destroyWithScene:true);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && DestroyWithSpawner && m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
        {
            m_SpawnedNetworkObject.Despawn();
        }
        base.OnNetworkDespawn();
    }
}