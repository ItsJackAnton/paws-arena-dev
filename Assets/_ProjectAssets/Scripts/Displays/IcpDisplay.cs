using System.Globalization;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;

public class IcpDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI icpAmountDisplay;

    private void OnEnable()
    {
        PlayerData.OnUpdatedSnacks += ShowSnacks;
        ShowSnacks();
    }

    private void OnDisable()
    {
        PlayerData.OnUpdatedSnacks -= ShowSnacks;
    }

    private void ShowSnacks()
    {
        icpAmountDisplay.text = BoomDaoUtility.Instance.IcpAmount.ToString(CultureInfo.InvariantCulture);
    }
}
