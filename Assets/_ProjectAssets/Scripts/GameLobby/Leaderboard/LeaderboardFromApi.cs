using System;
using System.Collections.Generic;
using Anura.ConfigurationModule.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LeaderboardFromApi : MonoBehaviour
{
    [HideInInspector] private static LeaderboardData leaderboard;

    public async UniTask GetLeaderboard(Action<LeaderboardData> _callBack)
    {
        if (leaderboard != null && leaderboard.Entries.Count > 0)
        {
            _callBack?.Invoke(leaderboard);
            return;
        }

        Debug.Log("[HTTP] Grabbing leaderboard...");
        string _resp = await NetworkManager.GETRequestCoroutine("/leaderboard",
            (_code, _err) => { Debug.LogWarning($"Couldn't retrieve Leaderboard!\n{_code}\n{_err}"); });

        if (string.IsNullOrEmpty(_resp)) return;


        if (ConfigurationManager.Instance.GameConfig.enableDevLogs)
        {
            Debug.Log(_resp);
        }

        var _mockLeaderboardData = JsonUtility.FromJson<LeaderboardGetResponseEntity>(_resp);
        leaderboard = new LeaderboardData();
        foreach (var _entry in _mockLeaderboardData.leaderboard)
        {
            leaderboard.Entries.Add(new LeaderboardEntries
            {
                PrincipalId = _entry.principalId,
                Nickname = _entry.nickname,
                KittyUrl = string.Empty,
                Points = _entry.points,
                Level = 0
            });
        }
        leaderboard.FinishSetup(3);
        leaderboard.TopPlayers = new List<string>() { _mockLeaderboardData.first, _mockLeaderboardData.second, _mockLeaderboardData.third };


        Debug.Log($"[HTTP] Grabbed {leaderboard.Entries.Count} players...");
        _callBack?.Invoke(leaderboard);
    }
}

[System.Serializable]
public class LeaderboardGetResponseEntity
{
    [SerializeField]
    public List<PlayerStatsEntity> leaderboard;
    [SerializeField]
    public string first;
    [SerializeField]
    public string second;
    [SerializeField]
    public string third;
}

[System.Serializable]
public class PlayerStatsEntity
{
    [SerializeField]
    public string principalId;
    [SerializeField]
    public string nickname;
    [SerializeField]
    public int points;
}
