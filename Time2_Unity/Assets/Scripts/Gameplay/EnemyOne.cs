using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;

public class EnemyOne : MonoBehaviour
{
    private void Awake()
    {
        var storyTeller = ServerGameNetPortal.Instance.GetPlayerByRole(Role.StoryTeller);
        if (!storyTeller.HasValue) return;
        if (storyTeller.Value.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            var r = GetComponent<Renderer>();

            if (r != null)
            {
                r.enabled = false;
            }
        }
    }
}
