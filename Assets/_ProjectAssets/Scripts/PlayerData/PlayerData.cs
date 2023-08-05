using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PlayerData
{
    private float snacks = 0;
    private float jugOfMilk = 0;
    private float glassOfMilk = 0;
    private CrystalsData crystals = new CrystalsData();
    private CraftingProcess craftingProcess;
    private bool hasPass;
    private int experience;
    private int level;
    private int experienceOnCurrentLevel;
    private int experienceForNextLevel;
    private List<ClaimedReward> claimedLevelRewards = new List<ClaimedReward>();
    private List<RecoveryEntrie> recoveringKitties = new List<RecoveryEntrie>();
    private List<int> ownedEquiptables = new List<int>() { 0, 25, 60, 74, 95 };
    private int seasonNumber;
    private List<int> ownedEmojis = new List<int>() { };
    private Challenges challenges = new Challenges();
    private string guildId = string.Empty;

    [JsonIgnore] public Action UpdatedSnacks;
    [JsonIgnore] public Action UpdatedJugOfMilk;
    [JsonIgnore] public Action UpdatedGlassOfMilk;
    [JsonIgnore] public Action UpdatedCraftingProcess;
    [JsonIgnore] public Action UpdatedClaimedLevels;
    [JsonIgnore] public Action UpdatedHasPass;
    [JsonIgnore] public Action UpdatedExp;
    [JsonIgnore] public Action UpdatedRecoveringKitties;
    [JsonIgnore] public Action UpdatedEquiptables;
    [JsonIgnore] public Action UpdatedSeasonNumber;
    [JsonIgnore] public Action UpdatedOwnedEmojis;
    [JsonIgnore] public Action UpdatedGuild;

    public PlayerData()
    {
    }

    public float Snacks
    {
        get { return snacks; }
        set
        {
            snacks = value;
            UpdatedSnacks?.Invoke();
        }
    }

    public float JugOfMilk
    {
        get { return jugOfMilk; }
        set
        {
            jugOfMilk = value;
            UpdatedJugOfMilk?.Invoke();
        }
    }

    public float GlassOfMilk
    {
        get { return glassOfMilk; }
        set
        {
            glassOfMilk = value;
            UpdatedGlassOfMilk?.Invoke();
        }

    }

    public CrystalsData Crystals
    {
        get => crystals;
        set => crystals = value;
    }

    public CraftingProcess CraftingProcess
    {
        get { return craftingProcess; }
        set
        {
            craftingProcess = value;
            UpdatedCraftingProcess?.Invoke();
        }
    }

    public int Experience
    {
        get { return experience; }
        set
        {
            experience = value;
            UpdatedExp?.Invoke();
            CalculateLevel(experience,out level,out experienceForNextLevel,out experienceOnCurrentLevel);
        }
    }

    [JsonIgnore]
    public int Level
    {
        get { return level; }
    }

    public void AddCollectedLevelReward(ClaimedReward _reward)
    {
        claimedLevelRewards.Add(_reward);
        UpdatedClaimedLevels?.Invoke();
    }

    public List<ClaimedReward> ClaimedLevelRewards
    {
        get { return claimedLevelRewards; }
        set { claimedLevelRewards = value; }
    }

    [JsonIgnore] public int ExperienceOnCurrentLevel => experienceOnCurrentLevel;
    [JsonIgnore] public int ExperienceForNextLevel => experienceForNextLevel;

    public bool HasPass
    {
        get { return hasPass; }
        set
        {
            hasPass = value;
            UpdatedHasPass?.Invoke();
        }
    }

    public bool HasClaimed(LevelReward _reward, int _level)
    {
        foreach (var _claimedReward in claimedLevelRewards)
        {
            if (_claimedReward.IsPremium == _reward.IsPremium && _claimedReward.Level == _level)
            {
                return true;
            }
        }

        return false;
    }

    public List<RecoveryEntrie> RecoveringKitties
    {
        get { return recoveringKitties; }
    }

    public void AddRecoveringKittie(RecoveryEntrie _recoveryEntrie)
    {
        recoveringKitties.Add(_recoveryEntrie);
        UpdatedRecoveringKitties?.Invoke();
    }

    public void RemoveRecoveringKittie(string _imageUrl)
    {
        RecoveryEntrie _entry = null;

        foreach (var _recovery in recoveringKitties)
        {
            if (_recovery.KittyImageUrl == _imageUrl)
            {
                _entry = _recovery;
                break;
            }
        }

        if (_entry == null)
        {
            return;
        }

        recoveringKitties.Remove(_entry);
        UpdatedRecoveringKitties?.Invoke();
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
        UpdatedEquiptables?.Invoke();
    }

    public void RemoveOwnedEquipment(int _id)
    {
        if (!ownedEquiptables.Contains(_id))
        {
            return;
        }

        ownedEquiptables.Remove(_id);
        UpdatedEquiptables?.Invoke();
    }

    public int SeasonNumber
    {
        get => seasonNumber;
        set
        {
            seasonNumber = value;
            UpdatedSeasonNumber?.Invoke();
        }
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
        UpdatedOwnedEmojis?.Invoke();
    }

    public Challenges Challenges
    {
        get => challenges;
        set => challenges = value;
    }


    public static void CalculateLevel(int _exp, out int level, out int expForNextLevel, out int experienceOnCurrentLevel)
    {
        float _experience = _exp;
        int _level = 1;
        float _expForNextLevel = DataManager.Instance.GameData.LevelBaseExp;

        if (_experience < DataManager.Instance.GameData.LevelBaseExp)
        {
            experienceOnCurrentLevel = (int)_experience;
            _expForNextLevel = DataManager.Instance.GameData.LevelBaseExp;
        }
        else
        {
            while (_experience >= _expForNextLevel)
            {
                _level++;
                _experience -= _expForNextLevel;
                _expForNextLevel = _expForNextLevel +
                                   (_expForNextLevel * ((float)DataManager.Instance.GameData.LevelBaseScaler / 100));
            }
        }

        expForNextLevel = (int)_expForNextLevel;
        experienceOnCurrentLevel = (int)_experience;
        level = _level;
    }

    public string GuildId
    {
        get => guildId;
        set
        {
            guildId = value;
            UpdatedGuild?.Invoke();
        }
    }

    [JsonIgnore] public string PlayerId => FirebaseManager.Instance.PlayerId;
    [JsonIgnore] public bool IsInGuild => !string.IsNullOrEmpty(GuildId);
    
    [JsonIgnore] public GuildData Guild
    {
        get
        {
            if (!IsInGuild)
            {
                return null;
            }

            GuildData _guild = DataManager.Instance.GameData.Guilds[guildId];
            _guild.Players = _guild.Players.OrderBy(_element => _element.Points).ToList();
            for (int i = 0; i < _guild.Players.Count; i++)
            {
                _guild.Players[i].Place = i + 1;
            }
            return _guild;
        }
    }
}
