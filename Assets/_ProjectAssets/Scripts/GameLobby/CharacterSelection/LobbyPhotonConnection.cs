using TMPro;
using UnityEngine;

public class LobbyPhotonConnection : MonoBehaviour
{
    [Header("Managers")]
    public PhotonManager photonManager;

    [Header("Internals")]
    public GameObject startButton;
    public GameObject fightButton;
    public TextMeshProUGUI label;

    private void OnEnable()
    {
        Init();
        photonManager.OnCreatingRoom += OnCreatingRoom;
        PhotonManager.OnFailedToCreateRoom += EnableButtons;
    }

    private void OnDisable()
    {
        photonManager.OnCreatingRoom -= OnCreatingRoom;
        PhotonManager.OnFailedToCreateRoom -= EnableButtons;
    }

    private void EnableButtons()
    {
        startButton.SetActive(true);
        fightButton.SetActive(true);
    }

    private void Init()
    {
        startButton.SetActive(true);
        fightButton.SetActive(true);
        label.text = string.Empty;
    }

    public void TryJoinRoom()
    {
        startButton.SetActive(false);
        fightButton.SetActive(false);
        label.text = string.Empty;

        photonManager.ConnectToRandomRoom();
    }

    private void OnCreatingRoom()
    {
        label.text = "No open match. Making a new one...";
    }
}
