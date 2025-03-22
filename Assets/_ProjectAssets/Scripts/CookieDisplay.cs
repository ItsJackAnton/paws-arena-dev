using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookieDisplay : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI display;
   [SerializeField] private Button purchase;

   private void OnEnable()
   {
      PlayerData.OnUpdatedCookies += ShowCookies;
      purchase.onClick.AddListener(Purchase);
   }

   private void OnDisable()
   {
      PlayerData.OnUpdatedCookies -= ShowCookies;
      purchase.onClick.RemoveListener(Purchase);
   }

   private void Purchase()
   {
      JavaScriptManager.Instance.PurchaseCookies(HandlePurchase);
   }

   private void HandlePurchase(bool _didPurchase, int _amount)
   {
      if (!_didPurchase)
      {
         return;
      }

      DataManager.Instance.PlayerData.Cookies += _amount;
   }

   private void ShowCookies()
   {
      display.text = DataManager.Instance.PlayerData.Cookies.ToString();
   }
}
