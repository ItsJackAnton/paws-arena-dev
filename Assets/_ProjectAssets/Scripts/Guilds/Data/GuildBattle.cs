using System;
using System.Collections.Generic;

[Serializable]
public class GuildBattle
{
    public string OpponentId;
    public List<GuildBattleEntry> BattleEntries;
    public int Number;
    public GuildData Opponent => DataManager.Instance.GameData.Guilds.Find(_guild => _guild.Id == OpponentId);

    public int Points
    {
        get
        {
            int _points = 0;

            foreach (var _entry in BattleEntries)
            {
                _points += _entry.Points;
            }
            
            return _points;
        }
    }
}
