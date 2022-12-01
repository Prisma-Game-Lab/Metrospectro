using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Battery))]
public class Storyteller : NetworkBehaviour
{
    private readonly Lock _lock = new Lock();

    [SerializeField] private GameObject lights;

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
            lights.SetActive(false);
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

    public void HandleCameraInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner || _lock.IsLocked()) return;
        var isMapEnabled = mapCanvas.activeSelf;
        if (!isMapEnabled) return;

        isMapEnabled = false;
        mapCanvas.SetActive(false);
        batteryCanvas.SetActive(true);
        _battery.TurnOn();
    }

    public void HandleMapInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsOwner || _lock.IsLocked()) return;
        var isMapEnabled = mapCanvas.activeSelf;
        if (isMapEnabled) return;

        isMapEnabled = true;
        mapCanvas.SetActive(true);
        batteryCanvas.SetActive(false);
        _battery.TurnOff();
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
        batteryCanvas.SetActive(false);
    }
    
    private void OnChangeTime(float currentPercentage)
    {
        var current = Mathf.FloorToInt(currentPercentage * 5);
        batteryImage.sprite = batterySprites[current];
    }
}
