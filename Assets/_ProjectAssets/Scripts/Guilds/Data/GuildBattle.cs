using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GuildBattle
{
    public string Guild1Id;
    public string Guild2Id;
    public List<GuildBattleEntry> BattleEntries = new ();

    public GuildData Opponent
    {
        get
        {
            string _opponentId = DataManager.Instance.PlayerData.GuildId == Guild1Id ? Guild2Id : Guild1Id;
            return DataManager.Instance.GameData.Guilds.Find(_guild => _guild.Id == _opponentId);
        }
    }

    public int Points(bool _forMyGuild)
    {
        int _points = 0;
        GuildData _guild = _forMyGuild ? DataManager.Instance.PlayerData.Guild : Opponent;

        foreach (var _entry in BattleEntries)
        {
            if (_guild.Players.All(_guildPlayer => _guildPlayer.Principal != _entry.Principal))
            {
                continue;
            }
            
            _points += _entry.Points;
        }
            
        return _points;
    }
}
