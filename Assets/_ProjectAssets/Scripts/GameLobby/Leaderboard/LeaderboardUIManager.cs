using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoomDaoWrapper;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIManager : MonoBehaviour
{
    [Header("UI Components")]
    public Transform leaderboardContent;
    public GameObject leaderboardLinePrefab;
    [SerializeField] private Button reload;

    [Header("Places")]
    public TextMeshProUGUI firstPlacePoints;
    public TextMeshProUGUI secondPlacePoints;
    public TextMeshProUGUI thirdPlacePoints;

    public PlayerPlatformBehaviour firstPlayer;
    public PlayerPlatformBehaviour secondPlayer;
    public PlayerPlatformBehaviour thirdPlayer;

    [SerializeField] private Button showContest;
    [SerializeField] private Button showLeaderboard;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject contestNotStartedHolder;
    [SerializeField] private TextMeshProUGUI contestNotStartedText;
    private LeaderboardData data;
    public List<Sprite> stars;
    private static bool shouldShowContest = true;

    private void OnEnable()
    {
        reload.onClick.AddListener(RequestReload);
        BoomDaoUtility.OnUpdatedWorldData += ReloadScene;
        showContest.onClick.AddListener(PrepareContest);
        showLeaderboard.onClick.AddListener(PrepareLeaderboard);
    }

    private void OnDisable()
    {
        reload.onClick.RemoveListener(RequestReload);
        BoomDaoUtility.OnUpdatedWorldData -= ReloadScene;
        showContest.onClick.RemoveListener(PrepareContest);
        showLeaderboard.onClick.RemoveListener(PrepareLeaderboard);
        StopAllCoroutines();
    }

    private void PrepareContest()
    {
        shouldShowContest = true;
        SceneManager.Instance.Reload();
    }

    private void PrepareLeaderboard()
    {
        shouldShowContest = false;
        SceneManager.Instance.Reload();
    }

    private void RequestReload()
    {
        BoomDaoUtility.Instance.ReloadWorldData();
    }
    
    private void ReloadScene()
    {
        SceneManager.Instance.Reload();
    }

    private void Start()
    {
        LeaderboardData _data = DataManager.Instance.GameData.GetLeaderboard;
        data = _data;
        if (shouldShowContest)
        {
            ShowContest();
        }
        else
        {
            ShowLeaderboard();
        }   
    }

  
    private void ShowContest()
    {
        showContest.interactable = false;
        showLeaderboard.interactable = true;
        DateTime _currentDate = DateTime.UtcNow;
        Debug.Log("Current date: "+_currentDate);
        Debug.Log("Contest end: "+DataManager.Instance.GameData.ContestEnd);
        Debug.Log("Contest start: "+DataManager.Instance.GameData.ContestStart);
        Debug.Log(_currentDate>DataManager.Instance.GameData.ContestEnd);
        Debug.Log(_currentDate<DataManager.Instance.GameData.ContestStart);
        if (DidContestEnd())
        {
            contestNotStartedHolder.SetActive(true);
            Debug.Log((_currentDate-DataManager.Instance.GameData.ContestStart).TotalDays < 10);
            Debug.Log(_currentDate < DataManager.Instance.GameData.ContestEnd);
            if ((_currentDate-DataManager.Instance.GameData.ContestStart).TotalDays < 10 && _currentDate < DataManager.Instance.GameData.ContestEnd)
            {
                StartCoroutine(ShowTimer());
                return;
            }

            contestNotStartedText.text = "Contest finished\nCome back later!";
            return;
        }
        LeaderboardData _data = JsonConvert.DeserializeObject<LeaderboardData>(JsonConvert.SerializeObject(data));

        foreach (var _playerEntry in _data.Entries.ToList())
        {
            if (_playerEntry.KittyUrl != "https://webapiwithssl20230210160824.azurewebsites.net/download/files/blackKitty.svg")
            {
                continue;
            }

            _data.Entries.Remove(_playerEntry);
        }
        
        _data.FinishSetup(3);
        
        int _idx = 0;
        foreach(LeaderboardEntries _playerStats in _data.Entries)
        {
            GameObject _go = Instantiate(leaderboardLinePrefab, leaderboardContent);
            _go.GetComponent<LeaderboardLineBehaviour>().SetPrincipalId(_playerStats.PrincipalId, _idx);
            _go.transform.Find("HorizontalLayout/Points").GetComponent<TextMeshProUGUI>().text = "" + _playerStats.Points;
            _go.transform.Find("HorizontalLayout/Nickname").GetComponent<TextMeshProUGUI>().text = _playerStats.Nickname;

            if (_idx < stars.Count)
            {
                _go.transform.Find("HorizontalLayout/Icon_Text").gameObject.SetActive(false);
                _go.transform.Find("HorizontalLayout/Icon").GetComponent<Image>().sprite = stars[_idx];
            }
            else
            {
                _go.transform.Find("HorizontalLayout/Icon").gameObject.SetActive(false);
                _go.transform.Find("HorizontalLayout/Icon_Text").GetComponent<TextMeshProUGUI>().text ="" + (_idx + 1);
            }

            _idx++;
        }

        if (_data.Entries.Count >= 1)
        {
            firstPlacePoints.text = "" + _data.Entries[0]?.Points;
            firstPlayer.SetCat(_data.TopPlayers[0]);
        }

        if(_data.Entries.Count >= 2)
        {
            secondPlacePoints.text = "" + _data.Entries[1]?.Points;
            secondPlayer.SetCat(_data.TopPlayers[1]);
        }

        if (_data.Entries.Count >= 3)
        {
            thirdPlacePoints.text = "" + _data.Entries[2]?.Points;
            thirdPlayer.SetCat(_data.TopPlayers[2]);
        }
    }

    private bool DidContestEnd()
    {
        DateTime _currentDate = DateTime.UtcNow;
        return _currentDate > DataManager.Instance.GameData.ContestEnd || _currentDate < DataManager.Instance.GameData.ContestStart;
    }

    private IEnumerator ShowTimer()
    {
        while (gameObject.activeSelf)
        {
            TimeSpan _timeRemaining = DataManager.Instance.GameData.ContestStart - DateTime.UtcNow;
            if (_timeRemaining.TotalSeconds<0)
            {
                SceneManager.Instance.Reload();
                yield break;
            }
        
            string _days = _timeRemaining.Days.ToString("D2");
            string _hours = _timeRemaining.Hours.ToString("D2");
            string _minutes = _timeRemaining.Minutes.ToString("D2");
            string _seconds = _timeRemaining.Seconds.ToString("D2");

            contestNotStartedText.text = $"Contest starts in: {_days}:{_hours}:{_minutes}:{_seconds}";

            yield return new WaitForSeconds(1);
        }
    }


    private void ShowLeaderboard()
    {
        showContest.interactable = true;
        showLeaderboard.interactable = false;
        
        int _idx = 0;
        foreach(LeaderboardEntries _playerStats in data.Entries)
        {
            GameObject _go = Instantiate(leaderboardLinePrefab, leaderboardContent);
            _go.GetComponent<LeaderboardLineBehaviour>().SetPrincipalId(_playerStats.PrincipalId, _idx);
            _go.transform.Find("HorizontalLayout/Points").GetComponent<TextMeshProUGUI>().text = "" + _playerStats.Points;
            _go.transform.Find("HorizontalLayout/Nickname").GetComponent<TextMeshProUGUI>().text = _playerStats.Nickname;

            if (_idx < stars.Count)
            {
                _go.transform.Find("HorizontalLayout/Icon_Text").gameObject.SetActive(false);
                _go.transform.Find("HorizontalLayout/Icon").GetComponent<Image>().sprite = stars[_idx];
            }
            else
            {
                _go.transform.Find("HorizontalLayout/Icon").gameObject.SetActive(false);
                _go.transform.Find("HorizontalLayout/Icon_Text").GetComponent<TextMeshProUGUI>().text ="" + (_idx + 1);
            }

            _idx++;
        }

        if (data.Entries.Count >= 1)
        {
            firstPlacePoints.text = "" + data.Entries[0]?.Points;
            firstPlayer.SetCat(data.TopPlayers[0]);
        }

        if(data.Entries.Count >= 2)
        {
            secondPlacePoints.text = "" + data.Entries[1]?.Points;
            secondPlayer.SetCat(data.TopPlayers[1]);
        }

        if (data.Entries.Count >= 3)
        {
            thirdPlacePoints.text = "" + data.Entries[2]?.Points;
            thirdPlayer.SetCat(data.TopPlayers[2]);
        }
    }

    public void GoBack()
    {
        SceneManager.Instance.LoadMainMenu();
    }
}
