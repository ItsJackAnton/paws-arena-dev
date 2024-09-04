using UnityEngine;
using UnityEngine.UI;

public class ElementumReward : MonoBehaviour
{
    [SerializeField] private Button copy;
    [SerializeField] private Button close;

    private void OnEnable()
    {
        copy.onClick.AddListener(Copy);
        close.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        copy.onClick.RemoveListener(Copy);
        close.onClick.RemoveListener(Close);
    }

    private void Copy()
    {
        Utilities.CopyToClipboard("KITKAM");
    }

    private void Close()
    {
        close.interactable = false;
        PUNRoomUtils _roomUtilities= FindObjectOfType<PUNRoomUtils>();
        if (_roomUtilities!=null)
        {
            _roomUtilities.TryLeaveRoom();
        }
        else
        {
            gameObject.SetActive(false);
            LuckyWheelUI.OnClaimed?.Invoke();
        }
    }
}
