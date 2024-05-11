using System;
using System.Collections.Generic;

[Serializable]
public class GuildData
{
    public string Id;
    public string Owner;
    public string Name;
    public string KingdomName;
    public string BadgeName;
    public int PointsRequirement;
    public int MatchesWon;
    public List<GuildPlayerData> Players = new ();
    public bool IsFull => Players.Count >= DataManager.Instance.GameData.MaxGuildPlayers;
    public int BattlesWon;
    
    public int SumOfPoints
    {
        get
        {
            int _sumOfPoints = 0;
            foreach (var _player in Players)
            {
                _sumOfPoints += _player.Points;
            }

            return _sumOfPoints;
        }
    }
}
