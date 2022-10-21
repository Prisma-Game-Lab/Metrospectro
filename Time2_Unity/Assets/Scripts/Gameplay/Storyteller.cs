using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Battery))]
public class Storyteller : NetworkBehaviour
{
    [SerializeField] private GameObject lights;

    private readonly Lock _lock = new Lock();

    private Battery _battery;
    [SerializeField] private Image mapImage;
    [SerializeField] private Image batteryImage;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            mapImage.gameObject.SetActive(false);
            batteryImage.gameObject.SetActive(false);
        }
        else
        {
            lights.SetActive(true);
        }
    }

    private void Awake()
    {
        _battery = GetComponent<Battery>();
        batteryImage.enabled = false;
        mapImage.enabled = true;
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner || _lock.IsLocked()) return;

        var isMapEnabled = mapImage.enabled;
        mapImage.enabled = !isMapEnabled;
        batteryImage.enabled = isMapEnabled;
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
        mapImage.enabled = true;
    }
    
    private void OnChangeTime(float currentPercentage)
    {
        batteryImage.fillAmount = currentPercentage;
    }
}
