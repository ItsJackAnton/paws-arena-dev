using System.Runtime.InteropServices;
using UnityEngine;

public class JavaScriptManager : MonoBehaviour
{
    public static JavaScriptManager Instance;
    
    [DllImport("__Internal")]
    public static extern void DoShareImageToTwitter(string _image, string _text);

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
        DoShareImageToTwitter(_image, _text);
    }
}
