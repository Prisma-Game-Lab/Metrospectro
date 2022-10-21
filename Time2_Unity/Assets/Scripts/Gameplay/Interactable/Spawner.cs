using System;
using Unity.Netcode;
using UnityEngine;

public class Spawner :  MonoBehaviour, IInteraction
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private bool spawnLocally;
    private GameObject _spawnedExplorer;
    
    public void Interact()
    {
        _spawnedExplorer = Instantiate(prefabToSpawn);
        if(!spawnLocally)
            _spawnedExplorer.GetComponent<NetworkObject>().Spawn();
    }

    public void ComeBack()
    {
        if(!spawnLocally)
            _spawnedExplorer.GetComponent<NetworkObject>().Despawn();
        else
            Destroy(_spawnedExplorer);
    }
}
