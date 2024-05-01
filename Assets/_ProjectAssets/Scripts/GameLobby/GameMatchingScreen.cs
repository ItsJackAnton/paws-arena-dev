using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[System.Serializable]
public class SeatGameobject
{
    [SerializeField]
    public TextMeshProUGUI occupierNickname;
}

public class GameMatchingScreen : MonoBehaviour
{
    [Header("Managers")]
    public PUNRoomUtils punRoomUtils;
    public SyncPlatformsBehaviour syncPlatformsBehaviour;

    [Header("Internals")]
    public GameObject notices;
    public List<SeatGameobject> seats;
    public Countdown countdown;

    [SerializeField] private GameObject wheelHolder;
    [SerializeField] private GameObject searchingForOpponent;

    private void OnEnable()
    {
        Init();
        PUNRoomUtils.onPlayerJoined += OnPlayerJoined;
        PUNRoomUtils.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        PUNRoomUtils.onPlayerJoined -= OnPlayerJoined;
        PUNRoomUtils.onPlayerLeft -= OnPlayerLeft;
    }

    private void Init()
    {
        notices.SetActive(false);

        SetSeats();

        //Setting up Room
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            int mapIdx = UnityEngine.Random.Range(0, 7);
            punRoomUtils.AddRoomCustomProperty("mapIdx", mapIdx);

            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                StartGame();
            }
            searchingForOpponent.SetActive(true);
        }
        else
        {
            List<Player> players = punRoomUtils.GetOtherPlayers();
            if (players.Count == 1)
            {
                notices.SetActive(true);
            }
            else
            {
                Debug.LogWarning(
                    $"PUN: Inconsistency! There are {players.Count} players in room??"
                );
            }
        }

        if (PhotonNetwork.CurrentRoom is {IsVisible: true})
        {
            StartCoroutine(BringBotAfterSeconds(15));
        }
    }

    public void SetSeats()
    {
        foreach (SeatGameobject seat in seats)
        {
            FreeSeat(seat);
        }

        bool isMyPlayerMaster = PhotonNetwork.LocalPlayer.IsMasterClient;
        OccupySeat(seats[isMyPlayerMaster ? 0 : 1], PhotonNetwork.LocalPlayer.NickName);

        List<Player> players = punRoomUtils.GetOtherPlayers();
        if (players.Count == 1)
        {
            OccupySeat(seats[isMyPlayerMaster ? 1 : 0], players[0].NickName);
        }
    }

    private IEnumerator BringBotAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds-3);
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        yield return new WaitForSeconds(3);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            BringBot();
        }
        searchingForOpponent.SetActive(false);
    }

    [ContextMenu("Bring Bot")]
    public void BringBot()
    {
        BotInformation botInformation = GetRandomBot();

        GameState.botInfo = botInformation;
        PhotonNetwork.CurrentRoom.MaxPlayers = 1;
        OccupySeat(seats[1], botInformation.nickname);
        syncPlatformsBehaviour.InstantiateBot();
        StartSinglePlayerGame();
    }

    private BotInformation GetRandomBot()
    {
        var _bots = new List<BotInformation>
        {
            new()
            {
                nickname = "Jack Sparrow",
                l = 4,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=izapu-dakor-uwiaa-aaaaa-cqace-eaqca-aabmc-a",
                SeasonProgressMultiplayer = 1.2f
            },
            new ()
            {
                nickname = "Cat Fairy",
                l = 4,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=pjbr5-xikor-uwiaa-aaaaa-cqace-eaqca-aabmq-a",
                SeasonProgressMultiplayer = 1.25f
            },
            new ()
            {
                nickname = "qrqhn",
                l = 5,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=oeeh5-yikor-uwiaa-aaaaa-cqace-eaqca-aaar7-a",
                SeasonProgressMultiplayer = 1.4f
            },
            new ()
            {
                nickname = "lazy_hunter",
                l = 1,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=kgds5-oykor-uwiaa-aaaaa-cqace-eaqca-aabyk-q",
                SeasonProgressMultiplayer = 0.5f
            },
            new ()
            {
                nickname = "Mr. Robot",
                l = 5,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=te62f-xykor-uwiaa-aaaaa-cqace-eaqca-aadpg-a",
                SeasonProgressMultiplayer = 2
            },
            new ()
            {
                nickname = "filipo",
                l = 5,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=qvghj-likor-uwiaa-aaaaa-cqace-eaqca-aabp4-q",
                SeasonProgressMultiplayer = 1.8f
            },
            new ()
            {
                nickname = "Callie",
                l = 3,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=ben3m-hykor-uwiaa-aaaaa-cqace-eaqca-aadb4-q",
                SeasonProgressMultiplayer = 1f
            },
            new ()
            {
                nickname = "Strawberry",
                l = 3,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=yil5q-5ykor-uwiaa-aaaaa-cqace-eaqca-aabur-a",
                SeasonProgressMultiplayer = 1.2f
            },
            new ()
            {
                nickname = "Strawberry",
                l = 1,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=dnmix-6qkor-uwiaa-aaaaa-cqace-eaqca-aabzy-q",
                SeasonProgressMultiplayer = 0.6f
            },
            new ()
            {
                nickname = "xLilMonster",
                l = 1,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=vrun7-takor-uwiaa-aaaaa-cqace-eaqca-aach5-a",
                SeasonProgressMultiplayer = 0.7f
            },
            new ()
            {
                nickname = "airstrike22",
                l = 1,
                kittyUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.ic0.app/?type=thumbnail&tokenid=c6bu2-jqkor-uwiaa-aaaaa-cqace-eaqca-aadni-q",
                SeasonProgressMultiplayer = 0.8f
            }
        };

        int _index = Random.Range(0, _bots.Count);
        if (DataManager.Instance.PlayerData.ShouldBotBeEasy)
        {
            int _minLevel = int.MaxValue;
            foreach (var _bot in _bots)
            {
                if (_bot.l<_minLevel)
                {
                    _minLevel = _bot.l;
                }
            }

            var _botsWithMinLevel = _bots.FindAll(_bot => _bot.l == _minLevel).ToList();
            return _botsWithMinLevel[Random.Range(0,_botsWithMinLevel.Count)];
        }
        return _bots[_index];
    }

    private void OccupySeat(SeatGameobject seat, string nickName)
    {
        seat.occupierNickname.text = nickName;
    }

    private void FreeSeat(SeatGameobject seat)
    {
        seat.occupierNickname.text = "-";
    }

    private void OnPlayerJoined(string opponentNickname, string userId)
    {
        int mySeat = Int32.Parse(PhotonNetwork.LocalPlayer.CustomProperties["seat"].ToString());
        int otherSeat = (mySeat + 1) % 2;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            CheckPlayersAreDifferent();
            searchingForOpponent.SetActive(false);
            notices.SetActive(true);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    StartGame();
                }
            }
        }
        OccupySeat(seats[otherSeat], opponentNickname);
    }

    private void CheckPlayersAreDifferent()
    {
        var keysList = new List<int>(PhotonNetwork.CurrentRoom.Players.Keys);

        Debug.Log(
            $"Comparing {PhotonNetwork.CurrentRoom.Players[keysList[0]].CustomProperties["principalId"]} with {PhotonNetwork.CurrentRoom.Players[keysList[1]].CustomProperties["principalId"]}"
        );

        if (
            ((string)PhotonNetwork.CurrentRoom.Players[keysList[0]].CustomProperties["principalId"])
            == (
                (string)
                    PhotonNetwork.CurrentRoom.Players[keysList[1]].CustomProperties["principalId"]
            )
        )
        {
            Debug.LogWarning("Same player connected twice!");
            StartCoroutine(TryExitRoomAfterSeconds(1));
        }
    }

    private void OnPlayerLeft()
    {
        int mySeat = Int32.Parse(PhotonNetwork.LocalPlayer.CustomProperties["seat"].ToString());
        int otherSeat = (mySeat + 1) % 2;
        notices.SetActive(false);
        FreeSeat(seats[otherSeat]);
        MakeRoomVisible();
        searchingForOpponent.SetActive(true);
    }

    private IEnumerator TryExitRoomAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        TryExitRoom();
    }

    public void TryExitRoom()
    {
        punRoomUtils.TryLeaveRoom();
    }

    public void StartGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        GetComponent<PhotonView>().RPC(nameof(StartGameRoutine), RpcTarget.All);
    }

    public void StartSinglePlayerGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = false;
        GetComponent<PhotonView>().RPC(nameof(StartSinglePlayerGameRoutine), RpcTarget.All);
    }

    private void MakeRoomVisible()
    {
        PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    [PunRPC]
    public void StartGameRoutine()
    {
        searchingForOpponent.SetActive(false);
        StartCountdown(SceneManager.GAME_SCENE);
    }
    
    [PunRPC]
    public void StartSinglePlayerGameRoutine()
    {
        StartCountdown(SceneManager.SINGLE_PLAYER_GAME);
    }

    private void StartCountdown(string _sceneName)
    {
        searchingForOpponent.SetActive(false);
        wheelHolder.SetActive(true);
        countdown.StartCountDown(() =>
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.IsMessageQueueRunning = false;
                PhotonNetwork.LoadLevel(_sceneName);
            }

        });
    }
}