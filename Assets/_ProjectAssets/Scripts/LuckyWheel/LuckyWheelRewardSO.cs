using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWheelReward", menuName = "ScriptableObjects/WheelReward")]
public class LuckyWheelRewardSO : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public ItemType Type { get; private set; }
    [field: SerializeField] public float Amount { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Key { get; private set; }
    [field: SerializeField] public float MinRotation { get; private set; }
    [field: SerializeField] public float MaxRotation { get; private set; }

    private static List<LuckyWheelRewardSO> allRewards;

    public static List<LuckyWheelRewardSO> GetAll()
    {
        LoadAllRewards();
        return allRewards.ToList();
    }

    public static LuckyWheelRewardSO Get(int _id)
    {
        LoadAllRewards();
        return allRewards.First(element => element.Id == _id);
    }
    
    public static LuckyWheelRewardSO Get(string _key)
    {
        LoadAllRewards();
        return allRewards.First(element => 
            Utilities.RemoveWhitespacesUsingRegex(element.Key).ToLower() == Utilities.RemoveWhitespacesUsingRegex
        (_key).ToLower());
    }
    private static void LoadAllRewards()
    {
        if (allRewards != null)
        {
            return;
        }

        allRewards = Resources.LoadAll<LuckyWheelRewardSO>("WheelRewards/").ToList();
    }
}
