using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : NetworkBehaviour
{
    public GameObject pauseMenuUI;
    private static bool _isPaused;

    private readonly List<InputActionMap> _actionMaps = new();

    public override void OnNetworkSpawn()
    {
        var inputs = FindObjectsOfType<PlayerInput>();

        foreach (var input in inputs)
        {
            if (input.currentActionMap.name == "PauseMenu") continue;
            _actionMaps.Add(input.currentActionMap);
        }

    }
    public void HandlePauseInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        if (_isPaused)
        {
            RequestResumeServerRPC();
        }
        else
        {
            RequestPauseServerRPC();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RequestResumeServerRPC()
    {
        ResumeClientRPC();
    }
    
    [ClientRpc]
    private void ResumeClientRPC()
    {
        EnableInputs();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestPauseServerRPC()
    {
        PauseClientRPC();
    }
    
    [ClientRpc]
    private void PauseClientRPC()
    {
        DisableInputs();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadMenuServerRPC()
    {
        RestoreTimeClientRPC();
        ServerGameNetPortal.Instance.EndGame();
    }

    [ClientRpc]
    private void RestoreTimeClientRPC()
    {
        Time.timeScale = 1f;
    }

    private void DisableInputs()
    {
        foreach (var map in _actionMaps)
        {
            map.Disable();
        }
    }
    
    private void EnableInputs()
    {
        foreach (var map in _actionMaps)
        {
            map.Enable();
        }
    }
}

