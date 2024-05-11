using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMessage : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button close;

    public void Show(string _text)
    {
        text.text = _text;
        holder.SetActive(true);
    }

    private void OnEnable()
    {
        close.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        close.onClick.RemoveListener(Close);
    }

    private void Close()
    {
        holder.SetActive(false);
    }
}
