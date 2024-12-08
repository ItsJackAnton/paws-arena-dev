using Anura.ConfigurationModule.Managers;
using Anura.Templates.MonoSingleton;
using com.colorfulcoding.GameScene;
using Photon.Pun;
using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class RoomStateManager : MonoSingleton<RoomStateManager>
{
    public static event Action<IRoomState> OnStateUpdated;

    public PUNGameRoomManager photonManager;
    public HttpNetworkCommunication httpCommunication;

    [Header("Player")]
    public GameObject playerPrefab;
    public GameObject playerUIPrefab;

    [Header("Bots")]
    public GameObject botPlayerPrefab;

    public Transform playerUIParent;

    [Header("Others")]
    public TrajectoryBehaviour trajectory;

    [HideInInspector]
    public GameSceneMasterInfo sceneInfo = new GameSceneMasterInfo();

    [HideInInspector]
    public PhotonView photonView;

    [HideInInspector]
    public int lastPlayerRound = 0;

    [HideInInspector]
    public int roundNumber = 1;

    [HideInInspector]
    public IRoomState currentState;

    [HideInInspector]
    public bool isMultiplayer;

    public TurnTimerBehaviour Timer { get; set; }

    private void Start()
    {
        lastPlayerRound = 0;
        photonView = GetComponent<PhotonView>();
        Init();
    }

    private void OnEnable()
    {
        isMultiplayer = ConfigurationManager.Instance.Config.GetIsMultiplayer();

        PUNRoomUtils.onPlayerLeft += OnPlayerLeft;
        PlayerManager.Instance.onHealthUpdated += CheckDeath;
    }

    private void OnDisable()
    {
        PUNRoomUtils.onPlayerLeft -= OnPlayerLeft;
        PlayerManager.Instance.onHealthUpdated -= CheckDeath;
    }

    private void OnDestroy()
    {
        LeanTween.cancelAll();
    }

    private void Init()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        MapsManager.Instance.CreateMap();
        if (isMultiplayer)
        {
            SetState(new WaitingForAllPlayersToJoinState());
            photonView.RPC("OnPlayerSceneLoaded", RpcTarget.All);
        }
        else
        {
            OnPlayerSceneLoaded_SinglePlayer();
        }
    }

    public void SetState(IRoomState state)
    {
        if (currentState is ResolvingGameState)
        {
            return;
        }

        if (currentState != null)
        {
            currentState.OnExit();
        }

        if (ConfigurationManager.Instance.GameConfig.enableDevLogs)
        {
            Debug.Log("Setting state " + state);
        }

        currentState = state;
        currentState.Init(this);

        OnStateUpdated?.Invoke(state);
    }

    public virtual void SetFirstPlayerTurn()
    {
        roundNumber++;
        if (LuckyWheelWhoPlaysFirst.DoIPlayFirst)
        {
            SetState(new MyTurnState());
        }
        else
        {
            SetState(new OtherPlayerTurnState());
        }

        if (PhotonNetwork.CurrentRoom==null || PhotonNetwork.CurrentRoom.PlayerCount==1&&!LuckyWheelWhoPlaysFirst.DoIPlayFirst)
        {
            SetState(new BotTurnState());
        }
    }

    public virtual bool WasMyRound()
    {
        if (!isMultiplayer)
            return true;

        return (lastPlayerRound == 0 && PhotonNetwork.LocalPlayer.IsMasterClient)
            || (lastPlayerRound == 1 && !PhotonNetwork.LocalPlayer.IsMasterClient);
    }

    public void OnPlayerLeft()
    {
        if (currentState is ResolvingGameState)
        {
            return;
        }
        SetState(
            new ResolvingGameState(
                PhotonNetwork.LocalPlayer.IsMasterClient
                    ? GameResolveState.PLAYER_1_WIN
                    : GameResolveState.PLAYER_2_WIN
            )
        );
    }

    public void TryStartNextRound()
    {
        if (!isMultiplayer || IsMasterClient())
        {
            GameResolveState resolveState = PlayerManager.Instance.GetWinnerByDeath();
            if (resolveState != GameResolveState.NO_WIN)
            {
                SingleAndMultiplayerUtils.RpcOrLocal(
                    this,
                    photonView,
                    false,
                    "StartResolveGame",
                    RpcTarget.All,
                    resolveState
                );
            }
            else
            {
                int nextRound = roundNumber;
                if (lastPlayerRound == (sceneInfo.usersInScene - 1))
                {
                    nextRound += 1;
                }
                if (nextRound > ConfigurationManager.Instance.Config.GetMaxNumberOfRounds())
                {
                    resolveState = PlayerManager.Instance.GetWinnerByHealth();

                    SingleAndMultiplayerUtils.RpcOrLocal(
                        this,
                        photonView,
                        false,
                        "StartResolveGame",
                        RpcTarget.All,
                        resolveState
                    );
                    return;
                }

                if (sceneInfo.usersInScene == 1)
                {
                    SingleAndMultiplayerUtils.RpcOrLocal(
                        this,
                        photonView,
                        false,
                        "StartNextRound",
                        RpcTarget.All,
                        CreateFriendlyMatch.AllowSpectators ? 3 : 0,
                        nextRound
                    );
                }
                else
                {
                    SingleAndMultiplayerUtils.RpcOrLocal(
                        this,
                        photonView,
                        false,
                        "StartNextRound",
                        RpcTarget.All,
                        GetNextPlayer,
                        nextRound
                    );
                }
            }
        }
    }
    
    private int GetNextPlayer
    {
        get
        {
            Debug.Log("Last round player: "+lastPlayerRound);
            
            if (CreateFriendlyMatch.AllowSpectators)
            {
                if (lastPlayerRound == 3)
                {
                    return 4;
                }

                return 3;
            }

            return (lastPlayerRound + 1) % 2;
        }
    }

    public void CheckDeath(int newHp)
    {
        GameResolveState state = PlayerManager.Instance.GetWinnerByDeath();
        if (state != GameResolveState.NO_WIN)
        {
            SingleAndMultiplayerUtils.RpcOrLocal(
                this,
                photonView,
                false,
                "StartResolveGame",
                RpcTarget.All,
                state
            );
        }
    }

    public void SetProjectileLaunchedState(float waitBeforeEndTurn)
    {
        SingleAndMultiplayerUtils.RpcOrLocal(
            this,
            photonView,
            false,
            "StartProjectileLaunchedState",
            RpcTarget.All,
            waitBeforeEndTurn
        );
    }

    public virtual void SendRetreatRPC()
    {
        int isMaster = !isMultiplayer ? 0 : (PhotonNetwork.LocalPlayer.IsMasterClient ? 0 : 1);
        SingleAndMultiplayerUtils.RpcOrLocal(
            this,
            photonView,
            false,
            "Retreat",
            RpcTarget.MasterClient,
            isMaster
        );
    }

    public void SinglePlayerReturnMainMenu()
    {
        SceneManager.Instance.LoadMainMenu();
    }

    public void LoadAfterGameScene(GameResolveState state)
    {
        if (IsMasterClient())
        {
            if (CreateFriendlyMatch.AllowSpectators)
            {
                StartCoroutine(EndSpectatorGame());
            }
            else
            {
                SceneManager.Instance.LoadAfterGame();
            }
        }
        else if (!isMultiplayer)
        {
            SinglePlayerReturnMainMenu();
        }
    }

    private IEnumerator EndSpectatorGame()
    {
        photonView.RPC(nameof(SetWinnerKitty), RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC(nameof(LetMasterLoadEndScene), RpcTarget.All);
    }

    [PunRPC]
    protected void SetWinnerKitty()
    {
        Debug.Log("--------- Trying to set winning kitty");
        int _seat = PUNGameRoomManager.Instance.GetMySeat();
        if (_seat<=2)
        {
            Debug.Log("--------- My seat is lower than two");
            return;
        }

        if (GameState.gameResolveState == GameResolveState.PLAYER_2_WIN && _seat == 3)
        {
            Debug.Log("--------- I didn't win");
            return;
        }

        if (GameState.gameResolveState == GameResolveState.PLAYER_1_WIN && _seat == 4)
        {
            Debug.Log("--------- I didn't win 2");
            return;
        }
        
        Debug.Log("--------- I am setting the winner information");
        Debug.Log($"--------- {PhotonManager.ROOM_WINNER}: {GameState.selectedNFT.imageUrl}");
        Debug.Log($"--------- {PhotonManager.ROOM_WINNER_IDS}: {JsonConvert.SerializeObject(GameState.selectedNFT.ids)}");
        PhotonManager.SetRoomProperties(PhotonManager.ROOM_WINNER, GameState.selectedNFT.imageUrl);
        PhotonManager.SetRoomProperties(PhotonManager.ROOM_WINNER_IDS, JsonConvert.SerializeObject(GameState.selectedNFT.ids));
    }

    [PunRPC]
    protected void LetMasterLoadEndScene()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        SceneManager.Instance.LoadAfterGameSpectator();
    }

    [PunRPC]
    public void StartProjectileLaunchedState(float waitBeforeEndTurn)
    {
        if (
            ConfigurationManager.Instance.Config.GetGameType()
                == Anura.ConfigurationModule.ScriptableObjects.GameType.TUTORIAL
            && !TutorialManager.Instance.finished
        )
        {
            SetState(new GamePausedState());
        }
        else
        {
            SetState(new ProjectileLaunchedState(waitBeforeEndTurn));
        }
    }

    [PunRPC]
    public void StartNextRound(int playerNumber, int roundNumber)
    {
        this.roundNumber = roundNumber;
        if (
            ConfigurationManager.Instance.Config.GetGameType()
            == Anura.ConfigurationModule.ScriptableObjects.GameType.TUTORIAL
        )
        {
            SetState(new MyTurnState());
            return;
        }

        if (!isMultiplayer)
        {
            if (playerNumber == 0)
            {
                SetState(new MyTurnState());
            }
            else
            {
                SetState(new BotTurnState());
            }
            return;
        }

        HandleNextRoundMultiplayer(playerNumber);
    }

    protected virtual void HandleNextRoundMultiplayer(int _playerNumber)
    {
        if (_playerNumber == 0)
        {
            SetState(
                PhotonNetwork.LocalPlayer.IsMasterClient
                    ? new MyTurnState()
                    : new OtherPlayerTurnState()
            );
        }
        else
        {
            SetState(
                PhotonNetwork.LocalPlayer.IsMasterClient
                    ? new OtherPlayerTurnState()
                    : new MyTurnState()
            );
        }
    }

    [PunRPC]
    public void OnPlayerSceneLoaded()
    {
        if (IsMasterClient())
        {
            sceneInfo.usersInScene++;

            Debug.Log(
                $"Players in scene: {sceneInfo.usersInScene} / {PhotonNetwork.CurrentRoom.PlayerCount}"
            );
            if (sceneInfo.usersInScene == ExpectedAmountOfPlayers())
            {
                Debug.Log("All players joined scene");
                photonView.RPC("OnAllPlayersJoinedScene", RpcTarget.All);
            }
        }
    }

    protected virtual int ExpectedAmountOfPlayers()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void OnPlayerSceneLoaded_SinglePlayer()
    {
        if (
            ConfigurationManager.Instance.Config.GetGameType()
            == Anura.ConfigurationModule.ScriptableObjects.GameType.TUTORIAL
        )
        {
            sceneInfo.usersInScene = 1;
        }
        else
        {
            //Me + bot;
            sceneInfo.usersInScene = 2;
        }
        OnAllPlayersJoinedScene();
    }

    [PunRPC]
    public void OnAllPlayersJoinedScene()
    {
        SetState(new StartingGameState());
    }

    [PunRPC]
    public void StartResolveGame(GameResolveState state)
    {
        if (currentState is ResolvingGameState)
        {
            return;
        }
        SetState(new ResolvingGameState(state));
    }

    [PunRPC]
    public void Retreat(int playerIdx)
    {
        GameResolveState resolveState = PlayerManager.Instance.GetWinnerByLoserIndex(playerIdx);
        SingleAndMultiplayerUtils.RpcOrLocal(
            this,
            photonView,
            false,
            "StartResolveGame",
            RpcTarget.All,
            resolveState
        );
    }

    protected virtual bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }
}
