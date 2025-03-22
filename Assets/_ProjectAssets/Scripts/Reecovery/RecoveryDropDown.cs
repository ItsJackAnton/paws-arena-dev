using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecoveryDropDown : MonoBehaviour
{
    [SerializeField] private GameObject healMessageHolder;
    [SerializeField] private BuyMilk buyMilkPanel;
    [SerializeField] private GameObject kittyIsFull;

    [SerializeField] private Button healButton;
    [SerializeField] private Button buyButton;

    [SerializeField] private TextMeshProUGUI jugOfMilkDisplay;
    [SerializeField] private TextMeshProUGUI glassOfMilkDisplay;

    [SerializeField] private Color normalAmountColor;
    [SerializeField] private Color zeroAmountColor;

    [SerializeField] private Button jugOfMilkButton;
    [SerializeField] private Button glassOfMilkButton;
    [SerializeField] private RecoveryHandler recoveryHandler;
    
    private RecoveryOption recoveryOption;

    private float animationLength = 0.1f;
    private bool isOpen;

    private void OnEnable()
    {
        recoveryOption = RecoveryOption.JugOfMilk;
        jugOfMilkButton.onClick.AddListener(SelectJugOfMilk);
        glassOfMilkButton.onClick.AddListener(SelectGlassOfMilk);
        ShowSelecteRecoveryOption();
    }

    private void OnDisable()
    {
        jugOfMilkButton.onClick.RemoveListener(SelectJugOfMilk);
        glassOfMilkButton.onClick.RemoveListener(SelectGlassOfMilk);
    }

    private void SelectJugOfMilk()
    {
        recoveryOption = RecoveryOption.JugOfMilk;
        ShowSelecteRecoveryOption();
    }

    private void SelectGlassOfMilk()
    {
        recoveryOption = RecoveryOption.GlassOfMilk;
        ShowSelecteRecoveryOption();
    }

    private void ShowSelecteRecoveryOption()
    {
        Image _jugOfMilkImage = jugOfMilkButton.GetComponent<Image>();
        Image _glassOfMilkImage = glassOfMilkButton.GetComponent<Image>();
        Color _jugOfMilkColor = _jugOfMilkImage.color;
        Color _glassOfMilkColor = _glassOfMilkImage.color;

        if (recoveryOption == RecoveryOption.JugOfMilk)
        {
            _jugOfMilkColor.a = 1;
            _glassOfMilkColor.a = 0;
        }
        else
        {
            _jugOfMilkColor.a = 0;
            _glassOfMilkColor.a = 1;
        }

        _jugOfMilkImage.color = _jugOfMilkColor;
        _glassOfMilkImage.color = _glassOfMilkColor;
    }

    private void Start()
    {
        Close();
    }

    public void HandleClick()
    {
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
        buyButton.onClick.RemoveListener(BuyMilk);
        healButton.onClick.RemoveListener(Heal);
    }

    private void Show()
    {
        gameObject.LeanScale(Vector3.one, animationLength);
        isOpen = true;
        buyButton.onClick.AddListener(BuyMilk);
        healButton.onClick.AddListener(Heal);

        jugOfMilkDisplay.text = DataManager.Instance.PlayerData.JugOfMilk.ToString();
        jugOfMilkDisplay.color = DataManager.Instance.PlayerData.JugOfMilk == 0 ? zeroAmountColor : normalAmountColor;

        glassOfMilkDisplay.text = DataManager.Instance.PlayerData.GlassOfMilk.ToString();
        glassOfMilkDisplay.color = DataManager.Instance.PlayerData.GlassOfMilk == 0 ? zeroAmountColor : normalAmountColor;
    }

    private void Heal()
    {
        if (DataManager.Instance.PlayerData.CanFight)
        {
            kittyIsFull.gameObject.SetActive(true);
            return;
        }

        if (recoveryOption == RecoveryOption.JugOfMilk)
        {
            if (DataManager.Instance.PlayerData.JugOfMilk <= 0)
            {
                healMessageHolder.SetActive(true);
                return;
            }

            DataManager.Instance.PlayerData.JugOfMilk--;
            DataManager.Instance.PlayerData.RecoveryEndDate = DateTime.UtcNow;
            EventsManager.OnHealedKitty?.Invoke();
            EventsManager.OnUsedMilkBottle?.Invoke();
        }
        else
        {
            if (DataManager.Instance.PlayerData.GlassOfMilk <= 0)
            {
                healMessageHolder.SetActive(true);
                return;
            }

            DataManager.Instance.PlayerData.GlassOfMilk--;
            DataManager.Instance.PlayerData.RecoveryEndDate = DataManager.Instance.PlayerData.RecoveryEndDate.AddMinutes(-15);
            recoveryHandler.RestartRoutine(DataManager.Instance.PlayerData.RecoveryEndDate);
            EventsManager.OnHealedKitty?.Invoke();
            return;
        }

        Close();
    }
    
    public void BuyMilk()
    {
        buyMilkPanel.Setup();
        Close();
    }
}
