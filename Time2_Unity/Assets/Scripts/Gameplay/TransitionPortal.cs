using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPortal : NetworkBehaviour
{
    [SerializeField] private string targetSceneA;
    [SerializeField] private string targetSceneB;
    
    [SerializeField] private float fadeDuration = 1; 
    [SerializeField] private GameObject fadeObject; 
    private Image _img;
     
    private void Awake()
    {
        var temp = Instantiate(fadeObject);
        _img = temp.GetComponentInChildren<Image>();
        _img.color = new Color(0, 0, 0, 0);
    }
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
        var target = ServerGameNetPortal.Instance.path == Path.A ? targetSceneA : targetSceneB;

        if (target == "FINAL") StartCoroutine(FadeToWhite());
        else ServerGameNetPortal.Instance.LoadScene(target);
    }
    
    private IEnumerator FadeToWhite()
    {
        var time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            _img.color = new Color(1, 1, 1, time/fadeDuration);
            yield return null;
        } 
        _img.color = new Color(1, 1, 1, 1);
        ServerGameNetPortal.Instance.EndGame();
    }
}