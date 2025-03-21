using System.Runtime.InteropServices;
using UnityEngine;

public class JavaScriptManager : MonoBehaviour
{
    public static JavaScriptManager Instance;
    
    [DllImport("__Internal")]
    public static extern void DoShareImageToTwitter(string _image, string _text);
    
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string _text);

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShareImageToTwitter(string _image, string _text)
    {
        if (Application.isEditor)
        {
            return;
        }
        DoShareImageToTwitter(_image, _text);
    }
    
    public static void DoCopyToClipboard(string _string)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CopyToClipboard(_string);
        }
        else
        {
            GUIUtility.systemCopyBuffer = _string;
        }
    }
}
