using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explorer : NetworkBehaviour
{
    [SerializeField] private float rotationDuration = .5f;
    [SerializeField] private float movementDuration = .5f;
    
    private readonly Lock _lock = new Lock();
    
    private MazeSpawner _mazeSpawner;
    private CharacterController _characterController;

    private void Awake()
    {
        _mazeSpawner = FindObjectOfType<MazeSpawner>();
        _characterController = GetComponent<CharacterController>();
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner) RenderSettings.fog = true;
    }

    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;
        
        var currentPosition = transform.position;
        
        var inputValue = context.ReadValue<Vector2>();
        var moveDirection = RotateVectorSnapped(inputValue, (int) transform.eulerAngles.y);
        
        var targetX = Mathf.FloorToInt(currentPosition.x + moveDirection.x);
        var targetZ = Mathf.FloorToInt(currentPosition.z + moveDirection.y);
        
        if (!_lock.IsLocked() && !_mazeSpawner.IsCellBlocked(targetX, targetZ))
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

    private IEnumerator RotateCamera(float angle)
    {
        _lock.AddLock();
        Quaternion startRotation = transform.rotation;
        float t = 0;

        while (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime/rotationDuration);
            Vector3 newEulerOffset = Vector3.up * (angle * t);      
            // global z rotation
            transform.rotation = Quaternion.Euler(newEulerOffset) * startRotation;
            // local z rotation
            // transform.rotation = startRotation * Quaternion.Euler(newEulerOffset);
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
            t = Mathf.Min(1f, t + Time.deltaTime/movementDuration);
            _characterController.Move(targetDirection * Time.deltaTime/movementDuration);
            yield return new WaitForFixedUpdate();
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
