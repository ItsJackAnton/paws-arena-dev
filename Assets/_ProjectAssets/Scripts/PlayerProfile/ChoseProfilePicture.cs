using System;
using System.Collections.Generic;
using BoomDaoWrapper;
using UnityEngine;
using UnityEngine.UI;

public class ChoseProfilePicture : MonoBehaviour
{
    public static Action<NFT> UpdatedProfilePicture;
    public static Action<string> UpdatedProfilePreviewPicture;
    private const string SET_AVATAR = "setAvatar";
    [SerializeField] private Button show;
    [SerializeField] private GameObject holder;
    [SerializeField] private AvatarChoseDisplay avatarPrefab;
    [SerializeField] private Transform avatarHolder;
    [SerializeField] private Button close;
    [SerializeField] private Button save;
    [SerializeField] private ShowProfilePicture profileDisplay;
    private NFT nft;

    private List<GameObject> shownObjects = new();

    private void Awake()
    {
        if (GameState.nfts.Find(_nft => _nft.imageUrl == DataManager.Instance.GameData.GetAvatarUrl()) !=null)
        {
            return;
        }
        
        SetSelectedAvatar(string.Empty);
    }

    private void OnEnable()
    {
        show.onClick.AddListener(Show);
        AvatarChoseDisplay.OnSelected += ShowProfilePreview;
        close.onClick.AddListener(Close);
        save.onClick.AddListener(Save);
    }

    private void OnDisable()
    {
        show.onClick.RemoveListener(Show);        
        AvatarChoseDisplay.OnSelected -= ShowProfilePreview;
        close.onClick.RemoveListener(Close);
        save.onClick.RemoveListener(Save);
    }

    private void Save()
    {
        if (nft != null)
        {
            SetSelectedAvatar(nft.imageUrl);
        }
        
        Close();
    }
    
    private void SetSelectedAvatar(string _url)
    {
        UpdatedProfilePicture?.Invoke(nft);
        BoomDaoUtility.Instance.ExecuteActionWithParameter(SET_AVATAR,
            new List<ActionParameter>() { new() { Key = GameData.KITTY_AVATAR, Value = _url } }, _ =>
            {
                UpdatedProfilePicture?.Invoke(nft);
            });
    }

    private void Close()
    {
        holder.SetActive(false);
        foreach (var _shownObject in shownObjects)
        {
            Destroy(_shownObject);
        }
        
        shownObjects.Clear();
    }

    private void Show()
    {
        holder.SetActive(true);
        foreach (var _nft in GameState.nfts)
        {
            var _pictureDisplay = Instantiate(avatarPrefab, avatarHolder);
            _pictureDisplay.Setup(_nft);
            shownObjects.Add(_pictureDisplay.gameObject);
        }
    }

    private void ShowProfilePreview(NFT _nft)
    {
        UpdatedProfilePreviewPicture?.Invoke(_nft.imageUrl);
        profileDisplay.ShowImage(_nft);
        nft = _nft;
    }
}
