using Unity.Netcode;
using UnityEngine;

public class Spawner : Interactable
{
    [SerializeField] private GameObject prefabToSpawn;
    private NetworkObject _spawnedExplorer;
    protected override void Interact()
    {
        var explorerInstance = Instantiate(prefabToSpawn);
        _spawnedExplorer = explorerInstance.GetComponent<NetworkObject>();
        _spawnedExplorer.Spawn();
    }

    protected override void ComeBack()
    {
        _spawnedExplorer.Despawn();
    }
}
