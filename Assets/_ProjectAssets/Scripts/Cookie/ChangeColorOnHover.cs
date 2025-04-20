using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeColorOnHover : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image image;
    
    public void OnPointerEnter(PointerEventData _eventData)
    {
        var _color = image.color;
        _color.a = 1;
        image.color = _color;
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        var _color = image.color;
        _color.a = 0;
        image.color = _color;
    }
}
