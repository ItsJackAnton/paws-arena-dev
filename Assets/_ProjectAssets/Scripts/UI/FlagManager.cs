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

            Texture2D _texture = new Texture2D((int)flagSprite.rect.width, (int)flagSprite.rect.height);
            _texture.SetPixels(flagSprite.texture.GetPixels((int)flagSprite.textureRect.x,
                (int)flagSprite.textureRect.y,
                (int)flagSprite.textureRect.width,
                (int)flagSprite.textureRect.height));
            _texture.Apply();

            shiningMaterial.SetTexture("_Mask", _texture);
        }
        
        messageDisplay.text = DataManager.Instance.GameData.FlagData.Description;
    }
}
