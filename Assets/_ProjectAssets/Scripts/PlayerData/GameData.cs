using System;
using System.Collections.Generic;
using System.Linq;
using BoomDaoWrapper;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class GameData
{
    public const string CLAIM_PREMIUM_REWARD = "battlePassPremium";
    public const string CLAIM_NORMAL_REWARD = "battlePassNormal";
    public const string LEADERBOARD_POINTS = "leaderboardPoints";
    public const string LEADERBOARD_KITTY_URL = "kittyUrl";
    public const string LEADERBOARD_SEASON = "leaderboardSeason";
    public const string LEADERBOARD_NICK_NAME = "nickName";
    public const string CURENT_SEASON = "currentSeason";
    
    private const string SEASON_KEY = "season";
    private const string SEASON_NUMBER = "number";
    private const string SEASON_START = "startDate";
    private const string SEASON_END = "endDate";
    private const string GLASS_MILK_PRICE = "milkGlass";
    private const string BOTTLE_MILK_PRICE = "milkBottle";
    private const string PRICE_TAG = "price";
    private const string AMOUNT_OF_REWARDS = "amountOfRewards";
    
    public const string GUILD_ID = "guildId";
    public const string GUILD_NAME = "name";
    public const string GUILD_KINGDOM_NAME = "kingdomName";
    public const string GUILD_OWNER = "guildOwner";
    public const string GUILD_BADGE_NAME = "badgeName";
    public const string GUILD_POINTS_REQUIREMENT = "pointsRequirement";
    public const string GUILD_PLAYERS = "players";

    public const string GUILD_BATTLES = "guildBattles";
    public const string GUILD_BATTLE_POINTS = "guildBattlePoints";
    public const string KITTY_AVATAR = "kittyAvatar";
    public const string GUILD_HISTORY = "guildHistory";
    public const string GUILD_BATTLES_START = "guildBattlesStart";
    public const string GUILD_BATTLES_END = "guildBattlesEnd";
    public const string GUILD_BATTLES_REVIEW = "guildBattlesReview";
    public const string GUILD_BATTLE_ID = "GuildBattleId";
    public const string GUILD_BATTLES_GUILD1 = "Guild1";
    public const string GUILD_BATTLES_GUILD2 = "Guild2";
    
    public const string KITTY_RECOVERY_KEY = "recoveryDate";
    public string KittyKey => GameState.selectedNFT.IsDefaultKitty ? "kitty_id" : "kittyId";
    
    private List<LevelReward> seasonRewards;
    
    public double RespinPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble("respinPrice", "cost");

    public int SeasonNumber => BoomDaoUtility.Instance.GetConfigDataAsInt(SEASON_KEY, SEASON_NUMBER);
    public DateTime SeasonStarts => BoomDaoUtility.Instance.GetConfigDataAsDate(SEASON_KEY, SEASON_START);
    public DateTime SeasonEnds => BoomDaoUtility.Instance.GetConfigDataAsDate(SEASON_KEY, SEASON_END);
    public bool HasSeasonEnded => DateTime.UtcNow > SeasonEnds;
    public bool HasSeasonStarted => DateTime.UtcNow > SeasonStarts;
    public bool IsSeasonActive => HasSeasonStarted && !HasSeasonEnded;
    public double GlassOfMilkPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble(GLASS_MILK_PRICE, PRICE_TAG);

    public double JugOfMilkPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble(BOTTLE_MILK_PRICE, PRICE_TAG);
    
    public List<LevelReward> SeasonRewards
    {
        get
        {
            if (seasonRewards==default)
            {
                FetchSeasonRewards();        
            }

            return seasonRewards;
        }
    }

    private void FetchSeasonRewards()
    {
        seasonRewards = new List<LevelReward>();
        int _amountOfRewards = BoomDaoUtility.Instance.GetConfigDataAsInt(SEASON_KEY, AMOUNT_OF_REWARDS);
        for (int _i = 1; _i <= _amountOfRewards; _i++)
        {
            TryAddReward(CLAIM_NORMAL_REWARD+_i,_i,false);
            TryAddReward(CLAIM_PREMIUM_REWARD+_i,_i,true);
        }

        void TryAddReward(string _id,int _level,bool _isPremium)
        {
            List<ActionOutcome> _actionOutcomes = BoomDaoUtility.Instance.GetActionOutcomes(_id);
            if (_actionOutcomes==default || _actionOutcomes.Count==0)
            {
                return;
            }

            ActionOutcome _outcome = _actionOutcomes[0];
            int _amount = Convert.ToInt32(_outcome.Value);
            ItemType _rewardType = Utilities.GetRewardType(_outcome.Name);
            LevelReward _levelReward = new LevelReward
            {
                Level = _level,
                Name = _rewardType.ToString(),
                Type = _rewardType,
                IsPremium = _isPremium,
                Parameter1 = _amount
            };
            
            seasonRewards.Add(_levelReward);
        }
    }

    public LeaderboardData GetLeaderboard
    {
        get
        {
            LeaderboardData _leaderboardData = new LeaderboardData();
            List<WorldDataEntry> _entries = BoomDaoUtility.Instance.GetWorldData(LEADERBOARD_POINTS,LEADERBOARD_NICK_NAME, LEADERBOARD_KITTY_URL, 
            PlayerData.SEASON_LEVEL, GUILD_BATTLE_POINTS, LEADERBOARD_SEASON);
            foreach (var _worldEntry in _entries)
            {
                if (_worldEntry.GetProperty(LEADERBOARD_NICK_NAME)== null)
                {
                    continue;
                }
                
                string _leaderboardSeason = _worldEntry.GetProperty(LEADERBOARD_SEASON);
                if (string.IsNullOrEmpty(_leaderboardSeason))
                {
                    continue;
                }
                int _season = BoomDaoUtility.Instance.ConvertToInt(_leaderboardSeason);
                if (_season<LeaderboardSeason)
                {
                    continue;
                }
                string _pointsString = _worldEntry.GetProperty(LEADERBOARD_POINTS);
                string _levelString = _worldEntry.GetProperty(PlayerData.SEASON_LEVEL);
                string _guildBattlePointsString = _worldEntry.GetProperty(GUILD_BATTLE_POINTS);
                if (string.IsNullOrEmpty(_levelString))
                {
                    _levelString = "0";
                }

                if (string.IsNullOrEmpty(_pointsString))
                {
                    _pointsString = "0";
                }

                if (string.IsNullOrEmpty(_guildBattlePointsString))
                {
                    _guildBattlePointsString = "";
                }
             
                int _points = BoomDaoUtility.Instance.ConvertToInt(_pointsString);
                int _level = BoomDaoUtility.Instance.ConvertToInt(_levelString);
                int _guildPoints = BoomDaoUtility.Instance.ConvertToInt(_guildBattlePointsString);
                
                _leaderboardData.Entries.Add(new LeaderboardEntries
                {
                    PrincipalId = _worldEntry.PrincipalId,
                    Nickname = _worldEntry.GetProperty(LEADERBOARD_NICK_NAME),
                    Points = _points,
                    KittyUrl = _worldEntry.GetProperty(LEADERBOARD_KITTY_URL),
                    Level = _level,
                    GuildBattlePoints = _guildPoints,
                    LeaderboardSeason = _season
                });
            }

            _leaderboardData.FinishSetup(3);
            return _leaderboardData;
        }
    }
    
    public DailyChallenges DailyChallenges
    {
        get
        {
            DailyChallenges _dailyChallenges = new DailyChallenges();
            for (int _i = 0; _i < ChallengesManager.AMOUNT_OF_CHALLENGES; _i++)
            {
                ChallengeData _challenge = GetChallenge(_i);
                if (_challenge==default)
                {
                    continue;
                }
                
                _dailyChallenges.Challenges.Add(_challenge);
            }

            string _nextResetString = BoomDaoUtility.Instance.GetString(ChallengesManager.DAILY_CHALLENGES,ChallengesManager.NEXT_RESET);
            if (string.IsNullOrEmpty(_nextResetString))
            {
                return default;
            }

            double _nextResetLong = BoomDaoUtility.Instance.ConvertToDouble(_nextResetString);
            _dailyChallenges.NextReset = Utilities.NanosecondsToDateTime(_nextResetLong);
            
            return _dailyChallenges;
        }
    }

    public bool HasDailyChallenges => BoomDaoUtility.Instance.DoesEntityExist(ChallengesManager.DAILY_CHALLENGE + "0");
    
    private ChallengeData GetChallenge(int _index)
    {
        string _challengeId = ChallengesManager.DAILY_CHALLENGE + _index;
        if (!BoomDaoUtility.Instance.DoesEntityExist(_challengeId))
        {
            return default;
        }

        ChallengeData _challengeData = new ChallengeData();
        _challengeData.Id = BoomDaoUtility.Instance.GetInt(_challengeId, ChallengesManager.CHALLENGE_ID);
        _challengeData.Identifier = BoomDaoUtility.Instance.GetString(_challengeId, ChallengesManager.CHALLENGE_IDENTIFIER);
        _challengeData.Description = BoomDaoUtility.Instance.GetString(_challengeId, ChallengesManager.CHALLENGE_DESCRIPTION);
        _challengeData.AmountNeeded = BoomDaoUtility.Instance.GetInt(_challengeId, ChallengesManager.CHALLENGE_AMOUNT_NEEDED);
        _challengeData.RewardAmount = BoomDaoUtility.Instance.GetInt(_challengeId, ChallengesManager.CHALLENGE_REWARD_AMOUNT);
        _challengeData.RewardType = (ItemType)BoomDaoUtility.Instance.GetInt(_challengeId, ChallengesManager.CHALLENGE_REWARD_TYPE);
        _challengeData.Category = (ChallengeCategory)BoomDaoUtility.Instance.GetInt(_challengeId, ChallengesManager.CHALLENGE_CATEGORY);
        return _challengeData;
    }

    public ChallengeData GetChallengeByIdentifier(string _identifier)
    {
        if (DataManager.Instance.GameData.DailyChallenges==null)
        {
            return null;
        }
        
        if (DataManager.Instance.GameData.DailyChallenges.Challenges==null)
        {
            return null;
        }

        ChallengeData _challengeData = null;
        foreach (var _challenge in DataManager.Instance.GameData.DailyChallenges.Challenges)
        {
            if (_challenge.Identifier != _identifier)
            {
                continue;
            }
            _challengeData = _challenge;
            break;
        }

        return _challengeData;
    }

    public int GetChallengeIndex(ChallengeProgress _challengeProgress)
    {
        for (int _i = 0; _i < DailyChallenges.Challenges.Count; _i++)
        {
            ChallengeData _data = DailyChallenges.Challenges[_i];
            if (_data.Identifier==_challengeProgress.Identifier)
            {
                return _i;
            }
        }

        throw new Exception("Can't find index of challenge");
    }
    
    public int GetChallengeIndex(string _identifier)
    {
        for (int _i = 0; _i < DailyChallenges.Challenges.Count; _i++)
        {
            ChallengeData _data = DailyChallenges.Challenges[_i];
            if (_data.Identifier==_identifier)
            {
                return _i;
            }
        }

        throw new Exception("Can't find index of challenge");
    }
    
    public DateTime GetKittyRecoveryDate(string _kittyId)
    {
        string _recoveryDateString = GetKittyRecoveryString(_kittyId);
        
        if (string.IsNullOrEmpty(_recoveryDateString))
        {
            return DateTime.MinValue;
        }
        
        DateTime _recoveryDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.ConvertToDouble(_recoveryDateString));
        if (_recoveryDate <= DateTime.UtcNow)
        {
            _recoveryDate = default;
        }

        return _recoveryDate;
    }
    
    public bool IsKittyHurt(string _kittyId)
    {
        string _recoveryDateString = GetKittyRecoveryString(_kittyId);
        
        if (string.IsNullOrEmpty(_recoveryDateString))
        {
            return false;
        }

        var _recoveryDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.ConvertToDouble(_recoveryDateString));
        
        
        return _recoveryDate > DateTime.UtcNow;
    }

    public string GetAvatarUrl()
    {
        List<WorldDataEntry> _kittyAvatars = BoomDaoUtility.Instance.GetWorldData(KITTY_AVATAR);
        if (_kittyAvatars == null)
        {
            return string.Empty;
        }

        string _avatarUrl = string.Empty;

        foreach (var _kittyAvatar in _kittyAvatars)
        {
            if (_kittyAvatar.PrincipalId != BoomDaoUtility.Instance.UserPrincipal)
            {
                continue;
            }

            return _kittyAvatar.GetProperty(KITTY_AVATAR);
        }

        return _avatarUrl;
    }

    private string GetKittyRecoveryString(string _kittyId)
    {
        List<WorldDataEntry> _recoveryEntries = BoomDaoUtility.Instance.GetWorldData(KITTY_RECOVERY_KEY);
        if (_recoveryEntries==null)
        {
            return string.Empty;
        }

        string _recoveryDateString = string.Empty;

        foreach (var _recoveryEntire in _recoveryEntries)
        {
            if (_recoveryEntire.PrincipalId != _kittyId)
            {
                continue;
            }

            if (_recoveryEntire.Data.Count == 0)
            {
                continue;
            }
            
            foreach (var _worldDataEntry in _recoveryEntire.Data)
            {
                if (_worldDataEntry.Key != KITTY_RECOVERY_KEY)
                {
                    continue;
                }
            
                _recoveryDateString = _worldDataEntry.Value;
                break;
            }
        }
        

        return _recoveryDateString;
    }

    public FlagData FlagData
    {
        get
        {
            List<ConfigData> _flagConfigData = BoomDaoUtility.Instance.GetConfigData("mainMenuFlag");
            FlagData _data = new FlagData
            {
                ImageUrl = _flagConfigData.FindEntityValue("imageUrl"),
                Description = _flagConfigData.FindEntityValue("description").ReplaceBackslashN()
            };
            return _data;
        }
    }

    public List<GuildData> Guilds
    {
        get
        {
            List<WorldDataEntry> _entries = BoomDaoUtility.Instance.GetWorldData(GUILD_ID, GUILD_NAME, GUILD_PLAYERS, GUILD_BADGE_NAME, 
            GUILD_KINGDOM_NAME, GUILD_POINTS_REQUIREMENT, GUILD_OWNER, GUILD_BATTLES_GUILD1,GUILD_BATTLES_GUILD2,GUILD_BATTLE_ID);
            
            if (_entries==null)
            {
                return new List<GuildData>();
            }
            
            var _leaderboardEntries = GetLeaderboard.Entries;
            List<GuildData> _guilds = new();
            DataManager.Instance.PlayerData.GuildId = string.Empty;
            foreach (var _worldEntry in _entries)
            {
                if (_worldEntry.GetProperty(GUILD_KINGDOM_NAME)== null)
                {
                    continue;
                }
                
                GuildData _guildData = new GuildData
                {
                    Id = _worldEntry.GetProperty(GUILD_ID),
                    Name = _worldEntry.GetProperty(GUILD_NAME),
                    KingdomName = _worldEntry.GetProperty(GUILD_KINGDOM_NAME),
                    BadgeName = _worldEntry.GetProperty(GUILD_BADGE_NAME),
                    PointsRequirement = BoomDaoUtility.Instance.ConvertToInt(_worldEntry.GetProperty(GUILD_POINTS_REQUIREMENT)),
                    Owner = _worldEntry.GetProperty(GUILD_OWNER)
                };

                string _guildBattleId = _worldEntry.GetProperty(GUILD_BATTLE_ID);
                if (string.IsNullOrEmpty(_guildBattleId))
                {
                    _guildBattleId = string.Empty;
                }

                _guildData.GuildBattleId = _guildBattleId;
                
                _guilds.Add(_guildData);

                string _guildHistory = _worldEntry.GetProperty(GUILD_HISTORY);
                if (!string.IsNullOrEmpty(_guildHistory))
                {
                    if (_guildHistory.Contains(","))
                    {
                        List<string>  _history= _guildHistory.Split(".").ToList();
                        foreach (var _his in _history)
                        {
                            _guildData.BattlesHistory.Add(BoomDaoUtility.Instance.ConvertToInt(_his));
                        }
                    }
                    else
                    {
                        _guildData.BattlesHistory.Add(BoomDaoUtility.Instance.ConvertToInt(_guildHistory));
                    }
                }

                string _playersString = _worldEntry.GetProperty(GUILD_PLAYERS);
                if (string.IsNullOrEmpty(_playersString))
                {
                    _guilds.Remove(_guildData);
                    continue;
                }
                
                List<string> _playersPrincipals = new();
                if (_playersString.Contains(","))
                {
                    _playersPrincipals = _playersString.Split(",").ToList();
                }
                else
                {
                    _playersPrincipals.Add(_playersString);
                }
                
                List<GuildPlayerData> _players = new ();
                foreach (var _playerPrincipal in _playersPrincipals)
                {
                    LeaderboardEntries _leaderboardEntry = _leaderboardEntries.Find(_entry => _entry.PrincipalId == _playerPrincipal);
                    GuildPlayerData _guildPlayer = new();
                    _players.Add(_guildPlayer);
                    _guildPlayer.Principal = _playerPrincipal;
                    _guildPlayer.IsLeader = _playerPrincipal == _guildData.Owner;
                    if (_playerPrincipal == BoomDaoUtility.Instance.UserPrincipal)
                    {
                        DataManager.Instance.PlayerData.GuildId = _guildData.Id;
                    }

                    if (_leaderboardEntry == null)
                    {
                        _guildPlayer.Points = 0;
                        _guildPlayer.Name = "NoEntry";
                        _guildPlayer.GuildBattlePoints = 0;
                        continue;
                    }
                    
                    _guildPlayer.Points = _leaderboardEntry.Points;
                    _guildPlayer.Name = _leaderboardEntry.Nickname;
                    _guildPlayer.Level = _leaderboardEntry.Level;
                    _guildPlayer.GuildBattlePoints = _leaderboardEntry.GuildBattlePoints;
                }

                if (!string.IsNullOrEmpty(_guildData.GuildBattleId))
                {
                    WorldDataEntry _guildBattleEntry = _entries.Find(_entry =>
                        _entry.GetProperty(GUILD_BATTLES_GUILD1) != null && _entry.GetProperty(GUILD_BATTLE_ID) == _guildData.GuildBattleId);
                    if (_guildBattleEntry!=null)
                    {
                        _guildData.GuildBattle = new GuildBattle
                        {
                            Guild1Id = _guildBattleEntry.GetProperty(GUILD_BATTLES_GUILD1),
                            Guild2Id = _guildBattleEntry.GetProperty(GUILD_BATTLES_GUILD2),
                        };
                    }
                }

                SetOpponentBattleEntries(_guildData.GuildBattle.Guild1Id, _entries, _leaderboardEntries, _guildData.GuildBattle);
                SetOpponentBattleEntries(_guildData.GuildBattle.Guild2Id, _entries, _leaderboardEntries, _guildData.GuildBattle);
                _guildData.Players = _players.OrderByDescending(_player => _player.Points).ToList();
            }

            return _guilds;
        }
    }

    private void SetOpponentBattleEntries(string _opponentId, List<WorldDataEntry> _entries, List<LeaderboardEntries> _leaderboardEntries, 
    GuildBattle _battle)
    {
        if (string.IsNullOrEmpty(_opponentId))
        {
            return;
        }
        
        WorldDataEntry _opponentGuild = _entries.Find(_worldEntry =>
            _worldEntry.GetProperty(GUILD_ID) == _opponentId && _worldEntry.GetProperty(GUILD_KINGDOM_NAME) != null);

        if (_opponentGuild == null)
        {
            return;
        }
        
        string _playersString = _opponentGuild.GetProperty(GUILD_PLAYERS);
        if (string.IsNullOrEmpty(_playersString))
        {
            return;
        }
        
        List<string> _playersPrincipals = new();
        if (_playersString.Contains(","))
        {
            _playersPrincipals = _playersString.Split(",").ToList();
        }
        else
        {
            _playersPrincipals.Add(_playersString);
        }
        
        foreach (var _playerPrincipal in _playersPrincipals)
        {
            LeaderboardEntries _leaderboardEntry = _leaderboardEntries.Find(_entry => _entry.PrincipalId == _playerPrincipal);
            if (_leaderboardEntry ==null)
            {
                continue;
            }
            
            _battle.BattleEntries.Add( new GuildBattleEntry { Principal = _leaderboardEntry.PrincipalId, Points = _leaderboardEntry
                .GuildBattlePoints, GuildId = _opponentId});
        }
    }

    public GuildBattles GuildBattles
    {
        get
        {
            GuildBattles _guildBattles = new GuildBattles();
            List<ConfigData> _config =BoomDaoUtility.Instance.GetConfigData(GUILD_BATTLES);
            string _startingDate = _config.Find(_configField => _configField.Name == GUILD_BATTLES_START).Value;
            string _endingDate = _config.Find(_configField => _configField.Name == GUILD_BATTLES_END).Value;
            string _reviewDate = _config.Find(_configField => _configField.Name == GUILD_BATTLES_REVIEW).Value;
            
            _guildBattles.StartingDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.ConvertToDouble(_startingDate));
            _guildBattles.EndingDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.ConvertToDouble(_endingDate));
            _guildBattles.ReviewDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.ConvertToDouble(_reviewDate));

            return _guildBattles;
        }
    }

    public double GuildPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "price");
    public int LeaderboardSeason => BoomDaoUtility.Instance.GetConfigDataAsInt(LEADERBOARD_SEASON, "currentSeason");
    public string GuildPriceAsString => BoomDaoUtility.Instance.GetConfigDataAsString("guildsConfig", "price");
    public double MaxGuildPlayers => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "maxPlayers");
    public double GuildGoldBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "goldBar");
    public double GuildSilverBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "silverBar");
    public double GuildBronzeBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "bronzeBar");
}
