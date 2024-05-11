using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildTop : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private TopGuildDisplay topGuildPrefab;
    [SerializeField] private Transform goldRankHolder;
    [SerializeField] private Transform silverRankHolder;
    [SerializeField] private Transform bronzeRankHolder;
    [SerializeField] private Transform restOfGuildsHolder;
    [SerializeField] private RectTransform[] layoutGroups;

    [SerializeField] private Button goldDetails;
    [SerializeField] private Button silverDetails;
    [SerializeField] private Button bronzeDetails;

    private List<GameObject> shownGuilds = new ();

    private void OnEnable()
    {
        goldDetails.onClick.AddListener(ShowGoldDetails);
        silverDetails.onClick.AddListener(ShowSilverDetails);
        bronzeDetails.onClick.AddListener(ShowBronzeDetails);
    }

    private void OnDisable()
    {
        goldDetails.onClick.RemoveListener(ShowGoldDetails);
        silverDetails.onClick.RemoveListener(ShowSilverDetails);
        bronzeDetails.onClick.RemoveListener(ShowBronzeDetails);
    }

    private void ShowGoldDetails()
    {
        GuildsPanel.Instance.ShowMessage($"Reach {DataManager.Instance.GameData.GuildGoldBar} points to reach golden rank!");
    }

    private void ShowSilverDetails()
    {
        GuildsPanel.Instance.ShowMessage($"Reach {DataManager.Instance.GameData.GuildSilverBar} points to reach silver rank!");
    }

    private void ShowBronzeDetails()
    {
        GuildsPanel.Instance.ShowMessage($"Reach {DataManager.Instance.GameData.GuildBronzeBar} points to reach bronze rank!");
    }

    public void Setup()
    {
        ClearShownGuilds();
        ShowGuilds();
        holder.SetActive(true);
        
        StartCoroutine(RebuildLayoutGroups());

        IEnumerator RebuildLayoutGroups()
        {
            foreach (var _layoutGroup in layoutGroups)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup);
                yield return null;
            }
        }
    }
    
    public void Close()
    {
        holder.SetActive(false);
    }

    private void ClearShownGuilds()
    {
        foreach (var _shownGuild in shownGuilds)
        {
            Destroy(_shownGuild);
        }
        
        shownGuilds.Clear();
    }

    private void ShowGuilds()
    {
        List<GuildData> _goldGuilds = new();
        List<GuildData> _silverGuilds = new();
        List<GuildData> _bronzeGuilds = new();
        List<GuildData> _restOfGuilds = new();

        foreach (var _guild in DataManager.Instance.GameData.Guilds)
        {
            if (_guild.SumOfPoints >= DataManager.Instance.GameData.GuildGoldBar)
            {
                _goldGuilds.Add(_guild);
            }
            else if (_guild.SumOfPoints >= DataManager.Instance.GameData.GuildSilverBar)
            {
                _silverGuilds.Add(_guild);
            }
            else if (_guild.SumOfPoints >= DataManager.Instance.GameData.GuildBronzeBar)
            {
                _bronzeGuilds.Add(_guild);
            }
            else
            {
                _restOfGuilds.Add(_guild);
            }
        }

        DisplayGuilds(_goldGuilds,goldRankHolder);
        DisplayGuilds(_silverGuilds,silverRankHolder);
        DisplayGuilds(_bronzeGuilds,bronzeRankHolder);
        DisplayGuilds(_restOfGuilds,restOfGuildsHolder);

        void DisplayGuilds(List<GuildData> _guilds, Transform _holder)
        {
            foreach (var _guild in _guilds)
            {
                TopGuildDisplay _guildDisplay = Instantiate(topGuildPrefab, _holder);
                _guildDisplay.Setup(_guild);
                shownGuilds.Add(_guildDisplay.gameObject);
            }
        }
    }
}