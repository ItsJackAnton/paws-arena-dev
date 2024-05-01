using System.Globalization;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;

public class IcpDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI icpAmountDisplay;

    private void OnEnable()
    {
        PlayerData.OnUpdatedToken += ShowIcp;
        ShowIcp();
    }

    private void OnDisable()
    {
        PlayerData.OnUpdatedToken -= ShowIcp;
    }

    private void ShowIcp()
    {
        icpAmountDisplay.text = BoomDaoUtility.Instance.GetTokenBalance(BoomDaoUtility.ICP_KEY).ToString(CultureInfo.InvariantCulture);
    }
}
