using BoomDaoWrapper;
using UnityEngine;
using UnityEngine.UI;

public class CopyPrincipal : MonoBehaviour
{
    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(Copy);        
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Copy);
    }

    private void Copy()
    {
        string _text = BoomDaoUtility.Instance.UserPrincipal;
        Utilities.DoCopyToClipboard(_text);
    }
}