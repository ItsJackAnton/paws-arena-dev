using System.Collections;
using UnityEngine;

public class GameMatchingScreenSinglePlayer : GameMatchingScreen
{
    
    protected override void OnDisable()
    {
    }

    protected override void OnEnable()
    {
        Init();
    }
    
    protected override void Init()
    {
        notices.SetActive(false);

        SetSeats();
        OccupySeat(seats[0], DataManager.Instance.PlayerData.Username);
        searchingForOpponent.SetActive(true);

        StartCoroutine(BringBotAfterSeconds(3));
    }
    
    protected override IEnumerator BringBotAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        BringBot();
    }
    
    public override void BringBot()
    {
        BotInformation botInformation = GetRandomBot();
        GameState.botInfo = botInformation;
        OccupySeat(seats[1], botInformation.nickname);
        syncPlatformsBehaviour.InstantiateBot();
        searchingForOpponent.SetActive(false);
        StartCountdown(SceneManager.SINGLE_PLAYER_GAME);
    }
    
    
    protected override void StartCountdown(string _sceneName)
    {
        if (wheelHolder)
        {
            wheelHolder.SetActive(true);
        }
        
        countdown.StartCountDown(() =>
        {
            SceneManager.Instance.LoadScene(_sceneName);
        });
    }
    
    public override void TryExitRoom()
    {
        SceneManager.Instance.LoadMainMenu();
    }
    
    public override void SetSeats()
    {
        foreach (SeatGameobject seat in seats)
        {
            FreeSeat(seat);
        }

        OccupySeat(seats[0], DataManager.Instance.PlayerData.Username);
    }
}
