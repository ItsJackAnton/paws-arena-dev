using System;
using UnityEngine;
using UnityEngine.UI;

public class GuildsPanel : MonoBehaviour
{
   public static GuildsPanel Instance;

   [SerializeField] private NotInAGuild notInAGuild;
   [SerializeField] private GuildCreate createGuild;
   [SerializeField] private GuildMy guildPanel;
   [SerializeField] private GuildTop guildTop;
   [SerializeField] private GuildsSearch guildsSearch;
   [SerializeField] private GuildJoin guildJoin;
   [SerializeField] private GuildBattlesPanel guildBattles;
   
   [SerializeField] private Button myGuild;
   [SerializeField] private Button guildBattle;
   [SerializeField] private Button topGuilds;
   [SerializeField] private Button searchGuild;
   [SerializeField] private Button close;

   [SerializeField] private Sprite selectedOption;
   [SerializeField] private Sprite notSelectedOption;

   [SerializeField] private GuildMessage guildMessage;
   [SerializeField] private GuildQuestion guildQuestion;
   [SerializeField] private GameObject inputBlocker;

   private void Awake()
   {
      Instance = this;
   }

   private void OnEnable()
   {
      myGuild.onClick.AddListener(ShowMyGuild);
      guildBattle.onClick.AddListener(ShowGuildBattle);
      topGuilds.onClick.AddListener(ShowTopGuilds);
      searchGuild.onClick.AddListener(ShowSearchGuilds);
      close.onClick.AddListener(Close);
   }

   private void OnDisable()
   {
      myGuild.onClick.RemoveListener(ShowMyGuild);
      guildBattle.onClick.RemoveListener(ShowGuildBattle);
      topGuilds.onClick.RemoveListener(ShowTopGuilds);
      searchGuild.onClick.RemoveListener(ShowSearchGuilds);
      close.onClick.RemoveListener(Close);
   }

   private void Start()
   {
      DataManager.Instance.PlayerData.GetMyGuild();
      ShowMyGuild();
   }

   public void ShowMyGuild()
   {
      CloseAll();
      myGuild.image.sprite = selectedOption;
      
      if (DataManager.Instance.PlayerData.IsInAGuild)
      {
         guildPanel.Setup();
      }
      else
      {
         ShowNotInAGuild();
      }
   }

   private void ShowGuildBattle()
   {
      CloseAll();
      guildBattle.image.sprite = selectedOption;
      
      if (DataManager.Instance.PlayerData.IsInAGuild)
      {
         guildPanel.Setup();
         return;
      }

      ShowMessage("You are not in a guild,\nplease join one in order to participate in guild battles");
      ShowSearchGuilds();
   }

   private void ShowTopGuilds()
   {
      CloseAll();
      topGuilds.image.sprite = selectedOption;
      
      guildTop.Setup();
   }

   public void ShowSearchGuilds()
   {
      CloseAll();
      searchGuild.image.sprite = selectedOption;
      guildsSearch.Setup();
   }
   
   public void ShowNotInAGuild()
   {
      CloseAll();
      myGuild.image.sprite = selectedOption;
      notInAGuild.Setup();
   }

   public void ShowCreateGuild()
   {
      CloseAll();
      myGuild.image.sprite = selectedOption;
      createGuild.Setup();
   }

   public void ShowJoinGuild(GuildData _guildData)
   {
      CloseAll();
      guildJoin.Setup(_guildData);
   }

   private void CloseAll()
   {
      myGuild.image.sprite = notSelectedOption;
      guildBattle.image.sprite = notSelectedOption;
      topGuilds.image.sprite = notSelectedOption;
      searchGuild.image.sprite = notSelectedOption;
      
      notInAGuild.Close();
      createGuild.Close();
      guildPanel.Close();
      guildTop.Close();
      guildsSearch.Close();
      guildJoin.Close();
      guildBattles.Close();
   }

   private void Close()
   {
      SceneManager.Instance.LoadMainMenu();
   }

   public void ShowMessage(string _message)
   {
      guildMessage.Show(_message);
   }
   
   public void ShowQuestion(string _question, Action _yesCallback=null, Action _noCallback=null)
   {
      guildQuestion.Show(_question,_yesCallback,_noCallback);
   }

   public void ManageInputBlocker(bool _show)
   {
      inputBlocker.SetActive(_show);
   }
   
}
