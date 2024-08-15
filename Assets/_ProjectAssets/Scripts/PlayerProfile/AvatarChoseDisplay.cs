using System;
using UnityEngine;
using UnityEngine.UI;

public class AvatarChoseDisplay : MonoBehaviour
{
    public static Action<NFT> OnSelected;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selected;
    private NFT nft;
    
    private void OnEnable()
    {
        button.onClick.AddListener(Select);
        ChoseProfilePicture.UpdatedProfilePreviewPicture += ShowIsSelected;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Select);
        ChoseProfilePicture.UpdatedProfilePreviewPicture -= ShowIsSelected;
    }

    private void Select()
    {
        OnSelected?.Invoke(nft);
    }

    public void Setup(NFT _nft)
    {
        nft = _nft;
        image.sprite = _nft.Sprite;
        ShowIsSelected(DataManager.Instance.GameData.GetAvatarUrl());
    }

    private void ShowIsSelected(string _url)
    {
        selected.SetActive(_url==nft.imageUrl);
    }
}
