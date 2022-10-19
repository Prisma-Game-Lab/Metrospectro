using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Interactable : NetworkBehaviour
{
    private bool _interacted;
    private Animator _animator;
    private static readonly int Interacted = Animator.StringToHash("Interacted");
    
    [SerializeField] private GameObject prefabToSpawn;
    private NetworkObject _spawnedExplorer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _interacted = false;
    }

    public void OnInteract()
    {
        //if (AnimatorIsPlaying()) return;
        
        if (!_interacted)
        {
            //_animator.SetBool(Interacted, true);
            var explorerInstance = Instantiate(prefabToSpawn);
            _spawnedExplorer = explorerInstance.GetComponent<NetworkObject>();
            _spawnedExplorer.Spawn();

        }
        else
        {
            _spawnedExplorer.Despawn();
            //_animator.SetBool(Interacted, false);
        }

        _interacted = !_interacted;
    }

    private bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    
    
}

