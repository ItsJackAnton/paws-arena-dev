using System.Collections.Generic;
using UnityEngine;

public class GuildKingdomSelection : MonoBehaviour
{
   [SerializeField] private Transform kingdomHolder;
   [SerializeField] private GuildKingdomDisplay kingdomPrefab;
   [SerializeField] private GuildBadgeSelection badgeSelection;

   private List<GameObject> shownObjects = new();
   private KingdomSo selectedKingdom;

   public string SelectedKingdomName => selectedKingdom.name;

   private void OnEnable()
   {
      GuildKingdomDisplay.OnSelected += SaveSelectedKingdom;
      ShowKingdoms();
   }

   private void OnDisable()
   {
      GuildKingdomDisplay.OnSelected -= SaveSelectedKingdom;
      ClearKingdoms();
   }

   private void SaveSelectedKingdom(KingdomSo _kingdom)
   {
      selectedKingdom = _kingdom;
      badgeSelection.ShowBadges(selectedKingdom);
   }

   private void ShowKingdoms()
   {
      foreach (var _kingdom in KingdomSo.GetAll())
      {
         GuildKingdomDisplay _display = Instantiate(kingdomPrefab, kingdomHolder);
         
         _display.Setup(_kingdom);
         shownObjects.Add(_display.gameObject);
      }
   }

   private void ClearKingdoms()
   {
      foreach (var _shownObject in shownObjects)
      {
         Destroy(_shownObject);
      }
      
      shownObjects.Clear();
   }

   public void SelectDefault()
   {
      GuildKingdomDisplay _guildKingdomDisplay= shownObjects[0].GetComponent<GuildKingdomDisplay>();
      _guildKingdomDisplay.Select();
   }
}
