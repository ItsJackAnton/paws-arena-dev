using System;
using System.Collections.Generic;
using BoomDaoWrapper;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildCreate : MonoBehaviour
{
    private const string CREATE_GUILD = "createGuild";
    private const string PLAYER_DATA = "playerData";
    
    [SerializeField] private GameObject holder;
    [SerializeField] private GuildBadgeSelection badgeSelection;
    [SerializeField] private GuildKingdomSelection kingdomSelection;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField pointsRequirementInput;
    [SerializeField] private Button create;

    public void Setup()
    {
        nameInput.text = string.Empty;
        pointsRequirementInput.text = string.Empty;
        holder.SetActive(true);
        kingdomSelection.SelectDefault();
    }

    public void Close()
    {
        holder.SetActive(false);
    }

    private void OnEnable()
    {
        create.onClick.AddListener(CreateGuild);
    }

    private void OnDisable()
    {
        create.onClick.RemoveListener(CreateGuild);
    }

    private void CreateGuild()
    {
        GuildsPanel.Instance.ShowQuestion($"Create new guild for {DataManager.Instance.GameData.GuildPriceAsString} ICP?", YesCreate);
    }

    private void YesCreate()
    {
        if (!IsInputValid())
        {
            return;
        }

        string _guildName = nameInput.text;
        string _kingdomName = kingdomSelection.SelectedKingdomName;
        string _badgeName = badgeSelection.SelectedBadgeName;
        int _pointsRequired = string.IsNullOrEmpty(pointsRequirementInput.text) ? 0 : Convert.ToInt32(pointsRequirementInput.text);
        string _id = Guid.NewGuid().ToString();


        GuildsPanel.Instance.ManageInputBlocker(true);
        var _parameters = new List<ActionParameter>
        {
            new() { Key = GameData.GUILD_ID, Value = _id }, new() { Key = GameData.GUILD_NAME, Value = _guildName },
            new () { Key = GameData.GUILD_KINGDOM_NAME, Value = _kingdomName },
            new () { Key = GameData.GUILD_BADGE_NAME, Value = _badgeName },
            new () { Key = GameData.GUILD_OWNER, Value = BoomDaoUtility.Instance.UserPrincipal },
            new () { Key = PLAYER_DATA, Value = BoomDaoUtility.Instance.UserPrincipal },
            new () { Key = GameData.GUILD_POINTS_REQUIREMENT, Value = _pointsRequired.ToString() }
        };

        BoomDaoUtility.Instance.ExecuteActionWithParameter(CREATE_GUILD, _parameters, HandleFinishedCreation);
    }

    private bool IsInputValid()
    {
        if (BoomDaoUtility.Instance.GetTokenBalance(BoomDaoUtility.ICP_KEY) < DataManager.Instance.GameData.GuildPrice)
        {
            GuildsPanel.Instance.ShowMessage($"You dont have enough icp! You need {DataManager.Instance.GameData.GuildPrice} to create a Guild");
            return false;
        }

        string _name = nameInput.text;

        int _minLenght = 3;
        int _maxLenght=15;
        if (_name.Length<_minLenght||_name.Length>_maxLenght)
        {
            GuildsPanel.Instance.ShowMessage($"Name must contain minimum {_minLenght} characters and maximum {_maxLenght}");
            return false;
        }

        return true;
    }
    
    private void HandleFinishedCreation(List<ActionOutcome> _outcomes)
    {
        GuildsPanel.Instance.ManageInputBlocker(false);
        if (DataManager.Instance.PlayerData.IsInAGuild)
        {
            GuildsPanel.Instance.ShowMyGuild();
            return;
        }
        
        GuildsPanel.Instance.ShowMessage("Something went wrong, please try again later");
    }
}
