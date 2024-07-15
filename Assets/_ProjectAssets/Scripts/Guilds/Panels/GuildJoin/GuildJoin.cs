using System.Collections.Generic;
using BoomDaoWrapper;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildJoin : MonoBehaviour
{
    private const string JOIN_GUILD = "joinGuild";
    [SerializeField] private GameObject holder;
    [SerializeField] private Button join;
    [SerializeField] private Button cancel;
    
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

    private float moveAmount=1;
    private GuildData guildData;
    private List<GameObject> shownObjects = new();


    public void Setup(GuildData _guildData)
    {
        guildData = _guildData;
        badgeDisplay.sprite = AssetsManager.Instance.GetChallengeBadgeSprite(_guildData.BadgeName);
        kingdomDisplay.sprite = AssetsManager.Instance.GetChallengeKingdomSprite(_guildData.KingdomName);
        nameDisplay.text = _guildData.Name;
        membersDisplay.text = $"Members: {_guildData.Players.Count}/{DataManager.Instance.GameData.MaxGuildPlayers}";
        battlesWonDisplay.text = "Battles won: " + _guildData.BattlesWon;
        leaderDisplay.text = _guildData.Players.Find(_player => _player.IsLeader).Name;

        foreach (var _player in _guildData.Players)
        {
            var _display = Instantiate(guildPlayerPrefab, playersHolder);
            _display.Setup(_player, false, _player.Points);
            shownObjects.Add(_display.gameObject);
        }

        holder.SetActive(true);
    }

    public void Close()
    {
        foreach (var _shownObject in shownObjects)
        {
            Destroy(_shownObject);
        }
        shownObjects.Clear();
        
        holder.SetActive(false);
    }

    private void OnEnable()
    {
        join.onClick.AddListener(Join);
        cancel.onClick.AddListener(Cancel);
        showUp.onClick.AddListener(ShowUp);
        showDown.onClick.AddListener(ShowDown);
    }

    private void OnDisable()
    {
        join.onClick.RemoveListener(Join);
        cancel.onClick.RemoveListener(Cancel);
        showUp.onClick.RemoveListener(ShowUp);
        showDown.onClick.RemoveListener(ShowDown);
    }

    private void Join()
    {
        if (DataManager.Instance.PlayerData.IsInAGuild)
        {
            GuildsPanel.Instance.ShowMessage("You are already in a guild");
            return;
        }
        
        if (DataManager.Instance.PlayerData.LeaderboardPoints<guildData.PointsRequirement)
        {
            GuildsPanel.Instance.ShowMessage("You don't have enough points");
            return;
        }

        if (guildData.IsFull)
        {
            GuildsPanel.Instance.ShowMessage("This guild is full");
            return;
        }
        
        GuildsPanel.Instance.ShowQuestion($"Are you sure that you want to join {guildData.Name}?",YesJoin);
    }

    private void YesJoin()
    {
        GuildsPanel.Instance.ManageInputBlocker(true);
        List<ActionParameter> _parameters = new List<ActionParameter>();
        _parameters.Add(new () { Key = GameData.GUILD_ID, Value = guildData.Id });
        BoomDaoUtility.Instance.ExecuteActionWithParameter(JOIN_GUILD, _parameters, HandleJoinGuildFinished);
    }

    private void HandleJoinGuildFinished(List<ActionOutcome> _outcomes)
    {
        GuildsPanel.Instance.ManageInputBlocker(false);
        DataManager.Instance.PlayerData.GetMyGuild();
        if (DataManager.Instance.PlayerData.IsInAGuild)
        {
            DataManager.Instance.PlayerData.GuildId = guildData.Id;
            GuildsPanel.Instance.ShowMyGuild();
            return;
        }

        GuildsPanel.Instance.ShowMessage("Something went wrong, please try again later");
    }

    private void Cancel()
    {
        GuildsPanel.Instance.ShowMyGuild();
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
}
