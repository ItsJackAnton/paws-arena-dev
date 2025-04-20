using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private RecoveryHandler mainRecoveryHandler;
    [SerializeField] private GameObject connectingToRoom;
    [SerializeField] private TextMeshProUGUI connectingToRoomText;
    [SerializeField] private PhotonManager photonManager;
    [SerializeField] private LobbyPhotonConnection lobbyPhotonConnection;
    [SerializeField] private GameObject settingsHolder;
    [SerializeField] private Button settings;
    [SerializeField] private Button fightButton;
    [SerializeField] private Button tutorialButton;

    private void OnEnable()
    {
        tutorialButton.onClick.AddListener(ShowTutorial);
        settings.onClick.AddListener(ShowSettings);
        PlayerData.OnUpdatedRecoverEndDate += CheckIfShouldStopRecovering;
        fightButton.onClick.AddListener(JoinRoom);
        mainRecoveryHandler.ShowRecovery(DataManager.Instance.PlayerData.RecoveryEndDate);
    }

    private void OnDisable()
    {
        tutorialButton.onClick.RemoveListener(ShowTutorial);
        settings.onClick.RemoveListener(ShowSettings);
        PlayerData.OnUpdatedRecoverEndDate -= CheckIfShouldStopRecovering;
        fightButton.onClick.RemoveListener(JoinRoom);
    }

    private void ShowTutorial()
    {
        SceneManager.Instance.LoadTutorial();
    }

    private void ShowSettings()
    {
        settingsHolder.SetActive(true);
    }

    private void CheckIfShouldStopRecovering()
    {
        if (DataManager.Instance.PlayerData.RecoveryEndDate <= DateTime.UtcNow)
        {
            mainRecoveryHandler.StopRecovery();
        }
    }
    
    private void JoinRoom()
    {
        if (!DataManager.Instance.PlayerData.CanFight)
        {
            RecoveryMessageDisplay.Instance.ShowMessage();
            return;
        }
        
        lobbyPhotonConnection.TryJoinRoom();
        return;
        connectingToRoom.SetActive(true);

        connectingToRoomText.text = "Connecting to Multiplayer Server(" + PhotonNetwork.CloudRegion + ")...";

        photonManager.OnConnectedServer += () =>
        {
            connectingToRoomText.text = "Connected succeeded!";
            lobbyPhotonConnection.TryJoinRoom();
        };

        photonManager.Connect();
    }
    
    public void GoToConnecting()
    {
        connectingToRoom.SetActive(true);
        connectingToRoomText.text = "Connecting to Multiplayer Server(" + PhotonNetwork.CloudRegion + ")...";
    }
    
    public void TryConnectToFriendlyRoom(string _name)
    {
        if (!DataManager.Instance.PlayerData.CanFight)
        {
            RecoveryMessageDisplay.Instance.ShowMessage();
            SceneManager.Instance.LoadNftSelection();
            return;
        }

        photonManager.OnConnectedServer += () =>
        {
            connectingToRoomText.text = "Connected succeeded!";
            lobbyPhotonConnection.photonManager.JoinFriendlyRoom(_name);
        };

        photonManager.Connect();
    }

    private void Start()
    {
        if (!DataManager.Instance.PlayerData.HasPlayedTutorial)
        {
            DataManager.Instance.PlayerData.HasPlayedTutorial = true;
            SceneManager.Instance.LoadTutorial();
        }
    }
}
