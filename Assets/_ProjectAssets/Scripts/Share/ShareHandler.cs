using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShareHandler : MonoBehaviour
{
    [SerializeField] private Button capture;

    private void OnEnable()
    {
        capture.onClick.AddListener(TakeScreenShot);
    }

    private void OnDisable()
    {
        capture.onClick.RemoveListener(TakeScreenShot);
    }

    private void TakeScreenShot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D _screenImage = new Texture2D(Screen.width, Screen.height);
        _screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        _screenImage.Apply();

        byte[] _imageBytes = _screenImage.EncodeToPNG();
        string _base64Image = System.Convert.ToBase64String(_imageBytes);

        JavaScriptManager.Instance.ShareImageToTwitter(_base64Image,"Test12331");
    }
}
