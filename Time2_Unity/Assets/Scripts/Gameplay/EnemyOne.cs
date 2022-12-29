using UnityEngine;

public class EnemyOne : MonoBehaviour
{
    public void SetUp(Explorer exp)
    {
        var isExp = exp.IsOwner;
        if (!isExp)
        {
            var r = GetComponent<Renderer>();

            if (r != null)
            {
                r.enabled = false;
            }
        }
    }

}
