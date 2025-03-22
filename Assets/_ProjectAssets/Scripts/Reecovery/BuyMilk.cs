using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyMilk : MonoBehaviour
{
    [SerializeField] private Button doneButton;
    [SerializeField] private Button buyJugOfMilkButton;
    [SerializeField] private Button buyGlassOfMilkButton;

    [SerializeField] private TextMeshProUGUI jugOfMilkDisplay;
    [SerializeField] private TextMeshProUGUI glassOfMilkDisplay;

    [SerializeField] private Color normalAmountColor;
    [SerializeField] private Color zeroAmountColor;

    [SerializeField] private GameObject insufficientFounds;

    public void Setup()
    {
        ShowGlassOfMilk();
        ShowJugOfMilk();

        doneButton.onClick.AddListener(Done);
        buyJugOfMilkButton.onClick.AddListener(BuyJugOfMilk);
        buyGlassOfMilkButton.onClick.AddListener(BuyGlassOfMIlk);

        PlayerData.OnUpdatedJugOfMilk += ShowJugOfMilk;
        PlayerData.OnUpdatedGlassOfMilk += ShowGlassOfMilk;


        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        doneButton.onClick.AddListener(Done);
        buyJugOfMilkButton.onClick.AddListener(BuyJugOfMilk);
        buyGlassOfMilkButton.onClick.AddListener(BuyGlassOfMIlk);

        PlayerData.OnUpdatedJugOfMilk -= ShowJugOfMilk;
        PlayerData.OnUpdatedGlassOfMilk -= ShowGlassOfMilk;
    }

    private void ShowJugOfMilk()
    {
        jugOfMilkDisplay.text = DataManager.Instance.PlayerData.JugOfMilk.ToString();
        jugOfMilkDisplay.color = DataManager.Instance.PlayerData.JugOfMilk == 0 ? zeroAmountColor : normalAmountColor;
    }

    private void ShowGlassOfMilk()
    {
        glassOfMilkDisplay.text = DataManager.Instance.PlayerData.GlassOfMilk.ToString();
        glassOfMilkDisplay.color = DataManager.Instance.PlayerData.GlassOfMilk == 0 ? zeroAmountColor : normalAmountColor;
    }

    private void BuyJugOfMilk()
    {
        int _price = 2;
        if (DataManager.Instance.PlayerData.Cookies<_price)
        {
            ShowInsufficientFunds();
            return;
        }

        DataManager.Instance.PlayerData.Cookies -= _price;
        DataManager.Instance.PlayerData.JugOfMilk++;
    }

    private void BuyGlassOfMIlk()
    {
        int _price = 1;
        if (DataManager.Instance.PlayerData.Cookies<_price)
        {
            ShowInsufficientFunds();
            return;
        }

        DataManager.Instance.PlayerData.Cookies -= _price;
        DataManager.Instance.PlayerData.GlassOfMilk++;
    }

    private void ShowInsufficientFunds()
    {
        insufficientFounds.gameObject.SetActive(true);
    }

    private void Done()
    {
        gameObject.SetActive(false);
    }
}