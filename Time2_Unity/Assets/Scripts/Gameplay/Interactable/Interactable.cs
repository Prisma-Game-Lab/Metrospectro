using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Interactable : NetworkBehaviour
{
    private Transform _player;
    private IInteraction _interaction;
    private bool _interacted;
    private Animator _animator;
    private static readonly int Interacted = Animator.StringToHash("Interacted");
    
    private void Awake()
    {
        _interaction = GetComponent<IInteraction>();
        //_animator = GetComponent<Animator>();
    }

    public void OnInteract()
    { 
        HandleInteractionClientRpc();
    }

    [ClientRpc]
    private void HandleInteractionClientRpc()
    {
        //if (AnimatorIsPlaying()) return;
        
        if (!_interacted)
        {
            //_animator.SetBool(Interacted, true);
            _interaction.Interact();
        }
        else
        {
            _interaction.ComeBack();
            //_animator.SetBool(Interacted, false);
        }
        _interacted = !_interacted;
    }
        

    private bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}

