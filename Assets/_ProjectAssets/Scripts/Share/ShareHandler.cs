using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShareHandler : MonoBehaviour
{
    [SerializeField] private Button capture;
    [SerializeField] private GameObject template;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject kittyStand;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Vector3 standPosition;
    

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
        mainMenu.SetActive(false);
        template.SetActive(true);
        PlayerPlatformBehaviour _platform = kittyStand.GetComponentInChildren<PlayerPlatformBehaviour>();
        Vector3 _standPosition = _platform.transform.localPosition;
        nameDisplay.text = DataManager.Instance.PlayerData.Username;
        scoreDisplay.text = DataManager.Instance.PlayerData.LeaderboardPoints.ToString();
        _platform.transform.localPosition = standPosition;
        _platform.Platform.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D _screenImage = new Texture2D(Screen.width, Screen.height);
        _screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        _screenImage.Apply();

        byte[] _imageBytes = _screenImage.EncodeToPNG();
        string _base64Image = Convert.ToBase64String(_imageBytes);

        JavaScriptManager.Instance.ShareImageToTwitter(_base64Image,$"Can you beat my current leaderboard score of " + 
        $"{DataManager.Instance.PlayerData.LeaderboardPoints}?\nI {DataManager.Instance.PlayerData.Username} am challenging you!");
        
        yield return new WaitForEndOfFrame();
        template.SetActive(false);
        _platform.transform.localPosition = _standPosition;
        mainMenu.SetActive(true);
        _platform.Platform.gameObject.SetActive(true);
    }
}
