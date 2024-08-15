using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NFT
{
    public string imageUrl;
    public string furType;
    public List<string> ids;
    public Texture2D imageTex;
    private XmlDocument doc;
    private DateTime recoveryEndDate;
    public Sprite Sprite;
    public Sprite Avatar;

    public bool CanFight => RecoveryEndDate < DateTime.UtcNow;
    public bool IsDefaultKitty => imageUrl == ConnectingToServer.DEFAULT_KITTY;
    public int MinutesUntilHealed => (int)(RecoveryEndDate - DateTime.UtcNow).TotalMinutes;

    public TimeSpan TimeUntilHealed => RecoveryEndDate - DateTime.UtcNow;

    public Action UpdatedRecoveryTime;

    public DateTime RecoveryEndDate
    {
        get
        {
            return recoveryEndDate;
        }
        set
        {
            recoveryEndDate = value;
            UpdatedRecoveryTime?.Invoke();
        }
    }

    public async UniTask GrabImage(Action _callBack=null)
    {
        doc = await NFTImageLoader.LoadSVGXML(imageUrl);
        if (imageTex == null)
        {
            //imageTex = NFTImageLoader.LoadNFT(doc);
            imageTex = NFTImageLoader.LoadNFTLocal(doc);
        }
        if (furType == null)
        {
            furType = NFTImageLoader.GetFurType(doc);
        }
        if (ids == null)
        {
            ids = NFTImageLoader.GetIds(doc);
        }

        if (imageTex)
        {
            Sprite = Utilities.TextureToSprite(imageTex);
            Avatar = Utilities.ConvertTextureToProfileSprite(imageTex,new Vector2Int(35, 30),50);
        }
        _callBack?.Invoke();
    }
}
