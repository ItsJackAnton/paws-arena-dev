using System;
using System.Collections.Generic;
using BoomDaoWrapper;
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

    public List<Sprite> stars;

    private void OnEnable()
    {
        reload.onClick.AddListener(RequestReload);
        BoomDaoUtility.OnUpdatedWorldData += ReloadScene;
    }

    private void OnDisable()
    {
        reload.onClick.RemoveListener(RequestReload);
        BoomDaoUtility.OnUpdatedWorldData -= ReloadScene;
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
        PopulateLeaderboard();
    }

    private void PopulateLeaderboard()
    {
        try
        {
            // LeaderboardData _data = DataManager.Instance.GameData.GetLeaderboard;
            // PopulateLeaderboardData(_data);
        }
        catch (Exception _exception)
        {
            Debug.Log($"Exception occured during loading leadeboard data{_exception.Message} {_exception.StackTrace}");
            throw;
        }
    }

    private void PopulateLeaderboardData(LeaderboardData _data)
    {
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

    public void GoBack()
    {
        SceneManager.Instance.LoadMainMenu();
    }
}
