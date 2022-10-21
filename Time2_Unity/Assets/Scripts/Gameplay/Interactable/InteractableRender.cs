using System;
using System.Collections;
using UnityEngine;

public class InteractableRender : MonoBehaviour
{
    [SerializeField] private bool lookAtPlayer;
    private Transform _player;
    private bool _isPlayerFound = false;

    public void Awake()
    {
        _player = GameObject.FindWithTag("Explorer")?.transform;
        if (_player == null)
        {
            StartCoroutine(LookForPlayer());
        }
    }

    private IEnumerator LookForPlayer()
    {
        while (_player != null)
        {
            yield return new WaitForSeconds(2);
            _player = GameObject.FindWithTag("Explorer")?.transform;
        }

        _isPlayerFound = true;
    }

    private void OnWillRenderObject()
    {
        if (!lookAtPlayer || !_isPlayerFound) return;
        
        var position = _player.position;
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
    }
}
