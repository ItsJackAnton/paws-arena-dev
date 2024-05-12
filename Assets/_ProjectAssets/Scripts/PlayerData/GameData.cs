using System;
using System.Collections.Generic;
using System.Linq;
using BoomDaoWrapper;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class GameData
{
    public double RespinPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble("respinPrice", "cost");

    //new system
    public const string CLAIM_PREMIUM_REWARD = "battlePassPremium";
    public const string CLAIM_NORMAL_REWARD = "battlePassNormal";
    public const string LEADERBOARD_POINTS = "leaderboardPoints";
    public const string LEADERBOARD_KITTY_URL = "kittyUrl";
    public const string LEADERBOARD_NICK_NAME = "nickName";
    
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
    public const string GUILD_OWNER = "owner";
    public const string GUILD_BADGE_NAME = "badgeName";
    public const string GUILD_POINTS_REQUIREMENT = "pointsRequirement";
    public const string GUILD_PLAYERS = "players";

    
    public const string KITTY_RECOVERY_KEY = "recoveryDate";
    public string KittyKey => GameState.selectedNFT.IsDefaultKitty ? "kitty_id" : "kittyId";
    
    private List<LevelReward> seasonRewards;

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
            PlayerData.SEASON_LEVEL);
            foreach (var _worldEntry in _entries)
            {
                if (_worldEntry.GetProperty(LEADERBOARD_NICK_NAME)== null)
                {
                    continue;
                }
                string _pointsString = _worldEntry.GetProperty(LEADERBOARD_POINTS);
                string _levelString = _worldEntry.GetProperty(PlayerData.SEASON_LEVEL);
                if (string.IsNullOrEmpty(_levelString))
                {
                    _levelString = "0";
                }
             
                int _points = Convert.ToInt32(_pointsString.Contains('.') ? _pointsString.Split('.')[0] : _pointsString);
                int _level = Convert.ToInt32(_levelString.Contains('.') ? _levelString.Split('.')[0] : _levelString);
                
                _leaderboardData.Entries.Add(new LeaderboardEntries
                {
                    PrincipalId = _worldEntry.PrincipalId,
                    Nickname = _worldEntry.GetProperty(LEADERBOARD_NICK_NAME),
                    Points = _points,
                    KittyUrl = _worldEntry.GetProperty(LEADERBOARD_KITTY_URL),
                    Level = _level
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
            ulong _nextResetLong = _nextResetString.Contains('.')
                ? Convert.ToUInt64(_nextResetString.Split('.')[0])
                : Convert.ToUInt64(_nextResetString);
            
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
        
        DateTime _recoveryDate = Utilities.NanosecondsToDateTime(Convert.ToDouble(_recoveryDateString));
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

        DateTime _recoveryDate = Utilities.NanosecondsToDateTime(Convert.ToDouble(_recoveryDateString));
        
        return _recoveryDate > DateTime.UtcNow;
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
            GUILD_KINGDOM_NAME, GUILD_POINTS_REQUIREMENT, GUILD_OWNER);
            
            if (_entries==null)
            {
                Debug.Log("No entries found");
                return new List<GuildData>();
            }
            
            var _leaderboardEntries = GetLeaderboard.Entries;
            List<GuildData> _guilds = new();
            foreach (var _worldEntry in _entries)
            {
                if (_worldEntry.PrincipalId == "de572c03-e27f-4d67-8e06-84cef068a952")
                {
                    Debug.Log("Found guild");
                    Debug.Log(JsonConvert.SerializeObject(_worldEntry.Data));
                }
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
                    PointsRequirement = Convert.ToInt32(_worldEntry.GetProperty(GUILD_POINTS_REQUIREMENT)),
                    Owner = _worldEntry.GetProperty(GUILD_OWNER)
                };
                
                _guilds.Add(_guildData);

                if (string.IsNullOrEmpty(_worldEntry.GetProperty(GUILD_PLAYERS)))
                {
                    Debug.Log("Found guild but it has no players");
                    continue;
                }

                List<string> _playersPrincipals = _worldEntry.GetProperty(GUILD_PLAYERS).Split(",").ToList();
                List<GuildPlayerData> _players = new ();
                foreach (var _player in _playersPrincipals)
                {
                    LeaderboardEntries _leaderboardEntry = _leaderboardEntries.Find(_entry => _entry.PrincipalId == _player);
                    GuildPlayerData _guildPlayer = new();
                    _players.Add(_guildPlayer);
                    if (_leaderboardEntry == null)
                    {
                        _guildPlayer.Points = 0;
                        _guildPlayer.Name = "WaitingForLeaderboardEntry";
                        continue;
                    }

                    _guildPlayer.Points = _leaderboardEntry.Points;
                    _guildPlayer.Name = _leaderboardEntry.Nickname;
                    _guildPlayer.IsLeader = _player == _guildData.Owner;
                    _guildPlayer.Level = _leaderboardEntry.Level;
                }

                _guildData.Players = _players.OrderByDescending(_player => _player.Points).ToList();
            }

            return _guilds;
        }
    }

    public double GuildPrice => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "price");
    public string GuildPriceAsString => BoomDaoUtility.Instance.GetConfigDataAsString("guildsConfig", "price");
    public double MaxGuildPlayers => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "maxPlayers");
    public double GuildGoldBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "goldBar");
    public double GuildSilverBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "silverBar");
    public double GuildBronzeBar => BoomDaoUtility.Instance.GetConfigDataAsDouble("guildsConfig", "bronzeBar");
}
