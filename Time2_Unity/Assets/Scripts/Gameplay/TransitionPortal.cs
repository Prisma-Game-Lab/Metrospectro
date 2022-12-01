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
        if (ServerGameNetPortal.Instance.path == Path.A)
            ServerGameNetPortal.Instance.LoadScene(targetSceneA);
        else
            ServerGameNetPortal.Instance.LoadScene(targetSceneB);
    }
}