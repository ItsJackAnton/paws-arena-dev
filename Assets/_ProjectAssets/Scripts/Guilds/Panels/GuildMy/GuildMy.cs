using System.Collections.Generic;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMy : MonoBehaviour
{
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

    public void Setup()
    {
        GuildData _guild = DataManager.Instance.PlayerData.Guild;
        badgeDisplay.sprite = AssetsManager.Instance.GetChallengeBadgeSprite(_guild.BadgeName);
        kingdomDisplay.sprite = AssetsManager.Instance.GetChallengeKingdomSprite(_guild.KingdomName);
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
        //todo Kick player
        GuildsPanel.Instance.ShowMessage("Kick player option is coming soon");
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
        //todo leave guild
        GuildsPanel.Instance.ShowMessage("Leave guild is coming soon");
    }
}
