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
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject batteryCanvas;
    [SerializeField] private Image batteryImage;
    [SerializeField] private Sprite[] batterySprites;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            mapCanvas.gameObject.SetActive(false);
            batteryCanvas.gameObject.SetActive(false);
        }
        else
        {
            lights.SetActive(true);
        }
    }

    private void Awake()
    {
        _battery = GetComponent<Battery>();
        batteryCanvas.SetActive(false);
        mapCanvas.SetActive(true);
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner || _lock.IsLocked()) return;

        var isMapEnabled = mapCanvas.activeSelf;
        mapCanvas.SetActive(!isMapEnabled);
        batteryCanvas.SetActive(isMapEnabled);
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
        mapCanvas.SetActive(true);
    }
    
    private void OnChangeTime(float currentPercentage)
    {
        var current = Mathf.FloorToInt(currentPercentage / 20);
        batteryImage.sprite = batterySprites[current];
    }
}
