using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;

public static class Utilities
{
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string _text);
    
    private static readonly DateTime UNIX_EPOCH = new (2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long DateTimeToNanoseconds(DateTime _dateTime)
    {
        TimeSpan _duration = _dateTime.ToUniversalTime() - UNIX_EPOCH;
        long _ticks = _duration.Ticks;
        long _nanoseconds = _ticks * 100;
        return _nanoseconds;
    }

    public static DateTime NanosecondsToDateTime(double _nanoseconds)
    {
        return NanosecondsToDateTime((long)_nanoseconds);
    }
    
    public static DateTime NanosecondsToDateTime(long _nanoseconds)
    {
        long _ticks = _nanoseconds / 100; 
        DateTime _dateTime = UNIX_EPOCH.AddTicks(_ticks);
        return _dateTime;
    }
    
    public static string RemoveWhitespacesUsingRegex(string _source)
    {
        return Regex.Replace(_source, @"\s", string.Empty);
    }
    
    public static ItemType GetRewardType(string _key)
    {
        switch (_key)
        {
            case PlayerData.COMMON_SHARD:
                return ItemType.CommonShard;
            case PlayerData.UNCOMMON_SHARD:
                return ItemType.UncommonShard;
            case PlayerData.RARE_SHARD:
                return ItemType.RareShard;
            case PlayerData.EPIC_SHARD:
                return ItemType.EpicShard;
            case PlayerData.LEGENDARY_SHARD:
                return ItemType.LegendaryShard;
            case PlayerData.MILK_GLASS:
                return ItemType.GlassOfMilk;
            case PlayerData.MILK_BOTTLE:
                return ItemType.JugOfMilk;
            default:
                throw new Exception($"Don't know how to reward {_key}");
        }
    }

    public static string GetItemKey(ItemType _type)
    {
        switch (_type)
        {
            case ItemType.CommonShard:
                return PlayerData.COMMON_SHARD;
            case ItemType.UncommonShard:
                return PlayerData.UNCOMMON_SHARD;
            case ItemType.RareShard:
                return PlayerData.RARE_SHARD;
            case ItemType.EpicShard:
                return PlayerData.EPIC_SHARD;
            case ItemType.LegendaryShard:
                return PlayerData.LEGENDARY_SHARD;
            case ItemType.Snack:
                return PlayerData.SNACKS;
            case ItemType.JugOfMilk:
                return PlayerData.MILK_BOTTLE;
            case ItemType.GlassOfMilk:
                return PlayerData.MILK_GLASS;
            default:
                throw new Exception($"Dont know how to convert {_type} to a key");
        }
    }

    public static CraftingRecepieSO EquipmentRarityToCraftingRecipe(EquipmentRarity _rarity)
    {
        switch (_rarity)
        {
            case EquipmentRarity.Common:
                return CraftingRecepieSO.Get(ItemType.CommonShard);
            case EquipmentRarity.Uncommon:
                return CraftingRecepieSO.Get(ItemType.UncommonShard);
            case EquipmentRarity.Rare:
                return CraftingRecepieSO.Get(ItemType.RareShard);
            case EquipmentRarity.Epic:
                return CraftingRecepieSO.Get(ItemType.EpicShard);
            case EquipmentRarity.Legendary:
                return CraftingRecepieSO.Get(ItemType.LegendaryShard);
            default:
                throw new Exception($"Don't know how to convert {_rarity} to recepie");
        }
    }

    public static string GetItemName(ItemType _type)
    {
        switch (_type)
        {
            case ItemType.CommonShard:
                return "Common Crystal";
            case ItemType.UncommonShard:
                return "Uncommon Crystal";
            case ItemType.RareShard:
                return "Rare Crystal";
            case ItemType.EpicShard:
                return "Epic Crystal";
            case ItemType.LegendaryShard:
                return "Legendary Crystal";
            case ItemType.Snack:
                return "Snack";
            case ItemType.JugOfMilk:
                return "Bottle of milk";
            case ItemType.GlassOfMilk:
                return "Glass of milk";
            case ItemType.Item:
                return "Item";
            case ItemType.Emote:
                return "Emote";
            case ItemType.WeaponSkin:
                return "Weapon skin";
            case ItemType.SeasonExperience:
                return "Season experience";
            default:
                throw new Exception($"Dont know how to convert {_type} to a name");
        }
    }

    public static string GetCrystalTypeString(ItemType _type)
    {
        switch (_type)
        {
            case ItemType.CommonShard:
                return "Common";
            case ItemType.UncommonShard:
                return "Uncommon";
            case ItemType.RareShard:
                return "Rare";
            case ItemType.EpicShard:
                return "Epic";
            case ItemType.LegendaryShard:
                return "Legendary";
            default:
                throw new Exception($"Dont know how to convert {_type} to a name");
        }
    }

    public static string UpperFirstLetter(this string _string)
    {
        string _newString = string.Empty;
        for (int _i = 0; _i < _string.Length; _i++)
        {
            char _char = _string[_i];
            if (_i==0)
            {
                _newString += char.ToUpper(_char);
            }
            else
            {
                _newString += _char;
            }
        }

        return _newString;
    }

    public static void DoCopyToClipboard(string _string)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CopyToClipboard(_string);
        }
        else
        {
            GUIUtility.systemCopyBuffer = _string;
        }
    }

    public static string FindEntityValue(this List<ConfigData> _this, string _key)
    {
        foreach (var _entity in _this)
        {
            if (_entity.Name==_key)
            {
                return _entity.Value;
            }
        }

        return string.Empty;
    }

    public static string ReplaceBackslashN(this string _this)
    {
        return _this.Replace("\\n","\n");
    }

    public static string TimeSpanToTimer(TimeSpan _timeLeft)
    {
        string _output = string.Empty;
        if (_timeLeft.Days > 0) {
            _output += _timeLeft.Days + "d ";
        }
        _output += _timeLeft.Hours < 10 ? "0" + _timeLeft.Hours : _timeLeft.Hours.ToString();
        _output += "h ";
        _output += _timeLeft.Minutes < 10 ? "0" + _timeLeft.Minutes : _timeLeft.Minutes.ToString();
        _output += "m ";
        _output += _timeLeft.Seconds < 10 ? "0" + _timeLeft.Seconds : _timeLeft.Seconds.ToString();
        _output += "s ";
        return _output;
    }

    public static Sprite ConvertTextureToProfileSprite(Texture2D _texture,Vector2Int _startCoordinates,int _squareSize)
    {
        if (_startCoordinates.x < 0 || _startCoordinates.y < 0 || 
            _startCoordinates.x + _squareSize > _texture.width || 
            _startCoordinates.y + _squareSize > _texture.height)
        {
            Debug.LogError("Requested area is out of the texture bounds.");
            return null;
        }

        Rect _rect = new Rect(_startCoordinates.x, _startCoordinates.y, _squareSize, _squareSize);
        Vector2 _pivot = new Vector2(0.5f, 0.5f); // Pivot at the center of the square

        Sprite _sprite = Sprite.Create(_texture, _rect, _pivot);

        return _sprite;
    }
    
    public static Sprite TextureToSprite(Texture2D _texture)
    {
        return Sprite.Create(
            _texture,
            new Rect(0, 0, _texture.width, _texture.height), 
            new Vector2(0.5f, 0.5f)
        );
    }
}
