using System.Collections.Generic;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMy : MonoBehaviour
{
    private const string LEAVE_GUILD = "leaveGuild";
    private const string KICK_PLAYER_GUILD = "kickPlayerGuild";
    private const string DELETE_GUILD = "deleteGuild";
    
    [SerializeField] private GameObject holder;
    [SerializeField] private Image badgeDisplay;
    [SerializeField] private Image kingdomDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI membersDisplay;
    [SerializeField] private TextMeshProUGUI battlesWonDisplay;
    [SerializeField] private TextMeshProUGUI leaderDisplay;
    [SerializeField] private Transform playersHolder;
    [SerializeField] private GuildPlayerDisplay guildPlayerPrefab;
    [SerializeField] private Button showUp;
    [SerializeField] private Button showDown;
    [SerializeField] private Button quit;

    private float moveAmount=1;
    private List<GameObject> shownObjects = new();
    private GuildPlayerData kickPlayer;
    private string deleteGuildId;

    public void Setup()
    {
        GuildData _guild = DataManager.Instance.PlayerData.Guild;
        badgeDisplay.sprite = _guild.Badge;
        kingdomDisplay.sprite = _guild.Kingdom;
        nameDisplay.text = _guild.Name;
        membersDisplay.text = $"Members: {_guild.Players.Count}/{DataManager.Instance.GameData.MaxGuildPlayers}";
        battlesWonDisplay.text = "Battles won: " + _guild.BattlesWon;
        leaderDisplay.text = _guild.Players.Find(_player => _player.IsLeader).Name;

        bool _amIOwner = _guild.Owner == BoomDaoUtility.Instance.UserPrincipal;
        foreach (var _player in _guild.Players)
        {
            var _display = Instantiate(guildPlayerPrefab, playersHolder);
            _display.Setup(_player, _amIOwner);
            shownObjects.Add(_display.gameObject);
        }

        GuildPlayerDisplay.OnKickPlayer += KickPlayer;
        holder.SetActive(true);
    }

    public void Close()
    {
        foreach (var _shownObject in shownObjects)
        {
            Destroy(_shownObject);
        }
        shownObjects.Clear();
        
        GuildPlayerDisplay.OnKickPlayer -= KickPlayer;
        holder.SetActive(false);
    }

    private void KickPlayer(GuildPlayerData _guildPlayer)
    {
        kickPlayer = _guildPlayer;
        GuildsPanel.Instance.ShowQuestion($"Kick {_guildPlayer.Name} ({_guildPlayer.Points})?",YesKickPlayer);
    }

    private void YesKickPlayer()
    {
        GuildsPanel.Instance.ManageInputBlocker(true);
        GuildData _guild = DataManager.Instance.PlayerData.Guild;
        List<ActionParameter> _parameters = new List<ActionParameter>();
        _parameters.Add(new () { Key = GameData.GUILD_ID, Value =  _guild.Id});
        _parameters.Add(new () { Key = "playerId", Value = kickPlayer.Principal});
        BoomDaoUtility.Instance.ExecuteActionWithParameter(KICK_PLAYER_GUILD, _parameters, HandleKickPlayer);
    }

    private void HandleKickPlayer(List<ActionOutcome> _outcomes)
    {
        GuildsPanel.Instance.ManageInputBlocker(false);
        GuildsPanel.Instance.ShowMyGuild();
    }

    private void OnEnable()
    {
        showUp.onClick.AddListener(ShowUp);
        showDown.onClick.AddListener(ShowDown);
        quit.onClick.AddListener(QuitGuild);
    }

    private void OnDisable()
    {
        showUp.onClick.RemoveListener(ShowUp);
        showDown.onClick.RemoveListener(ShowDown);
        quit.onClick.RemoveListener(QuitGuild);
    }

    private void ShowUp()
    {
        var _holderTransform = playersHolder.transform;
        
        Vector3 _itemsPosition = _holderTransform.position;
        _itemsPosition.y -= moveAmount;
        _holderTransform.position = _itemsPosition;
    }

    private void ShowDown()
    {
        var _holderTransform = playersHolder.transform;
        
        Vector3 _itemsPosition = _holderTransform.position;
        _itemsPosition.y += moveAmount;
        _holderTransform.position = _itemsPosition;
    }
    
    private void QuitGuild()
    {
        GuildData _guild = DataManager.Instance.PlayerData.Guild;
        if (_guild.IsAdmin && _guild.Players.Count>1)
        {
            GuildsPanel.Instance.ShowMessage("You can't leave the guild, there are players left in it!");
            return;
        }
        
        GuildsPanel.Instance.ShowQuestion("Are you sure that you want to leave the guild?",YesQuit);
    }

    private void YesQuit()
    {
        GuildsPanel.Instance.ManageInputBlocker(true);
        GuildData _guild = DataManager.Instance.PlayerData.Guild;
        List<ActionParameter> _parameters = new List<ActionParameter>();
        _parameters.Add(new () { Key = GameData.GUILD_ID, Value =  _guild.Id});
        deleteGuildId = _guild.Id;
        BoomDaoUtility.Instance.ExecuteActionWithParameter(LEAVE_GUILD, _parameters, _guild.IsAdmin? DeleteGuild : HandleLeaveFinished);
    }

    private void DeleteGuild(List<ActionOutcome> _outcomes)
    {
        if (_outcomes.Count==0)
        {
            GuildsPanel.Instance.ManageInputBlocker(false);
            GuildsPanel.Instance.ShowMessage("Something went wrong, please try again later");
            return;
        }
        
        List<ActionParameter> _parameters = new List<ActionParameter>();
        _parameters.Add(new () { Key = GameData.GUILD_ID, Value =  deleteGuildId});
        BoomDaoUtility.Instance.ExecuteActionWithParameter(DELETE_GUILD, _parameters, HandleDeleteGuildFinished);
    }

    private void HandleDeleteGuildFinished(List<ActionOutcome> _outcomes)
    {
        GuildsPanel.Instance.ManageInputBlocker(false);
        DataManager.Instance.PlayerData.GetMyGuild();
        
        if (DataManager.Instance.PlayerData.IsInAGuild)
        {
            GuildsPanel.Instance.ShowMessage("Failed to delete guild");
            return;
        }
        
        GuildsPanel.Instance.ShowMyGuild();
    }

    private void HandleLeaveFinished(List<ActionOutcome> _outcomes)
    {
        GuildsPanel.Instance.ManageInputBlocker(false);
        DataManager.Instance.PlayerData.GetMyGuild();
        
        if (DataManager.Instance.PlayerData.IsInAGuild)
        {
            GuildsPanel.Instance.ShowMessage("Something went wrong, please try again later");
            return;
        }
        
        GuildsPanel.Instance.ShowMyGuild();
    }
}
