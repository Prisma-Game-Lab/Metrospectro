using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;

public class EnemyOne : MonoBehaviour
{
    private SpawnerSetup _spawner;

    private void Awake()
    {
        _spawner = FindObjectOfType<SpawnerSetup>();
    }

    private void SetUp()
    {
        var isExp = FindObjectOfType<Explorer>().IsOwner;
        if (!isExp)
        {
            var r = GetComponent<Renderer>();

            if (r != null)
            {
                r.enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        _spawner.OnExplorerSpawn += SetUp;
    }
    
    private void OnDisable()
    {
        _spawner.OnExplorerSpawn -= SetUp;
    }
}
