using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildSearchResultDisplay : MonoBehaviour
{
    public static Action<GuildData> OnJoinGuild;
    
    [SerializeField] private Image badgeDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Image kingdomDisplay;
    [SerializeField] private TextMeshProUGUI amountOfMembersDisplay;
    [SerializeField] private TextMeshProUGUI minPoints;
    [SerializeField] private Button joinButton;

    private GuildData guildData;
    
    public void Setup(GuildData _guildData)
    {
        guildData = _guildData;
        badgeDisplay.sprite = AssetsManager.Instance.GetChallengeBadgeSprite(_guildData.BadgeName);
        nameDisplay.text = guildData.Name;
        kingdomDisplay.sprite = AssetsManager.Instance.GetChallengeKingdomSprite(_guildData.KingdomName);
        amountOfMembersDisplay.text = guildData.Players.Count.ToString();
        minPoints.text = _guildData.PointsRequirement.ToString();
    }

    private void OnEnable()
    {
        joinButton.onClick.AddListener(JoinGuild);
    }

    private void OnDisable()
    {
        joinButton.onClick.RemoveListener(JoinGuild);
    }

    private void JoinGuild()
    {
        OnJoinGuild?.Invoke(guildData);
    }
}
