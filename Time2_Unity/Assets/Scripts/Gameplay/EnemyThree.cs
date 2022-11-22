using System;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyThree : MonoBehaviour
{
    private Explorer _explorer;
    [SerializeField] private bool direita;

    private void Start()
    {
        _explorer = FindObjectOfType<Explorer>();
    }

    private async Task Execute()
    {
        SetInvertedInputs(true);
        Debug.Log("Invertido");
        await Task.Delay(5000);
        SetInvertedInputs(false);
        Debug.Log("Normal");
    }

    private void SetInvertedInputs(bool val)
    {
        _explorer.invertedInput = val;
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explorer"))
        {
            _explorer = other.GetComponent<Explorer>();
            await Execute();
        }
    }
    
}
