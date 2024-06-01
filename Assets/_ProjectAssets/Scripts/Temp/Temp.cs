using System;
using NaughtyAttributes;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField] private string text;

    [Button()]
    private void ConvertTextToDate()
    {
        Debug.Log(Utilities.NanosecondsToDateTime(Convert.ToDouble(text)));
    }
}
