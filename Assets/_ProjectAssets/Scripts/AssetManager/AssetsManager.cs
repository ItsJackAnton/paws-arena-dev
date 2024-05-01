using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    public static AssetsManager Instance;
    [SerializeField] private List<ItemSprite> items;
    [SerializeField] private List<ItemSprite> rewardsForChallenges;
    
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
}