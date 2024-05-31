using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildBattleGuildDisplay : MonoBehaviour
{
    [SerializeField] private Image badgeDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Image kittyDisplay;

    public int Score => Convert.ToInt32(scoreDisplay);
    
    public void Setup(Sprite _badge, string _name, string _score, Color _color)
    {
        badgeDisplay.sprite = _badge;
        nameDisplay.text = _name;
        scoreDisplay.text = _score;
        kittyDisplay.color = _color;
    }
}
