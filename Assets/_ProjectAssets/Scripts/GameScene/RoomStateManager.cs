using Anura.Templates.MonoSingleton;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStateManager : MonoSingleton<RoomStateManager>
{
    public static event Action<IRoomState> OnStateUpdated;

    public PUNGameRoomManager photonManager;
    public GameObject playerPrefab;

    [HideInInspector]
    public GameSceneMasterInfo sceneInfo = new GameSceneMasterInfo();

    [HideInInspector]
    public int lastPlayerRound = -1;
    [HideInInspector]
    public PhotonView photonView;

    private IRoomState currentState;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Init();
    }

    private void Init()
    {
        SetState(new WaitingForAllPlayersToJoinState()); ;
        photonView.RPC("OnPlayerSceneLoaded", RpcTarget.All);
    }

    public void SetState(IRoomState state)
    {
        if(currentState != null)
        {
            currentState.OnExit();
        }

        currentState = state;
        currentState.Init(this);

        OnStateUpdated?.Invoke(state);
    }


    public void SetFirstPlayerTurn()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            SetState(new MyTurnState());
        }
        else
        {
            SetState(new OtherPlayersTurnState());
        }
    }


    [PunRPC]
    private void StartNextRound()
    {
        if (lastPlayerRound == 0)
        {
            SetState(PhotonNetwork.LocalPlayer.IsMasterClient ? new OtherPlayersTurnState() : new MyTurnState());
        }
        else
        {
            SetState(PhotonNetwork.LocalPlayer.IsMasterClient ? new MyTurnState() : new OtherPlayersTurnState());
        }
    }

    [PunRPC]
    private void OnPlayerSceneLoaded()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            sceneInfo.usersInScene++;

            Debug.Log($"Players in scene: {sceneInfo.usersInScene} / {PhotonNetwork.CurrentRoom.PlayerCount}");
            if (sceneInfo.usersInScene == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                photonView.RPC("OnAllPlayersJoinedScene", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void OnAllPlayersJoinedScene()
    {
        SetState(new StartingGameState());
    }
}
