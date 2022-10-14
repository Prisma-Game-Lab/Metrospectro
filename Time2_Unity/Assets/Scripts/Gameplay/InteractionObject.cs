using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class InteractionObject : MonoBehaviour
{
    private bool _interacted;
    private Animator _animator;
    private static readonly int Interacted = Animator.StringToHash("Interacted");

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _interacted = false;
    }

    public virtual void OnInteract()
    {
        if (AnimatorIsPlaying()) return;
        
        if (!_interacted)
        {
            _animator.SetBool(Interacted, true);
        }
        else
        {
            _animator.SetBool(Interacted, false);
        }
    }

    private bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}

