using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildQuestion : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button yes;
    [SerializeField] private Button no;

    private Action yesCallBack;
    private Action noCallBack;
    
    public void Show(string _question, Action _yesCallback=null, Action _noCallback=null)
    {
        text.text = _question;
        yesCallBack = _yesCallback;
        noCallBack = _noCallback;
        holder.SetActive(true);
    }

    private void OnEnable()
    {
        yes.onClick.AddListener(SelectYes);
        no.onClick.AddListener(SelectNo);
    }

    private void OnDisable()
    {
        yes.onClick.RemoveListener(SelectYes);
        no.onClick.RemoveListener(SelectNo);
    }

    private void SelectYes()
    {
        yesCallBack?.Invoke();
        Close();
    }

    private void SelectNo()
    {
        noCallBack?.Invoke();
        Close();
    }

    private void Close()
    {
        holder.SetActive(false);
    }
}
