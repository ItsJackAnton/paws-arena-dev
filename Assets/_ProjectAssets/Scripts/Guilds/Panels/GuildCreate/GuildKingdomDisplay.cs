using System;
using UnityEngine;
using UnityEngine.UI;

public class GuildKingdomDisplay : MonoBehaviour
{
    public static Action<KingdomSo> OnSelected;
    
    [SerializeField] private Image display;
    [SerializeField] private Button select;

    private KingdomSo kingdom;
    
    private void OnEnable()
    {
        select.onClick.AddListener(Select);
        OnSelected += ShowAsSelected;
    }

    private void OnDisable()
    {
        select.onClick.RemoveListener(Select);
        OnSelected += ShowAsSelected;
    }

    public void Select()
    {
        OnSelected?.Invoke(kingdom);
    }

    private void ShowAsSelected(KingdomSo _kingdom)
    {
        display.sprite = _kingdom == kingdom ? kingdom.Selected : kingdom.Normal;
    }

    public void Setup(KingdomSo _kingdom)
    {
        kingdom = _kingdom;
        display.sprite = _kingdom.Normal;
    }
}
