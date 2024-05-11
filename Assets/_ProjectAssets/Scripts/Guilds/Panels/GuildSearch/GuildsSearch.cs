using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildsSearch : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private GuildSearchResultDisplay searchResultPrefab;
    [SerializeField] private Transform resultsHolder;
    [SerializeField] private Button showUp;
    [SerializeField] private Button showDown;

    private List<GameObject> shownObjects = new();
    private float moveAmount = 1;
    
    private void OnEnable()
    {
        showUp.onClick.AddListener(ShowUp);
        showDown.onClick.AddListener(ShowDown);
        searchInput.onEndEdit.AddListener(Search);

        GuildSearchResultDisplay.OnJoinGuild += JoinGuild;
    }

    private void OnDisable()
    {
        showUp.onClick.RemoveListener(ShowUp);
        showDown.onClick.RemoveListener(ShowDown);
        searchInput.onEndEdit.RemoveListener(Search);
        
        GuildSearchResultDisplay.OnJoinGuild -= JoinGuild;
    }

    public void Setup()
    {
        searchInput.text = string.Empty;
        SearchGuilds();
        holder.SetActive(true);
    }

    public void Close()
    {
        holder.SetActive(false);
    }
    
    private void Search(string _)
    {
        SearchGuilds();
    }

    private void ShowUp()
    {
        var _holderTransform = resultsHolder.transform;
        
        Vector3 _itemsPosition = _holderTransform.position;
        _itemsPosition.y -= moveAmount;
        _holderTransform.position = _itemsPosition;
    }

    private void ShowDown()
    {
        var _holderTransform = resultsHolder.transform;
        
        Vector3 _itemsPosition = _holderTransform.position;
        _itemsPosition.y += moveAmount;
        _holderTransform.position = _itemsPosition;
    }

    private void SearchGuilds()
    {
        CleanShownGuilds();
        
        if (DataManager.Instance.GameData.Guilds.Count==0)
        {
            GuildsPanel.Instance.ShowMessage("There are no created guilds at the moment");
            GuildsPanel.Instance.ShowNotInAGuild();
            return;
        }
        
        string _searchKey = searchInput.text;

        foreach (var _guildData in DataManager.Instance.GameData.Guilds.ToList().OrderBy(_guild => _guild.Name))
        {
            if (!string.IsNullOrEmpty(_searchKey))
            {
                if (!_guildData.Name.Contains(_searchKey))
                {
                    continue;
                }
            }
            
            GuildSearchResultDisplay _searchResult = Instantiate(searchResultPrefab, resultsHolder);
            _searchResult.Setup(_guildData);
            shownObjects.Add(_searchResult.gameObject);
        }

        if (shownObjects.Count==0)
        {
            GuildsPanel.Instance.ShowMessage("No guilds match the given name");
        }
    }

    private void CleanShownGuilds()
    {
        foreach (var _shownObject in shownObjects)
        {
            Destroy(_shownObject);
        }

        shownObjects.Clear();
    }

    private void JoinGuild(GuildData _guildData)
    {
        GuildsPanel.Instance.ShowJoinGuild(_guildData);
    }
}
