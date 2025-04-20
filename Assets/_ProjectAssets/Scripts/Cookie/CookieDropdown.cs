using UnityEngine;
using UnityEngine.UI;

public class CookieDropdown : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button offer1;
    [SerializeField] private Button offer2;
    [SerializeField] private Button offer3;
    

    private bool isOpen;
    private float animationLength = 0.1f;
    
    private PurchaseRequest purchaseRequest1 = new()
    {
        Amount = 100,
        Price = "1666666666666666", // $5 in ETH
        ToAddress = "0xeB946ea643820fb81d1fcf7E5628c5cBD90f2A37"
    };

    private PurchaseRequest purchaseRequest2 = new()
    {
        Amount = 500,
        Price = "16666600000000000", // $50 in ETH
        ToAddress = "0xeB946ea643820fb81d1fcf7E5628c5cBD90f2A37"
    };

    private PurchaseRequest purchaseRequest3 = new()
    {
        Amount = 1000,
        Price = "166666000000000000", // $500 in ETH
        ToAddress = "0xeB946ea643820fb81d1fcf7E5628c5cBD90f2A37"
    };
    
    private void OnEnable()
    {
        button.onClick.AddListener(HandleClick);
        offer1.onClick.AddListener(PurchaseOffer1);
        offer2.onClick.AddListener(PurchaseOffer2);
        offer3.onClick.AddListener(PurchaseOffer3);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(HandleClick);
        offer1.onClick.RemoveListener(PurchaseOffer1);
        offer2.onClick.RemoveListener(PurchaseOffer2);
        offer3.onClick.RemoveListener(PurchaseOffer3);
    }

    public void HandleClick()
    {
        Debug.Log("Handling click");
        if (isOpen)
        {
            Close();
        }
        else
        {
            Show();
        }
    }
    
    private void Close()
    {
        gameObject.LeanScale(Vector3.zero, animationLength);
        isOpen = false;
    }
    
    private void Show()
    {
        gameObject.LeanScale(Vector3.one, animationLength);
        isOpen = true;
    }

    private void PurchaseOffer1()
    {
        JavaScriptManager.Instance.PurchaseCookies(purchaseRequest1,HandlePurchase);
    }
    
    private void PurchaseOffer2()
    {
        JavaScriptManager.Instance.PurchaseCookies(purchaseRequest2,HandlePurchase);
    }
    
    private void PurchaseOffer3()
    {
        JavaScriptManager.Instance.PurchaseCookies(purchaseRequest3,HandlePurchase);
    }
    
    private void HandlePurchase(bool _didPurchase, PurchaseRequest _purchaseRequest)
    {
        if (!_didPurchase)
        {
            return;
        }

        DataManager.Instance.PlayerData.Cookies += _purchaseRequest.Amount;
    }}
