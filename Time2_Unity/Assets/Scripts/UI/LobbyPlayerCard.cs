using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject waitingForPlayerPanel;
    [SerializeField] private GameObject playerDataPanel;

    [Header("Data Display")]
    [SerializeField] private TMP_Text playerDisplayNameText;
    [SerializeField] private Image selectedCharacterImage;
    [SerializeField] private Toggle isReadyToggle;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        playerDisplayNameText.text = lobbyPlayerState.PlayerName.ToString();
        isReadyToggle.isOn = lobbyPlayerState.IsReady;
        
        waitingForPlayerPanel.SetActive(false);
        playerDataPanel.SetActive(true);
    }

    public void DisableDisplay()
    {
        waitingForPlayerPanel.SetActive(true);
        playerDataPanel.SetActive(false);
    }
}

