using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayLevelBarBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI levelDisplay;
    [SerializeField] protected Image levelBar;
    
    protected void ShowProgress(double _experience)
    {
        int _level;
        int _expForNextLevel;
        int _expOnCurrentLevel;

        PlayerData.CalculateLevel(_experience, out _level, out _expForNextLevel, out _expOnCurrentLevel);

        levelDisplay.text = _level.ToString();
        levelBar.fillAmount = (float)_expOnCurrentLevel / _expForNextLevel;
    }
}
