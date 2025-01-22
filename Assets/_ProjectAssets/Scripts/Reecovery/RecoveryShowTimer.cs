using System;
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
            if (GameState.selectedNFT.CanFight)
            {
                recoveryDisplay.text = string.Empty;
            }
            else
            {
                int _minutes = GameState.selectedNFT.MinutesUntilHealed;
                if (_minutes!=0)
                {
                    recoveryDisplay.text = _minutes + "m";
                }
                else
                {
                    recoveryDisplay.text = (int)GameState.selectedNFT.TimeUntilHealed.TotalSeconds + "s";
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}
