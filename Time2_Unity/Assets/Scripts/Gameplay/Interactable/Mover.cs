using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour, IInteraction
{
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private Vector3 whereToMove;
    [SerializeField] private float duration;
    private Vector3 _startingPosition;
    private bool _moving;

    private void Awake()
    {
        _startingPosition = objectToMove.transform.position;
    }

    public void Interact()
    {
        if (_moving) return;
        StartCoroutine(Move(whereToMove));
    }

    public void ComeBack()
    {
        if (_moving) return;
        StartCoroutine(Move(_startingPosition));
    }

    private IEnumerator Move(Vector3 destination)
    {
        _moving = true;
        Vector3 startingPos  = objectToMove.transform.position;
        float elapsedTime = 0;
         
        while (elapsedTime < duration)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        MapGrid.Instance.UpdateGrid();
        objectToMove.transform.position = destination;
        _moving = false;
    }
}
