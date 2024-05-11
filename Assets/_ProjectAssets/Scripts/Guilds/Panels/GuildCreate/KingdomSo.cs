using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKingdom", menuName = "ScriptableObjects/Guilds/Kingdom")]
public class KingdomSo : ScriptableObject
{
    [field: SerializeField] public Sprite Normal { get; private set; }
    [field: SerializeField]public Sprite Selected{ get; private set; }

    private static List<KingdomSo> allKingdoms;
    
    public static KingdomSo Get(string _key)
    {
        LoadAll();
        return allKingdoms.Find(_kingdom => _kingdom.Normal.name == _key);
    }
    
    [ItemCanBeNull]
    public static List<KingdomSo>GetAll()
    {
        LoadAll();
        return allKingdoms;
    }

    private static void LoadAll()
    {
        if (allKingdoms!=null)
        {
            return;
        }
        allKingdoms = Resources.LoadAll<KingdomSo>("Kingdoms").ToList();
    }
}
