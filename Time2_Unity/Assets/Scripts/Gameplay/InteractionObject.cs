using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]


public class InteractionObject : MonoBehaviour
{
    private enum State {Default,Interacted};
    private State _currentState;
    private Animator _animator;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentState = State.Default;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnInteract() 
    {
        if (_currentState == State.Default)
        {
            _animator.SetTrigger("Interacted");
        }
    }
}

