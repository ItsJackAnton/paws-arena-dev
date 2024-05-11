using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildBadgeSelection : MonoBehaviour
{
    [SerializeField] private Image badgeDisplay;
    [SerializeField] private Button showNext;
    [SerializeField] private Button showPrevious;
    private List<Sprite> badges;
    private int badgeIndex;

    public string SelectedBadgeName => badges[badgeIndex].name;

    private void OnEnable()
    {
        showNext.onClick.AddListener(ShowNext);
        showPrevious.onClick.AddListener(ShowPrevious);
        
        Setup();
    }

    private void OnDisable()
    {
        showNext.onClick.RemoveListener(ShowNext);
        showPrevious.onClick.RemoveListener(ShowPrevious);
    }

    private void ShowNext()
    {
        badgeIndex++;
        if (badgeIndex>badges.Count-1)
        {
            badgeIndex = badges.Count - 1;
        }
        
        ShowBadge();
    }

    private void ShowPrevious()
    {
        badgeIndex--;
        if (badgeIndex<0)
        {
            badgeIndex = 0;
        }
        
        ShowBadge();
    }

    private void Setup()
    {
        badges = AssetsManager.Instance.GetChallengeBadgeSprites();
        badgeIndex = 0;
    }

    private void ShowBadge()
    {
        badgeDisplay.sprite = badges[badgeIndex];
    }
}
