using System;
using UnityEngine;
using UnityEngine.UI;

public class NotInAGuild : MonoBehaviour
{
   [SerializeField] private GameObject holder;
   [SerializeField] private Button createGuild;
   [SerializeField] private Button joinGuild;
   
   public void Setup()
   {
      holder.SetActive(true);
   }

   public void Close()
   {
      holder.SetActive(false);
   }

   private void OnEnable()
   {
      createGuild.onClick.AddListener(CreateGuild);
      joinGuild.onClick.AddListener(JoinGuild);
   }

   private void OnDestroy()
   {
      createGuild.onClick.RemoveListener(CreateGuild);
      joinGuild.onClick.RemoveListener(JoinGuild);
   }

   private void CreateGuild()
   {
      GuildsPanel.Instance.ShowCreateGuild();
   }

   private void JoinGuild()
   {
      GuildsPanel.Instance.ShowSearchGuilds();
   }
}
