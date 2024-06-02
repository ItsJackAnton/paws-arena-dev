using System;
using System.Collections.Generic;
using BoomDaoWrapper;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class GuildData
{
    public string Id;
    public string Owner;
    public string Name;
    public string KingdomName;
    public string BadgeName;
    public int PointsRequirement;
    public List<int> BattlesHistory = new ();
    public List<GuildPlayerData> Players = new ();
    public bool IsFull => Players.Count >= DataManager.Instance.GameData.MaxGuildPlayers;
    public GuildBattle GuildBattle = new ();
    public int BattlesWon;

    [JsonIgnore] public Sprite Badge => AssetsManager.Instance.GetChallengeBadgeSprite(BadgeName);
    [JsonIgnore] public Sprite Kingdom => AssetsManager.Instance.GetChallengeKingdomSprite(KingdomName);
    
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

    public bool IsAdmin => Owner == BoomDaoUtility.Instance.UserPrincipal;
}
