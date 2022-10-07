using UnityEngine;

namespace Nightvision.Examples
{
    [RequireComponent(typeof(Nightvision))]
    public class NightvisionSwitcher : MonoBehaviour
    {
        Nightvision nightvision;

        void Start()
        {
            nightvision = GetComponent<Nightvision>();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.N))
            {
                nightvision.enabled = !nightvision.enabled;
            }
        }
    }
}
