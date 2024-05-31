using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    public static AssetsManager Instance;
    [SerializeField] private List<ItemSprite> items;
    [SerializeField] private List<ItemSprite> rewardsForChallenges;
    [SerializeField] private List<Sprite> guildKingdoms;
    [SerializeField] private List<Sprite> guildBadge;
    [SerializeField] private List<ItemColor> kingdomColors;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetChallengeReward(ItemType _type)
    {
        var _itemSprite = rewardsForChallenges.Find(_item => _item.Type == _type);
        if (_itemSprite==null)
        {
            throw new Exception("Not found: " + _type.ToString());
        }
        return _itemSprite.Sprite;
    }

    public Sprite GetChallengeKingdomSprite(string _kingdomName)
    {
        var _itemSprite = guildKingdoms.Find(_item => _item.name == _kingdomName);
        if (_itemSprite==null)
        {
            throw new Exception("Not found: " + _kingdomName);
        }
        return _itemSprite;
    }

    public List<Sprite> GetChallengeBadgeSprites()
    {
        return guildBadge;
    }

    public Sprite GetChallengeBadgeSprite(string _badgeName)
    {
        var _itemSprite = guildBadge.Find(_item => _item.name == _badgeName);
        if (_itemSprite==null)
        {
            throw new Exception("Not found: " + _badgeName);
        }
        return _itemSprite;
    }

    public Color GetKingdomColor(Sprite _kingdom)
    {
        return kingdomColors.Find(_kingdomSprite => _kingdomSprite.Sprite == _kingdom).Color;
    }
}