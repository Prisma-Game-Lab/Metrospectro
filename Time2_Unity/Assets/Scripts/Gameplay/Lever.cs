using UnityEngine.Events;

namespace Gameplay
{
    public class Lever : InteractionObject
    {
        public UnityEvent interact;

        protected override void Awake()
        {
            base.Awake();
            OnInteract();
            Invoke($"OnInteract", 5);
        }

    public override void OnInteract()
        {
            base.OnInteract();
            interact.Invoke();
        }
    }
}