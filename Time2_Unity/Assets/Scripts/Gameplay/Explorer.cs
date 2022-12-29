using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explorer : NetworkBehaviour
{
    public bool invertedInput;

    [SerializeField] private float rotationDuration = .25f;
    [SerializeField] private float movementDuration = .35f;
    
    private readonly Lock _lock = new Lock();
    
    public delegate void NotifyOnRotate(Direction dir);
    public event NotifyOnRotate OnExplorerRotate;
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RequestCheckEnemiesServerRpc();
    }

    [ServerRpc (RequireOwnership = false)]
    private void RequestCheckEnemiesServerRpc()
    {
        CheckEnemiesClientRpc();
    }

    [ClientRpc]
    private void CheckEnemiesClientRpc()
    {
        var inimigos = FindObjectsOfType<EnemyOne>();
        foreach (var inimigo in inimigos)
        {
            inimigo.SetUp(this);
        }
    }
    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        if (!IsOwner || !context.performed) return;
        
        var currentPosition = transform.position;

        
        
        var inputValue = context.ReadValue<Vector2>();
        if (inputValue.x != 0) inputValue.y = 0;
        
        var moveDirection = inputValue.RotateVectorSnapped((int) transform.eulerAngles.y);
        
        if (invertedInput)
        {
            moveDirection = new Vector2(-1 * moveDirection.x, -1 * moveDirection.y);
        }


        
        var targetX = Mathf.FloorToInt(currentPosition.x + moveDirection.x);
        var targetZ = Mathf.FloorToInt(currentPosition.z + moveDirection.y);

        var cellBlocked = MapGrid.Instance.IsCellBlocked(targetX, targetZ);

        if (!cellBlocked) AudioManager.Instance.PlayStep(moveDirection);
        else if (!_lock.IsLocked())
        {
            AudioManager.Instance.PlayCollide(moveDirection);
        }
        
        if (!cellBlocked && !_lock.IsLocked())
        {
            MovePlayerServerRPC(moveDirection.x, moveDirection.y);
        }
    }

    [ServerRpc]
    private void MovePlayerServerRPC(float x, float y)
    {
        MovePlayerClientRPC(x, y);
    }
    
    [ClientRpc]
    private void MovePlayerClientRPC(float x, float y)
    {
        StartCoroutine(MovePlayer(x, y));
    }
    
    public void HandleCameraInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;
    
        var inputValue = context.ReadValue<float>();
        
        OnExplorerRotate?.Invoke(inputValue > 0? Direction.Right : Direction.Left);

        if (invertedInput)
        {
            inputValue = -1 * inputValue;
        }
        
        if (!_lock.IsLocked())
        {
            RotateCameraServerRPC(inputValue);
        }
    }
    
    [ServerRpc]
    private void RotateCameraServerRPC(float angle)
    {
        RotateCameraClientRPC(angle);
    }
    
    [ClientRpc]
    private void RotateCameraClientRPC(float angle)
    {
        StartCoroutine(RotateCamera(Mathf.FloorToInt(angle)));
    }

    public void HandleInteractionInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner) return;

        var rayDirection = new Vector2(0,1).RotateVectorSnapped((int) transform.eulerAngles.y);
        
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

    private IEnumerator MovePlayer(float x, float y)
    {
        _lock.AddLock();

        var targetDirection = new Vector3(x,0, y);
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
}

public enum Direction {Left, Right};
