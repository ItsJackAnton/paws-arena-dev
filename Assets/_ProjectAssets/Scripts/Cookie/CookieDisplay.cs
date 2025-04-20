using TMPro;
using UnityEngine;

public class CookieDisplay : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI display;

   private void OnEnable()
   {
      PlayerData.OnUpdatedCookies += ShowCookies;
      ShowCookies();
   }

   private void OnDisable()
   {
      PlayerData.OnUpdatedCookies -= ShowCookies;
   }

   private void ShowCookies()
   {
      display.text = DataManager.Instance.PlayerData.Cookies.ToString();
   }
}
