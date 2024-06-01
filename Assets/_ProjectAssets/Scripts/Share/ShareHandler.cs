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
        capture.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D _screenImage = new Texture2D(Screen.width, Screen.height);
        _screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        _screenImage.Apply();

        byte[] _imageBytes = _screenImage.EncodeToPNG();
        string _base64Image = Convert.ToBase64String(_imageBytes);

        JavaScriptManager.Instance.ShareImageToTwitter(_base64Image,$"Can you beat my current leaderboard score of " + 
        $"{DataManager.Instance.PlayerData.LeaderboardPoints}?\nI {DataManager.Instance.PlayerData.Username} am challenging you!");
        
        yield return new WaitForEndOfFrame();
        capture.gameObject.SetActive(true);
    }
}
