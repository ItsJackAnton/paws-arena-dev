using System;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public static Action OnUpdatedUsername;
    public static Action OnUpdatedRecoverEndDate;
    public static Action OnUpdatedJugOfMilk;
    public static Action OnUpdatedGlassOfMilk;
    public static Action OnUpdatedCookies;
    
    private string username = string.Empty;
    private DateTime recoveryEndDate;
    private int glassOfMilk;
    private int jugOfMilk;
    private int cookies;
    
    [JsonIgnore]public bool CanFight => RecoveryEndDate < DateTime.UtcNow;
    
    [JsonIgnore]public int MinutesUntilHealed => (int)(RecoveryEndDate - DateTime.UtcNow).TotalMinutes;
    [JsonIgnore]public TimeSpan TimeUntilHealed => RecoveryEndDate - DateTime.UtcNow;

    public string Username
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
            OnUpdatedUsername?.Invoke();
        }
    }

    public DateTime RecoveryEndDate
    {
        get
        {
            return recoveryEndDate;
        }

        set
        {
            recoveryEndDate = value;
            OnUpdatedRecoverEndDate?.Invoke();
        }
    }

    public int GlassOfMilk
    {
        get
        {
            return glassOfMilk;
        }
        set
        {
            glassOfMilk = value;
            OnUpdatedGlassOfMilk?.Invoke();
        }
    }

    public int JugOfMilk
    {
        get
        {
            return jugOfMilk;
        }
        set
        {
            jugOfMilk = value;
            OnUpdatedJugOfMilk?.Invoke();
        }
    }

    [JsonIgnore] public NFT Nft;

    public int Cookies
    {
        get
        {
            return cookies;
        }
        set
        {
            cookies = value;
            OnUpdatedCookies?.Invoke();
        }
    }
}