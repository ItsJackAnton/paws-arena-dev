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
