using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BoomDaoWrapper;
using com.colorfulcoding.AfterGame;
using NaughtyAttributes;
using UnityEngine;

public class JavaScriptManager : MonoBehaviour
{
    public static JavaScriptManager Instance;
    
    [DllImport("__Internal")]
    public static extern void DoShareImageToTwitter(string _image, string _text);

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

    public void ShareImageToTwitter(string _image, string _text)
    {
        DoShareImageToTwitter(_image, _text);
    }

    [Button()]
    private void TestHurtKitty()
    {
        float _maxHp = 100;
        float _minutesItWillTakeToRecover = 15;
        DateTime _recoveryEnds = DateTime.UtcNow.AddMinutes(_minutesItWillTakeToRecover);
        GameState.selectedNFT.RecoveryEndDate = _recoveryEnds;
        string _hurtAction = GameState.selectedNFT.IsDefaultKitty ? AfterGameMainTitle.HURT_KITTY : AfterGameMainTitle.HURT_KITTY + 2;
        BoomDaoUtility.Instance.ExecuteActionWithParameter(_hurtAction,
            new List<ActionParameter>
            {
                new() { Key = GameData.KITTY_RECOVERY_KEY, Value = Utilities.DateTimeToNanoseconds(_recoveryEnds).ToString() },
                new() { Key = DataManager.Instance.GameData.KittyKey, Value = GameState.selectedNFT.imageUrl }
            }, null);
    }
}
