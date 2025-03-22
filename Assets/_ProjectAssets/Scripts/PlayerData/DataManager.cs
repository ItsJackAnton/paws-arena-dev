using Newtonsoft.Json;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string PLAYER_JSON_KEY = "playerDataJson";
    public static DataManager Instance;

    public PlayerData PlayerData { get; private set; }

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

    public void Setup()
    {
        if (JavaScriptManager.UseMockUpData)
        {
            if (PlayerPrefs.HasKey(PLAYER_JSON_KEY))
            {
                PlayerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(PLAYER_JSON_KEY));
            }
            else
            {
                PlayerData = new PlayerData { Username = "Unity Editor", RecoveryEndDate = default, JugOfMilk = 100, GlassOfMilk = 100};
            }

            SubscribeEvents();
        }
        else
        {
            //todo handle Abstract
        }
    }

    private void SubscribeEvents()
    {
        if (JavaScriptManager.UseMockUpData)
        {
            PlayerData.OnUpdatedGlassOfMilk += SaveJsonInPlayerPrefs;
            PlayerData.OnUpdatedJugOfMilk += SaveJsonInPlayerPrefs;
            PlayerData.OnUpdatedRecoverEndDate += SaveJsonInPlayerPrefs;
            PlayerData.OnUpdatedUsername += SaveJsonInPlayerPrefs;
        }
        else
        {
            //todo fix me for Abstract
        }
    }

    private void SaveJsonInPlayerPrefs()
    {
        PlayerPrefs.SetString(PLAYER_JSON_KEY, JsonConvert.SerializeObject(PlayerData));
    }
}
