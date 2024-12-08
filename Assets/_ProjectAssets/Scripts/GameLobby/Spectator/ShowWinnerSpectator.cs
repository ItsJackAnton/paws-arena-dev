using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

public class ShowWinnerSpectator : PlayerPlatformBehaviour
{
    protected override void OnEnable()
    {
        Debug.Log(PhotonManager.ROOM_WINNER);
        Debug.Log(PhotonNetwork.CurrentRoom);
        string _url = PhotonNetwork.CurrentRoom.CustomProperties[PhotonManager.ROOM_WINNER].ToString();
        List<string> _ids = JsonConvert.DeserializeObject<List<string>>(PhotonNetwork.CurrentRoom.CustomProperties[PhotonManager.ROOM_WINNER_IDS].ToString());
        playerCustomization.SetCat(_url, _ids);
    }
}
