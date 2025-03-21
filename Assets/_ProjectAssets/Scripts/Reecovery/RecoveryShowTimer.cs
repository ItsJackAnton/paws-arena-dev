using System.Collections;
using UnityEngine;
using TMPro;

public class RecoveryShowTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recoveryDisplay;

    private void OnEnable()
    {
        StartCoroutine(Show());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Show()
    {
        while (gameObject.activeSelf)
        {
            if (DataManager.Instance.PlayerData.CanFight)
            {
                recoveryDisplay.text = string.Empty;
            }
            else
            {
                int _minutes = DataManager.Instance.PlayerData.MinutesUntilHealed;
                if (_minutes!=0)
                {
                    recoveryDisplay.text = _minutes + "m";
                }
                else
                {
                    recoveryDisplay.text = (int)DataManager.Instance.PlayerData.TimeUntilHealed.TotalSeconds + "s";
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}
