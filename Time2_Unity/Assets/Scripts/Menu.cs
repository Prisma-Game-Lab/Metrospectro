using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private GameObject hostContainer;
    [SerializeField] private GameObject joinContainer;
    [SerializeField] private GameObject settingsContainer;
    
    [Header("Network Items")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private async void Start()
    {
        LoadMainContainer();
        await Authenticator.Authenticate();
        
        startClientButton.onClick.AddListener(StartClient);
        startHostButton.onClick.AddListener(StartHost);
        
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Logger.Instance.LogInfo($"Jogador {id} acaba de conectar...");
        };
    }
    
    public void LoadMainContainer()
    {
        mainContainer.SetActive(true);
        hostContainer.SetActive(false);
        joinContainer.SetActive(false);
        settingsContainer.SetActive(false);
    }

    public void LoadHostContainer()
    {
        mainContainer.SetActive(false);
        hostContainer.SetActive(true);
        joinContainer.SetActive(false);
        settingsContainer.SetActive(false);
    }  
    
    public void LoadJoinContainer()
    {
        mainContainer.SetActive(false);
        hostContainer.SetActive(false);
        joinContainer.SetActive(true);
        settingsContainer.SetActive(false);
    }
    
    public void LoadSettingsContainer()
    {
        mainContainer.SetActive(false);
        hostContainer.SetActive(false);
        joinContainer.SetActive(false);
        settingsContainer.SetActive(true);
    }

    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit(0);
    }
    
    private async void StartHost()
    {
        if (RelayManager.IsRelayEnabled) 
            await RelayManager.SetupRelay();

        NetworkManager.Singleton.StartHost();
    }

    private async void StartClient()
    {
        if (RelayManager.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.JoinRelay(joinCodeInput.text);

        NetworkManager.Singleton.StartClient();
    }
}
