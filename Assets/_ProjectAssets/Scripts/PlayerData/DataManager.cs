using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public PlayerData PlayerData { get; private set; }

    private Action callBack;

    private bool isPushingUpdate;

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

    public void Setup(Action _callBack)
    {
        callBack = _callBack;
        FirebaseManager.Instance.GetPlayerData(FinishSetup);
    }

    private void FinishSetup(string _data)
    {
        if (string.IsNullOrEmpty(_data) || _data.ToLower() == "null")
        {
            PlayerData = new PlayerData();
        }
        else
        {
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(_data);
        }
        
        SubscribeEvents();
        callBack?.Invoke();
    }

    private void SubscribeEvents()
    {
        PlayerData.OnUpdatedGlassOfMilk += SaveData;
        PlayerData.OnUpdatedJugOfMilk += SaveData;
        PlayerData.OnUpdatedRecoverEndDate += SaveData;
        PlayerData.OnUpdatedUsername += SaveData;
        PlayerData.OnUpdatedCookies += SaveData;
    }

    private void SaveData()
    {
        if (isPushingUpdate)
        {
            return;
        }

        isPushingUpdate = true;
        StartCoroutine(PushDataRoutine());
    }

    private IEnumerator PushDataRoutine()
    {
        yield return new WaitForSeconds(1);
        isPushingUpdate = false;
        FirebaseManager.Instance.SavePlayerData(JsonConvert.SerializeObject(PlayerData));
    }
}
