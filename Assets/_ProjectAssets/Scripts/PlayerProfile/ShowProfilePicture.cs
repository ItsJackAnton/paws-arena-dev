using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowProfilePicture : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        ChoseProfilePicture.UpdatedProfilePicture += ShowNewAvatar;
        MainMenuScreen.OnLoadedImages += ShowImage;
    }

    private void OnDisable()
    {
        ChoseProfilePicture.UpdatedProfilePicture -= ShowNewAvatar;
        MainMenuScreen.OnLoadedImages -= ShowImage;
    }

    private void ShowNewAvatar(NFT _nft)
    {
        ShowImage(_nft);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MainMenuScreen.DidInit);
        if (GameState.selectedNFT == null)
        {
            SceneManager.Instance.LoadNftSelection();
            yield break;
        }
        
        ShowImage(DataManager.Instance.GameData.GetAvatarUrl());
    }

    private void ShowImage()
    {
        string _url = DataManager.Instance.GameData.GetAvatarUrl();
        if (string.IsNullOrEmpty(_url))
        {
            return;
        }
        
        ShowImage(_url);
    }

    private void ShowImage(string _url)
    {
        if (string.IsNullOrEmpty(_url))
        {
            return;
        }

        NFT _nft = GameState.nfts.Find(_nft => _nft.imageUrl == _url);
        ShowImage(_nft);
    }
    
    public void ShowImage(NFT _nft)
    {
        if (_nft == null)
        {
            return;
        }
        Show(_nft.Avatar);
    }

    private void Show(Sprite _sprite)
    {
        image.sprite = _sprite;
    }
}
