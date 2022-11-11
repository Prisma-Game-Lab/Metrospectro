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
    [SerializeField] public Image roleImage;
    [SerializeField] private Toggle isReadyToggle;

     [SerializeField] private Sprite[] images;

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

    public void SwitchRoleImage()
    {
        roleImage.sprite = roleImage.sprite == images[0] ? images[1] : images[0];
    }

    public void SetRoleImage(int image)
    {
        roleImage.sprite = images[image];
    }
}

