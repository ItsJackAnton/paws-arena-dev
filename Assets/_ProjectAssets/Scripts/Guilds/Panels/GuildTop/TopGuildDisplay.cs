using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopGuildDisplay : MonoBehaviour
{
    [SerializeField] private Image badgeDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Image kittyDisplay;
    [SerializeField] private TextMeshProUGUI amountOfMembersDisplay;
    [SerializeField] private TextMeshProUGUI sumOfPoints;
    [SerializeField] private Button joinButton;

    private GuildData guildData;
    
    public void Setup(GuildData _guildData)
    {
        guildData = _guildData;
        badgeDisplay.sprite = AssetsManager.Instance.GetChallengeBadgeSprite(_guildData.BadgeName);
        nameDisplay.text = guildData.Name;
        kittyDisplay.sprite = AssetsManager.Instance.GetChallengeKingdomSprite(_guildData.KingdomName);
        amountOfMembersDisplay.text = guildData.Players.Count.ToString();
        sumOfPoints.text = guildData.SumOfPoints.ToString();
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
        GuildsPanel.Instance.ShowJoinGuild(guildData);
    }
}
