using Unity.Netcode;
using UnityEngine;

public class TransitionPortal : NetworkBehaviour
{
    [SerializeField] private string targetSceneA;
    [SerializeField] private string targetSceneB;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explorer"))
        { 
            RequestAdvanceSceneServerRpc();
        }
    }

    [ServerRpc]
    private void RequestAdvanceSceneServerRpc()
    {
        ServerGameNetPortal.Instance.LoadScene(
            ServerGameNetPortal.Instance.path == Path.A ? targetSceneA : targetSceneB);
    }
}