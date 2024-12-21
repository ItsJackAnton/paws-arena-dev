using System.Collections.Generic;
using BoomDaoWrapper;
using UnityEngine;

public class TryToResetLeaderboardScore : MonoBehaviour
{
    private void Start()
    {
        if (DataManager.Instance.GameData.LeaderboardSeason != DataManager.Instance.PlayerData.CurrentLeaderboardSeason)
        {
            List<ActionParameter> _parameters = new()
            {
                new ActionParameter { Key = GameData.LEADERBOARD_SEASON, Value = DataManager.Instance.GameData.LeaderboardSeason.ToString()}
            };
            
            BoomDaoUtility.Instance.ExecuteActionWithParameter("resetMyLeaderboardPoints", _parameters,null);
        }
    }
}
