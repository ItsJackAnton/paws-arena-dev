using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using System;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const string NAME = "Name";
    public const string SEAT = "Seat";
    public const string ALLOW_SPECTATORS = "AllowSpectators";
    public const string ROOM_WINNER = "RoomWinner";
    public const string ROOM_WINNER_IDS = "RoomWinnerIds";
    
    public event Action OnStartedConnection;
    public event Action OnConnectedServer;
    public event Action OnCreatingRoom;
    public event Action OnRoomLeft;
    public static Action OnFailedToCreateRoom;
    public static Action OnJoinedFriendlyRoom;

    [SerializeField]
    private byte maxPlayersPerRoom = 2;
    private byte maxPlayersPerSpectatorRoom = 4;
    private string gameVersion = "2";

    private bool isRoomCreated = false;
    private bool isSinglePlayer = false;
    private string friendlyRoomName;
    public static bool IsFriendly;


    #region ACTIONS
    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        OnStartedConnection?.Invoke();
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
        else
        {
            OnConnectedServer?.Invoke();
        }
    }

    public void ConnectToRandomRoom()
    {
        isSinglePlayer = false;
        PhotonNetwork.JoinRandomRoom();
    }

    public void TryLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinFriendlyRoom(string _roomName)
    {
        friendlyRoomName = _roomName;
        PhotonNetwork.JoinRoom(friendlyRoomName);
    }
    
    #endregion
    #region CALLBACKS

    public override void OnConnectedToMaster()
    {
        SetCustomProperties();
        OnConnectedServer?.Invoke();
    }

    private void SetCustomProperties()
    {
        Debug.Log("----- setting custom room properties");
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable
            {
                { "principalId", GameState.principalId },
                {NAME, GameState.nickname},
                {SEAT, 0}
            });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.Instance.LoadMainMenu();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        isRoomCreated = true;

        string roomName = Guid.NewGuid().ToString();
        PhotonNetwork.CreateRoom(roomName, new RoomOptions{ MaxPlayers = maxPlayersPerRoom,  });
        GameState.roomName = roomName;
        OnCreatingRoom?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        isRoomCreated = true;

        PhotonNetwork.CreateRoom(friendlyRoomName, 
            new RoomOptions
            {
                IsVisible = false,
                MaxPlayers = CreateFriendlyMatch.AllowSpectators ? maxPlayersPerSpectatorRoom : maxPlayersPerRoom,
                CustomRoomProperties = new Hashtable()
                {
                    [ALLOW_SPECTATORS] = CreateFriendlyMatch.AllowSpectators ? 1: 0,
                }
            });
        GameState.roomName = friendlyRoomName;
        OnCreatingRoom?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnFailedToCreateRoom?.Invoke();
    }

    public void CreateSinglePlayerRoom()
    {
        isRoomCreated = true;
        isSinglePlayer = true;
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1 });
        OnCreatingRoom?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        if (isRoomCreated)
        {
            if (!isSinglePlayer)
            {
                PhotonNetwork.LoadLevel(CreateFriendlyMatch.AllowSpectators ? SceneManager.GAME_ROOM_SPECTATOR : SceneManager.GAME_ROOM);
            }
            else
            {
                PhotonNetwork.LoadLevel(SceneManager.SINGLE_PLAYER);
            }
        }

        if (PhotonNetwork.CurrentRoom is { IsVisible: false })
        {
            IsFriendly = true;
            OnJoinedFriendlyRoom?.Invoke();
        }
        else
        {
            IsFriendly = false;
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable());
        OnRoomLeft?.Invoke();
    }
    #endregion
    
    public static void SetRoomProperties(string _key, string _value)
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        Hashtable _propertiesToSet = new Hashtable { [_key] = _value };
        PhotonNetwork.CurrentRoom.SetCustomProperties(_propertiesToSet);
    }
    
}
