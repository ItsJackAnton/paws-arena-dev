using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Password")]
    public GameObject passwordScreen;

    [Header("Connecting")]
    public GameObject connectingToServerScreen;

    [Header("NFT Selection")]
    public List<GameObject> nftSelectionScreens;

    [Header("Equipment")]
    public List<GameObject> equipmentScreens;

    [Header("Game Menu")]
    public List<GameObject> gameMenuScreens;
    //public GameObject gameMenuSprites;

    [Header("Connecting")]
    public GameObject connectingToRoom;
    public TMPro.TextMeshProUGUI connectingToRoomText;
    public PhotonManager photonManager;
    public LobbyPhotonConnection lobbyPhotonConnection;

    [Header("Settings")]
    public GameObject settings;

    [Header("Others")]
    public GameObject loadingScreen;


    private void OnEnable()
    {
        if (GameState.selectedNFT != null)
        {
            OpenGameMenu();
        }
    }

    private void OnDestroy()
    {
        LeanTween.cancelAll();
    }

    public void OpenNFTSelectionScreen()
    {
        passwordScreen.SetActive(false);
        connectingToServerScreen.SetActive(false);
        connectingToRoom.SetActive(false);

        foreach (GameObject screen in gameMenuScreens)
        {
            screen.SetActive(false);
        }

        foreach (GameObject screen in nftSelectionScreens)
        {
            screen.SetActive(true);
        }
        GameState.SetSelectedNFT(null);
    }

    public void OpenEquipmentScreen()
    {
        foreach (GameObject screen in gameMenuScreens)
        {
            screen.SetActive(false);
        }

        foreach (GameObject screen in equipmentScreens)
        {
            screen.SetActive(true);
        }
    }

    private void OpenLoadingScreen()
    {
        foreach (GameObject screen in nftSelectionScreens)
        {
            screen.SetActive(false);
        }
        loadingScreen.SetActive(true);
    }

    public void OpenGameMenu()
    {
        loadingScreen.SetActive(false);
        connectingToServerScreen.SetActive(false);
        passwordScreen.SetActive(false);

        foreach (GameObject screen in nftSelectionScreens)
        {
            screen.SetActive(false);
        }

        foreach (GameObject screen in equipmentScreens)
        {
            screen.SetActive(false);
        }

        foreach (GameObject screen in gameMenuScreens)
        {
            screen.SetActive(true);
        }
    }

    private void CloseGameMenu()
    {
        foreach (GameObject screen in gameMenuScreens)
        {
            screen.SetActive(false);
        }
    }

    public void TryConnectToRoom()
    {
        if (!DataManager.Instance.PlayerData.CanFight)
        {
            RecoveryMessageDisplay.Instance.ShowMessage();
            OpenNFTSelectionScreen();
            return;
        }
        
        CloseGameMenu();
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
        CloseGameMenu();
        connectingToRoom.SetActive(true);

        connectingToRoomText.text = "Connecting to Multiplayer Server(" + PhotonNetwork.CloudRegion + ")...";
    }

    public void OpenSettings()
    {
        CloseGameMenu();
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        OpenGameMenu();
        settings.SetActive(false);
    }
}
