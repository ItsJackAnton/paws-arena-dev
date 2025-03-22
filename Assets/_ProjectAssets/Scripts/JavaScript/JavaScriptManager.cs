using System.Runtime.InteropServices;
using UnityEngine;

public class JavaScriptManager : MonoBehaviour
{
    public static JavaScriptManager Instance;

    public static bool UseMockUpData = true;
    
    [DllImport("__Internal")]
    public static extern void DoShareImageToTwitter(string _image, string _text);
    
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string _text);    
    
    [DllImport("__Internal")]
    public static extern void ReceiveMessage(string _text);

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

    public void SendMessage()
    {
        ReceiveMessage("This is a test message from Unity");
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

    public void ReceiveMessageOutside(string _message)
    {
        Debug.Log("Received message outside Unity: "+_message);
    }
}
