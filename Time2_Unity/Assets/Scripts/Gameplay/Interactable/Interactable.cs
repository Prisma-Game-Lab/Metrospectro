using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Interactable : NetworkBehaviour
{
    [SerializeField] private bool lookAtPlayer;

    private Transform _player;

    private InteractableRender _interactableRender;
    private bool _interacted;
    private Animator _animator;
    private static readonly int Interacted = Animator.StringToHash("Interacted");
    
    protected virtual void Awake()
    {
        _interactableRender = GetComponentInChildren<InteractableRender>();
        //_animator = GetComponent<Animator>();
        _interacted = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _player = GameObject.FindWithTag("Explorer").transform;
        _interactableRender.Init(lookAtPlayer, _player);
    }

    public void OnInteract()
    { 
        //if (AnimatorIsPlaying()) return;
        
        if (!_interacted)
        {
            //_animator.SetBool(Interacted, true);
            Interact();
        }
        else
        {
            ComeBack();
            //_animator.SetBool(Interacted, false);
        }
        _interacted = !_interacted;
    }
    
    protected virtual void Interact()
    {
        throw new NotImplementedException();
    }

    protected virtual void ComeBack()
    {
        throw new NotImplementedException();
    }
    
    private bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}

