using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explorer : NetworkBehaviour
{
    [SerializeField] private float rotationDuration = .25f;
    [SerializeField] private float movementDuration = .35f;

    private readonly Lock _lock = new Lock();
    
    private MapGrid _mapGrid;
    
    private void Awake()
    {
        _mapGrid = FindObjectOfType<MapGrid>();
    }

    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;
        
        var currentPosition = transform.position;
        
        var inputValue = context.ReadValue<Vector2>();
        var moveDirection = RotateVectorSnapped(inputValue, (int) transform.eulerAngles.y);
        
        var targetX = Mathf.FloorToInt(currentPosition.x + moveDirection.x);
        var targetZ = Mathf.FloorToInt(currentPosition.z + moveDirection.y);
        
        if (!_lock.IsLocked() && !_mapGrid.IsCellBlocked(targetX, targetZ))
        {
            StartCoroutine(MovePlayer(moveDirection));
        }
    }

    public void HandleCameraInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;
    
        var inputValue = context.ReadValue<float>();
        
        if (!_lock.IsLocked())
        {
            StartCoroutine(RotateCamera(Mathf.FloorToInt(inputValue)));
        }
    }

    public void HandleInteractionInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;

        var rayDirection = RotateVectorSnapped(new Vector2(0,1), (int) transform.eulerAngles.y);
        
        var currentPosition = transform.position;
        currentPosition.y = 1.4f;
        var direction = new Vector3(rayDirection.x, 0, rayDirection.y);
        
        var hits = new RaycastHit[5];
        Physics.RaycastNonAlloc(currentPosition, direction, hits, 2);
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            var interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();
            if (interactable == null) continue;
            Debug.Log("Found");
            interactable.OnInteract();
            break;
        }
    }



    private IEnumerator RotateCamera(float angle)
    {
        _lock.AddLock();
        Quaternion startRotation = transform.rotation;
        float t = 0;

        while (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime/rotationDuration);
            Vector3 newEulerOffset = Vector3.up * (angle * t);      
            transform.rotation = Quaternion.Euler(newEulerOffset) * startRotation;
            yield return null;
        } 
        _lock.RemoveLock();
    }

    private IEnumerator MovePlayer(Vector2 vec)
    {
        _lock.AddLock();

        var targetDirection = new Vector3(vec.x,0, vec.y);
        var finalPosition = targetDirection + transform.position;

        float t = 0;
        while (t < 1f)
        {
            yield return null;
            t = Mathf.Min(1f, t + Time.deltaTime/movementDuration);
            transform.position += targetDirection * Time.deltaTime/movementDuration;
        }
        transform.position = finalPosition;
        
        _lock.RemoveLock();
    }

    private Vector2 RotateVectorSnapped(Vector2 vec,int angle)
    {
        switch (angle)
        {
            case 90:
                return RotateToRight(vec);
            case -90: case 270:
                return RotateToLeft(vec);
            case 180: case -180:
                return Rotate180(vec);
            default:
                return vec;
        }
    }

    private Vector2 RotateToLeft(Vector2 vec) {

        return new Vector2(vec.y * -1.0f, vec.x).normalized;

    }

    private Vector2 RotateToRight(Vector2 vec) {

        return new Vector2(vec.y, vec.x * -1.0f).normalized;

    }

    private Vector2 Rotate180(Vector2 vec) {

        vec = new Vector2(vec.y, vec.x * -1);

        vec = new Vector2(vec.y, vec.x * -1);

        return vec;

    }
}
