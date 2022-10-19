using System;
using UnityEngine;

public class InteractableRender : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    private bool _lookAtPlayer;
    private Transform _player;

    public void Init(bool lookAtPlayer, Transform player)
    {
        _lookAtPlayer = lookAtPlayer;
        _player = player;
    }

    private void OnWillRenderObject()
    {
        if(_lookAtPlayer)
        {
            var position = _player.position;
            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        }
    }
}
