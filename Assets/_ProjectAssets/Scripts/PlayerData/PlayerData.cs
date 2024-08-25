using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BoomDaoWrapper;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerData
{
    
    private List<int> ownedEquiptables;
    private List<int> ownedEmojis;
    private DailyChallenges dailyChallenges = new();
    public void SetStartingValues()
    {
        ownedEquiptables = new List<int>()
        {
            0,
            25,
            60,
            74,
            95
        };
        ownedEmojis = new List<int>()
        {
            0,
            1,
            2,
            3,
            4
        };
    }
    public List<int> OwnedEquiptables
    {
        get { return ownedEquiptables; }
        set { ownedEquiptables = value; }
    }
    public void AddOwnedEquipment(int _id)
    {
        if (ownedEquiptables.Contains(_id))
        {
            return;
        }

        ownedEquiptables.Add(_id);
    }
    public void RemoveOwnedEquipment(int _id)
    {
        if (!ownedEquiptables.Contains(_id))
        {
            return;
        }

        ownedEquiptables.Remove(_id);
    }
    public List<int> OwnedEmojis
    {
        get => ownedEmojis;
        set
        {
            ownedEmojis = value;
            ownedEmojis.Sort();
        }
    }
    public void AddOwnedEmoji(int _id)
    {
        if (ownedEmojis.Contains(_id))
        {
            return;
        }

        ownedEmojis.Add(_id);
        ownedEmojis.Sort();
    }

    // new system
    public const string SNACKS = "snack";
    public const string FREE_EMOJI = "freeEmoji";
    public const string LEADERBOARD_POINTS = "leaderboardPoints";
    public const string EARNED_XP_KEY = "earnedXp";
    
    public const string NAME_KEY = "username";

    public const string COMMON_SHARD = "commonShard";
    public const string UNCOMMON_SHARD = "uncommonShard";
    public const string RARE_SHARD = "rareShard";
    public const string EPIC_SHARD = "epicShard";
    public const string LEGENDARY_SHARD = "legendaryShard";
    
    public const string MILK_BOTTLE = "milkBottle";
    public const string MILK_GLASS = "milkGlass";
    
    public const string USER_PROFILE = "user_profile";

    private const string XP = "xp";
    public const string SEASON_LEVEL = "seasonLevel";
    private const string AMOUNT_KEY = "amount";

    private const string CLAIMED_NORMAL_REWARDS = "claimedNormalRewards";
    private const string CLAIMED_PREMIUM_REWARDS = "claimedPremiumRewards";
    private const string HAS_PREMIUM_PASS = "hasPremiumPass";
    
    
    public const string CRAFTING_PROCESS = "craftingProcess";
    private const string CRAFTING_END = "craftingEnd";
    public const string CRAFTING_INGREDIENT = "craftingIngredient";

    public const string DAILY_CHALLENGE_PROGRESS = "dailyChallengeProgress";

    public const string AMOUNT_OF_GAMES_PLAYED_TODAY = "amountOfGamesPlayedToday";
    public const string RESET_AMOUNT_OF_GAMES_PLAYED_TODAY = "resetAmountOfGamesPlayedToday";
    
    public static Action OnUpdatedSnacks;
    public static Action OnUpdatedShards;
    public static Action OnUpdatedExp;
    public static Action OnUpdatedJugOfMilk;
    public static Action OnUpdatedGlassOfMilk;
    public static Action OnClaimedReward;
    public static Action OnBoughtPass;
    public static Action OnUpdatedCraftingProcess;
    public static Action OnUpdatedToken;

    public int Level { get; private set; }
    public int ExperienceOnCurrentLevel { get; private set; }
    public int ExperienceForNextLevel { get; private set; }
    
    public string UseMilkBottle => GameState.selectedNFT.IsDefaultKitty ? "useMilkBottle" : "useMilkBottle2";
    public string UseMilkGlass => GameState.selectedNFT.IsDefaultKitty ? "useMilkGlass" : "useMilkGlass2";

    public string Username => BoomDaoUtility.Instance.GetString(USER_PROFILE, NAME_KEY);
    public double Snacks => BoomDaoUtility.Instance.GetDouble(SNACKS, AMOUNT_KEY);
    public double CommonShard => BoomDaoUtility.Instance.GetDouble(COMMON_SHARD, AMOUNT_KEY);
    public double UncommonShard => BoomDaoUtility.Instance.GetDouble(UNCOMMON_SHARD, AMOUNT_KEY);
    public double RareShard => BoomDaoUtility.Instance.GetDouble(RARE_SHARD, AMOUNT_KEY);
    public double EpicShard => BoomDaoUtility.Instance.GetDouble(EPIC_SHARD, AMOUNT_KEY);
    public double LegendaryShard => BoomDaoUtility.Instance.GetDouble(LEGENDARY_SHARD, AMOUNT_KEY);
    public double TotalCrystals => CommonShard + UncommonShard + RareShard + EpicShard + LegendaryShard;
    
    public double JugOfMilk => BoomDaoUtility.Instance.GetDouble(MILK_BOTTLE, AMOUNT_KEY);

    public double GlassOfMilk => BoomDaoUtility.Instance.GetDouble(MILK_GLASS, AMOUNT_KEY);

    public double Experience => BoomDaoUtility.Instance.GetDouble(XP, AMOUNT_KEY);
    
    public bool HasClaimed(LevelReward _reward, int _level)
    {
        foreach (var _claimedReward in GetClaimedRewards())
        {
            if (_claimedReward.IsPremium == _reward.IsPremium && _claimedReward.Level == _level)
            {
                return true;
            }
        }

        return false;
    }
    
    private List<ClaimedReward> GetClaimedRewards()
    {
        List<ClaimedReward> _claimedLevelRewards = new List<ClaimedReward>();
        List<int> _normalRewards = BoomDaoUtility.Instance.GetListOfInts(CLAIMED_NORMAL_REWARDS);
        List<int> _premiumRewards = BoomDaoUtility.Instance.GetListOfInts(CLAIMED_PREMIUM_REWARDS);
        AddClaimedReward(_normalRewards,false);
        AddClaimedReward(_premiumRewards,true);

        return _claimedLevelRewards;
        void AddClaimedReward(List<int> _levels, bool _isPremium)
        {
            if (_levels==default)
            {
                return;
            }
            
            foreach (var _level in _levels)
            {
                ClaimedReward _claimedReward = new ClaimedReward { Level = _level, IsPremium = _isPremium };
                _claimedLevelRewards.Add(_claimedReward);
            }
        }
    }

    public bool HasPass => BoomDaoUtility.Instance.GetDouble(HAS_PREMIUM_PASS, AMOUNT_KEY) > 0.5f;

    public void SubscribeEvents()
    {
        BoomDaoUtility.OnDataUpdated += RiseEvent;
        CalculateLevel();
    }

    public void UnsubscribeEvents()
    {
        BoomDaoUtility.OnDataUpdated += RiseEvent;
    }

    private void RiseEvent(string _key)
    {
        switch (_key)
        {
            case XP:
                CalculateLevel();
                OnUpdatedExp?.Invoke();
                break;
            case COMMON_SHARD:
            case UNCOMMON_SHARD:
            case RARE_SHARD:
            case EPIC_SHARD:
            case LEGENDARY_SHARD:
                OnUpdatedShards?.Invoke();
                break;
            case MILK_GLASS:
                OnUpdatedGlassOfMilk?.Invoke();
                break;
            case MILK_BOTTLE:
                OnUpdatedJugOfMilk?.Invoke();
                break;
            case SNACKS:
                OnUpdatedSnacks?.Invoke();
                break;
            case CLAIMED_PREMIUM_REWARDS:
                OnClaimedReward?.Invoke();
                break;
            case CLAIMED_NORMAL_REWARDS:
                OnClaimedReward?.Invoke();
                break;
            case HAS_PREMIUM_PASS:
                OnBoughtPass?.Invoke();
                break;
            case CRAFTING_PROCESS:
                OnUpdatedCraftingProcess?.Invoke();
                break;
            default:
                 // Debug.Log($"--- {_key} got updated!, add handler?");
                break;
        }
    }

    public double GetAmountOfCrystals(ItemType _type)
    {
        switch (_type)
        {
            case ItemType.CommonShard:
                return CommonShard;
            case ItemType.UncommonShard:
                return UncommonShard;
            case ItemType.RareShard:
                return RareShard;
            case ItemType.EpicShard:
                return EpicShard;
            case ItemType.LegendaryShard:
                return LegendaryShard;
        }

        throw new Exception("Unsupported type of shards: " + _type);
    }

    private void CalculateLevel()
    {
        CalculateLevel(Experience, out var _level, out var _expForNextLevel, out var _experienceOnCurrentLevel);

        Level = _level;
        ExperienceForNextLevel = _expForNextLevel;
        ExperienceOnCurrentLevel = _experienceOnCurrentLevel;
    }
    
    public CraftingProcess CraftingProcess
    {
        get
        {
            if (!IsCrafting)
            {
                return default;
            }
            
            DateTime _endDate = Utilities.NanosecondsToDateTime(BoomDaoUtility.Instance.GetDouble(CRAFTING_PROCESS, CRAFTING_END));
            if (_endDate==default)
            {
                return default;
            }
            
            string _ingredientString = BoomDaoUtility.Instance.GetString(CRAFTING_PROCESS, CRAFTING_INGREDIENT);
            ItemType _type = Utilities.GetRewardType(_ingredientString);

            return new CraftingProcess() { EndDate = _endDate, EndProduct = _type};
        }
    }

    public bool IsCrafting => BoomDaoUtility.Instance.DoesEntityExist(CRAFTING_PROCESS);

    public List<ChallengeProgress> ChallengeProgresses
    {
        get
        {
            List<ChallengeProgress> _progresses = new List<ChallengeProgress>();
            for (int _i = 0; _i < ChallengesManager.AMOUNT_OF_CHALLENGES; _i++)
            {
                ChallengeProgress _progress = GetChallengeProgress(_i);
                if (_progress==default)
                {
                    continue;
                }

                _progresses.Add(_progress);
            }

            return _progresses;
        }
    }

    private ChallengeProgress GetChallengeProgress(int _challengeIndex)
    {
        string _progressId = DAILY_CHALLENGE_PROGRESS + _challengeIndex;
        ChallengeProgress _progress = new ChallengeProgress
            {
                Identifier = BoomDaoUtility.Instance.GetString(_progressId, ChallengesManager.CHALLENGE_IDENTIFIER),
                Value = BoomDaoUtility.Instance.GetInt(_progressId, ChallengesManager.PROGRESS_VALUE),
                Claimed = BoomDaoUtility.Instance.GetInt(_progressId, ChallengesManager.PROGRESS_CLAIMED)==1,
            };
        return _progress;
    }

    public bool HasClaimedChallengeSpin => BoomDaoUtility.Instance.DoesEntityExist(ChallengesManager.CLAIMED_LUCKY_SPIN);

    public bool HasClaimedFreeEmoji => BoomDaoUtility.Instance.DoesEntityExist(FREE_EMOJI);

    public static void CalculateLevel(double _exp, out int _level, out int _expForNextLevel, out int _experienceOnCurrentLevel)
    {
        double _experience = _exp;
        int _calculatedLevel = 1;
        int _levelScale = 100;
        float _calculatedExpForNextLevel = 100;

        if (_experience < _calculatedExpForNextLevel)
        {
            _experienceOnCurrentLevel = (int)_experience;
        }
        else
        {
            while (_experience >= _calculatedExpForNextLevel)
            {
                _calculatedLevel++;
                _experience -= _calculatedExpForNextLevel;
                _calculatedExpForNextLevel += (_calculatedExpForNextLevel * ((float)_levelScale / 100));
            }
        }

        _expForNextLevel = (int)_calculatedExpForNextLevel;
        _experienceOnCurrentLevel = (int)_experience;
        _level = _calculatedLevel;
    }
    
    public bool ShouldBotBeEasy
    {
        get
        {
            double _amountOfGamesPlayedToday = BoomDaoUtility.Instance.GetDouble(AMOUNT_OF_GAMES_PLAYED_TODAY, BoomDaoUtility.AMOUNT_KEY);
            return _amountOfGamesPlayedToday<=5;
        }
    }

    public int LeaderboardPoints => DataManager.Instance.GameData.GetLeaderboard.Entries.Find(_entry => _entry.PrincipalId == BoomDaoUtility
        .Instance.UserPrincipal).Points;

    public string GuildId;

    public bool IsInAGuild => !string.IsNullOrEmpty(GuildId);

    public GuildData Guild
    {
        get
        {
            foreach (var _guild in DataManager.Instance.GameData.Guilds)
            {
                var _guildPlayer = _guild.Players.Find(_player => _player.Principal == BoomDaoUtility.Instance.UserPrincipal);
                if (_guildPlayer == null)
                {
                    continue;
                }

                return _guild;
            }

            return null;
        }
    }

    public void GetMyGuild()
    {
        //this needs to be here, since getting Guilds will set my guild id
        Debug.Log("There are: "+DataManager.Instance.GameData.Guilds.Count+" guilds");
    }
}