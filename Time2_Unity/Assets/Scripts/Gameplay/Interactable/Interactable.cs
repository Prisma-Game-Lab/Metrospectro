using System;
using Unity.Netcode;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
    private Transform _player;
    private IInteraction _interaction;
    private bool _interacted;
    private Animator _animator;
    
    private void Awake()
    {
        _interaction = GetComponent<IInteraction>();
    }

    public void OnInteract()
    { 
        HandleInteractionClientRpc();
    }

    [ClientRpc]
    private void HandleInteractionClientRpc()
    {
        
        if (!_interacted)
        {
            _interaction.Interact();
        }
        else
        {
            _interaction.ComeBack();
        }
        _interacted = !_interacted;
    }
}

