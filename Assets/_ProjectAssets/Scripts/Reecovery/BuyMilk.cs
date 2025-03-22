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
        ManageInteractables(false);
        if (JavaScriptManager.UseMockUpData)
        {
            bool _outcome = Random.Range(0, 2) == 1;
            if (_outcome)
            {
                DataManager.Instance.PlayerData.JugOfMilk++;
            }
            else
            {
                ShowInsufficientFunds();
            }
            
            ManageInteractables(true);
        }
        else
        {
            //Todo fix me Abstract
        }
    }

    private void BuyGlassOfMIlk()
    {
        ManageInteractables(false);
        if (JavaScriptManager.UseMockUpData)
        {
            bool _outcome = Random.Range(0, 2) == 1;
            if (_outcome)
            {
                DataManager.Instance.PlayerData.GlassOfMilk++;
            }
            else
            {
                ShowInsufficientFunds();
            }
            
            ManageInteractables(true);
        }
        else
        {
            //Todo fix me Abstract
        }
    }

    private void ShowInsufficientFunds()
    {
        insufficientFounds.gameObject.SetActive(true);
    }

    private void Done()
    {
        gameObject.SetActive(false);
    }

    private void ManageInteractables(bool _status)
    {
        buyJugOfMilkButton.interactable = _status;
        buyGlassOfMilkButton.interactable = _status;
    }
}
