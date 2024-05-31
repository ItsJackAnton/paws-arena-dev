using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildBattlesPanel : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject battleHolder;
    [SerializeField] private GameObject peaceHolder;

    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private GuildBattleGuildDisplay myGuild;
    [SerializeField] private GuildBattleGuildDisplay opponentGuild;
    [SerializeField] private GuildBattlesHistory history;

    [SerializeField] private GuildPlayerDisplay guildPlayerPrefab;
    [SerializeField] private Transform playersHolder;
    [SerializeField] private Button showUp;
    [SerializeField] private Button showDown;

    private List<GameObject> shownObjects = new ();
    private float moveAmount=1;

    public void Setup()
    {
        holder.SetActive(true);
        battleHolder.SetActive(false);
        peaceHolder.SetActive(false);
        if (!DataManager.Instance.GameData.GuildBattles.IsActive)
        {
            peaceHolder.SetActive(true);
            return;
        }
        
        battleHolder.SetActive(true);
        var _myGuild = DataManager.Instance.PlayerData.Guild;
        var _opponentGuild = DataManager.Instance.PlayerData.Guild.GuildBattle.Opponent;
        myGuild.Setup(_myGuild.Badge, _myGuild.Name, _myGuild.GuildBattle.Points.ToString(), AssetsManager.Instance.GetKingdomColor(_myGuild.Kingdom));
        opponentGuild.Setup(_opponentGuild.Badge, _opponentGuild.Name, _opponentGuild.GuildBattle.Points.ToString(), AssetsManager.Instance.GetKingdomColor(_opponentGuild
        .Kingdom));
        history.Setup(DataManager.Instance.PlayerData.Guild.BattlesHistory);
        foreach (var _entry in _myGuild.GuildBattle.BattleEntries)
        {
            GuildPlayerDisplay _display = Instantiate(guildPlayerPrefab, playersHolder);
            GuildPlayerData _playerData =
                JsonConvert.DeserializeObject<GuildPlayerData>(JsonConvert.SerializeObject(_myGuild.Players.Find(_player => _player.Principal == _entry
                .Principal)));
            _display.Setup(_playerData, false);
            shownObjects.Add(_display.gameObject);
        }
        StartCoroutine(ShowTimer());
    }

    private IEnumerator ShowTimer()
    {
        while (battleHolder.activeSelf)
        {
            TimeSpan _endTime = DataManager.Instance.GameData.GuildBattles.EndingDate - DataManager.Instance.GameData.GuildBattles.StartingDate;
            string _output = "Ends in\n" + Utilities.TimeSpanToTimer(_endTime);
            if (_endTime.TotalSeconds<0)
            {
                Setup();
                yield break;
            }

            timerDisplay.text = _output;
            yield return new WaitForSeconds(1);
        }
    }

    public void Close()
    {
        holder.SetActive(false);
        
        foreach (var _shownObject in shownObjects)
        {
            Destroy(_shownObject);
        }
    }
    
    private void OnEnable()
    {
        showUp.onClick.AddListener(ShowUp);
        showDown.onClick.AddListener(ShowDown);
    }

    private void OnDisable()
    {
        showUp.onClick.RemoveListener(ShowUp);
        showDown.onClick.RemoveListener(ShowDown);
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
