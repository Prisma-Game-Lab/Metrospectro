using System;
using System.Collections;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Battery))]
public class Storyteller : NetworkBehaviour
{
    [SerializeField] private RenderTexture map;
    [SerializeField] private GameObject lights;

    private readonly Lock _lock = new Lock();

    private Battery _battery;
    private RawImage _mapImage;
    private Image _batteryImage;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            _mapImage.gameObject.SetActive(false);
            _batteryImage.gameObject.SetActive(false);
        }
        else
        {
            lights.SetActive(true);
        }
    }

    private void Awake()
    {
        _mapImage = GetComponentInChildren<RawImage>();
        _batteryImage = GetComponentInChildren<Image>();
        _battery = GetComponent<Battery>();
        _mapImage.texture = map;
        _batteryImage.enabled = false;
        _mapImage.enabled = true;
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner || _lock.IsLocked()) return;

        var isMapEnabled = _mapImage.enabled;
        _mapImage.enabled = !isMapEnabled;
        _batteryImage.enabled = isMapEnabled;
        _battery.SwitchPower();

    }

    private void OnEnable()
    {
        _battery.OnEndTime += OnEndTime;
        _battery.OnChangeTime += OnChangeTime;
    }
    
    private void OnDisable()
    {
        _battery.OnEndTime -= OnEndTime;
        _battery.OnChangeTime -= OnChangeTime;
    }

    private void OnEndTime()
    {
        _lock.AddLock();
        _mapImage.enabled = true;
    }
    
    private void OnChangeTime(float currentPercentage)
    {
        _batteryImage.fillAmount = currentPercentage;
    }
}
