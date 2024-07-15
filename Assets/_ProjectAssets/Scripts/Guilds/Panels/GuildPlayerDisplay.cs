using System;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildPlayerDisplay : MonoBehaviour
{
    public static Action<GuildPlayerData> OnKickPlayer;
    
    [SerializeField] private TextMeshProUGUI placeDisplay;
    [SerializeField] private GameObject leaderIcon;
    [SerializeField] private TextMeshProUGUI levelDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI pointsDisplay;
    [SerializeField] private Button kickButton;
    private GuildPlayerData playerData;

    
    public void Setup(GuildPlayerData _playerData, bool _showKickButton, int _pointsDisplay)
    {
        playerData = _playerData;
        if (BoomDaoUtility.Instance.UserPrincipal==_playerData.Principal)
        {
            _showKickButton = false;
        }
        placeDisplay.text = _playerData.Place + ".";
        leaderIcon.SetActive(_playerData.IsLeader);
        levelDisplay.text = _playerData.Level.ToString();
        nameDisplay.text = _playerData.Name;
        pointsDisplay.text = _pointsDisplay.ToString();
        kickButton.gameObject.SetActive(_showKickButton);
    }

    private void OnEnable()
    {
        kickButton.onClick.AddListener(KickPlayer);
    }

    private void OnDisable()
    {
        kickButton.onClick.RemoveListener(KickPlayer);
    }

    private void KickPlayer()
    {
        OnKickPlayer?.Invoke(playerData);
    }
}
