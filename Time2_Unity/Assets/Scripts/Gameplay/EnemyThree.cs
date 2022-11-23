using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyThree : MonoBehaviour
{
    private Explorer _explorer;
    [SerializeField] private Direction invertDirection;
    [SerializeField] private float invertDuration;

    private void Start()
    {
        _explorer = FindObjectOfType<Explorer>();
    }

    private IEnumerator Execute()
    {
        SetInvertedInputs(true);
        Debug.Log("Invertido");
        yield return new WaitForSeconds(invertDuration);
        SetInvertedInputs(false);
        Debug.Log("Normal");
    }

    private  void DefineResult(Direction dir)
    {
        _explorer.OnExplorerRotate -= DefineResult;
        if (dir == invertDirection)
        {
            StopAllCoroutines();
             StartCoroutine(Execute());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetInvertedInputs(bool val)
    {
        _explorer.invertedInput = val;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explorer"))
        {
            _explorer = other.GetComponent<Explorer>();
            _explorer.OnExplorerRotate += DefineResult;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Explorer"))
        {
            _explorer.OnExplorerRotate -= DefineResult;
        }
    }
    
}
