using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class FlagManager : MonoBehaviour
{
    [SerializeField] private Image flagImageDisplay;
    [SerializeField] private Image flagShineDisplay;
    [SerializeField] private TextMeshProUGUI messageDisplay;
    [SerializeField] private Material shiningMaterial;

    private static Sprite flagSprite;

    private void Start()
    {
        if (flagSprite == null)
        {
            StartCoroutine(GetImageFromUrl());
        }
        else
        {
            SetDetails();
        }
    }

    private IEnumerator GetImageFromUrl()
    {
        UnityWebRequest _request = UnityWebRequestTexture.GetTexture(DataManager.Instance.GameData.FlagData.ImageUrl);
        yield return _request.SendWebRequest();

        if (_request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Something went wrong while downloading flag image");
        }
        else
        {
            Texture2D _texture = ((DownloadHandlerTexture)_request.downloadHandler).texture;
            Rect _rect = new Rect(0, 0, _texture.width, _texture.height);
            Sprite _sprite = Sprite.Create(_texture, _rect, new Vector2(0.5f, 0.5f), 100);
            flagSprite = _sprite;
            SetDetails();
        }
    }

    private void SetDetails()
    {
        if (flagSprite != null)
        {
            flagImageDisplay.sprite = flagSprite;
            flagShineDisplay.sprite = flagSprite;

            Rect texRect = flagSprite.textureRect;
            Texture2D sourceTexture = flagSprite.texture;

            int width = (int)texRect.width;
            int height = (int)texRect.height;

            Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color[] pixels = sourceTexture.GetPixels(
                (int)texRect.x,
                (int)texRect.y,
                width,
                height
            );

            newTexture.SetPixels(pixels);
            newTexture.Apply();

            shiningMaterial.SetTexture("_Mask", newTexture);
        }

        messageDisplay.text = DataManager.Instance.GameData.FlagData.Description;
    }

}
