using Anura.ConfigurationModule.Managers;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using BoomDaoWrapper;
using com.colorfulcoding.AfterGame;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.colorfulcoding.GameScene
{
    public class HttpNetworkCommunication : MonoBehaviour
    {
        private void Start()
        {
            RegisterStartOfTheMatch();
        }

        private async void RegisterStartOfTheMatch()
        {
            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    GameState.roomName = PhotonNetwork.CurrentRoom.Name;
                }
            }
            catch (Exception) { }

            LeaderboardPostRequestEntity req = new LeaderboardPostRequestEntity()
            {
                matchId = GameState.roomName,
                kittyUrl = GameState.selectedNFT.imageUrl,
                status = GetMatchStartStatus()
            };

            string reqJson = JsonUtility.ToJson(req);

            try
            {
                await NetworkManager.POSTRequest(
                    "/leaderboard/match",
                    reqJson,
                    (resp) =>
                    {
                    },
                    (err, code) =>
                    {
                        Debug.LogWarning($"[HTTP]Error registering the match {code}: {err}");
                    },
                    true
                );
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogWarning($"[HTTP]Error registering the match {ex.ResponseCode}: {ex.Text}");
                RoomStateManager.Instance.OnPlayerLeft();
            }
        }

        public async UniTask RegisterEndOfTheMatch(int hp, GameResolveState state)
        {
            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    GameState.roomName = PhotonNetwork.CurrentRoom.Name;
                }
            }
            catch (Exception) { }

            LeaderboardPostRequestEntity req = new LeaderboardPostRequestEntity()
            {
                matchId = GameState.roomName,
                kittyUrl = GameState.selectedNFT.imageUrl,
                status = GetMatchEndStatus(),
                hp = hp,
                winner = (
                    state == GameResolveState.DRAW
                        ? GameResult.Draw
                        : (
                            state == GameResolveState.PLAYER_1_WIN
                                ? GameResult.Player1
                                : GameResult.Player2
                        )
                ),
                isFriendly =  PhotonManager.IsFriendly
            };

            string reqJson = JsonUtility.ToJson(req);

            await NetworkManager.POSTRequest(
                "/leaderboard/match",
                reqJson,
                (resp) =>
                {
                    Debug.Log(resp);
                    LeaderboardPostResponseEntity response = JsonUtility.FromJson<LeaderboardPostResponseEntity>(resp);
                    Debug.Log(1);
                    Debug.Log(DataManager.Instance);
                    Debug.Log(DataManager.Instance.PlayerData);
                    Debug.Log(DataManager.Instance.PlayerData.LeaderboardPoints);
                    response.oldPoints = DataManager.Instance.PlayerData.LeaderboardPoints;
                    Debug.Log(2);
                    GameState.pointsChange = response;
                    Debug.Log(3);
                },
                (err, code) =>
                {
                    Debug.LogWarning($"[HTTP]Error registering the match ending; {code}: {err}");
                },
                true
            );
        }

        private MatchStatus GetMatchStartStatus()
        {
            if (
                ConfigurationManager.Instance.Config.GetGameType()
                == Anura.ConfigurationModule.ScriptableObjects.GameType.SINGLEPLAYER
            )
            {
                return MatchStatus.MatchStartedForBothPlayers;
            }

            if (PhotonNetwork.CurrentRoom.masterClientId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return MatchStatus.MatchStartedForPlayer1;
            }

            return MatchStatus.MatchStartedForPlayer2;
        }

        private MatchStatus GetMatchEndStatus()
        {
            if (
                ConfigurationManager.Instance.Config.GetGameType()
                == Anura.ConfigurationModule.ScriptableObjects.GameType.SINGLEPLAYER
            )
            {
                return MatchStatus.MatchFinishedForBothPlayers;
            }

            if (PhotonNetwork.CurrentRoom.masterClientId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return MatchStatus.MatchFinishedForPlayer1;
            }

            return MatchStatus.MatchFinishedForPlayer2;
        }
    }
}
