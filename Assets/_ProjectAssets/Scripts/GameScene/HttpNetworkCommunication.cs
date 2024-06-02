using Anura.ConfigurationModule.Managers;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using BoomDaoWrapper;
using com.colorfulcoding.AfterGame;
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

            if (ConfigurationManager.Instance.GameConfig.enableDevLogs)
            {
                Debug.Log(reqJson);
            }

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

            Debug.Log("Registering match end");
            Debug.Log(reqJson);
            
            Debug.Log("Registering match ended: "+reqJson+ " my principal: "+GameState.principalId);

            await NetworkManager.POSTRequest(
                "/leaderboard/match",
                reqJson,
                (resp) =>
                {
                    Debug.Log("----- Got result of the match: "+resp);
                    LeaderboardPostResponseEntity response = JsonUtility.FromJson<LeaderboardPostResponseEntity>(resp);
                    Debug.Log(resp);
                    Debug.Log(
                        $"[HTTP]Match ending registered! You won {response.oldPoints + response.points} points."
                    );

                    if (ConfigurationManager.Instance.GameConfig.enableDevLogs)
                    {
                        Debug.Log(resp);
                    }

                    if (DataManager.Instance.PlayerData.LeaderboardPoints==0)
                    {
                        List<ActionParameter> _parameters = new List<ActionParameter>()
                        {
                            new() { Key = "IncreaseAmount", Value = response.oldPoints.ToString() }
                        };
                        
                        BoomDaoUtility.Instance.ExecuteActionWithParameter(AfterGameMainTitle.INCREASE_LEADERBOARD_POINTS,_parameters,null);
                    }
                    else
                    {
                        response.oldPoints = DataManager.Instance.PlayerData.LeaderboardPoints;
                    }

                    int _oldPoints = response.oldPoints;
                    int _points = response.points;
                    Debug.Log("------------ "+response.gameResultType);
                    response = new LeaderboardPostResponseEntity
                    {
                        points = _points, oldPoints = _oldPoints, gameResultType = (int)state, reason = string.Empty
                    };
                    
                    GameState.pointsChange = response;
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
