using System.Collections.Generic;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePass : MonoBehaviour
{
    private const string UPGRADE = "buyPremiumPassIcp";
    [SerializeField] private Button upgrade;
    [SerializeField] private GameObject dialog;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button buy;
    [SerializeField] private Button cancel;
    [SerializeField] private GameObject insufficientFunds;

    private void OnEnable()
    {
        upgrade.onClick.AddListener(Upgrade);
        buy.onClick.AddListener(Buy);
        cancel.onClick.AddListener(Cancel);
        message.text = "Unlock premium rewards for 0.5 ICP?";
        
        upgrade.interactable = !DataManager.Instance.PlayerData.HasPass;
    }

    private void OnDisable()
    {
        upgrade.onClick.RemoveListener(Upgrade);
        buy.onClick.RemoveListener(Buy);
        cancel.onClick.RemoveListener(Cancel);
    }

    private void Upgrade()
    {
        if (!DataManager.Instance.GameData.IsSeasonActive)
        {
            return;
        }
        dialog.SetActive(true);
    }

    private void Buy()
    {
        buy.interactable = false;
        BoomDaoUtility.Instance.ExecuteAction(UPGRADE, HandleUpgrade);
    }

    private void Cancel()
    {
        dialog.SetActive(false);
    }

    private void HandleUpgrade(List<ActionOutcome> _outcomes)
    {
        buy.interactable = true;
        if (_outcomes==default || _outcomes.Count==0)
        {
            insufficientFunds.SetActive(true);
            return;
        }

        Cancel();
        upgrade.interactable = false;
    }
}
