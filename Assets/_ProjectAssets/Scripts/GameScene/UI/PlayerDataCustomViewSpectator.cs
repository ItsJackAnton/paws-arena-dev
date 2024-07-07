using Photon.Pun;
using UnityEngine;

public class PlayerDataCustomViewSpectator : PlayerDataCustomView
{
    protected override void Init()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetParent(GameObject.Find(parentPath).transform);
        rt.localScale = Vector3.one;

        int myseat = PUNGameRoomManager.Instance.GetMySeat() == 3 ? 0 : 1;

        bool isMyPlayer = (myseat == 0 && photonview.IsMine || myseat == 1 && !photonview.IsMine);
        rt.anchorMin = rt.anchorMax = rt.pivot = isMyPlayer ? new Vector2(0, 1) : new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);

        //Mirror UI for right UI
        bool isPlayer2Data = (RoomStateManagerSpectator.IsMasterInSpectator && !photonview.IsMine) || (!RoomStateManagerSpectator.IsMasterInSpectator && photonview.IsMine);
        
        if (isPlayer2Data)
        {
            photonview.RPC(nameof(SetOnRight), RpcTarget.AllBuffered,isMyPlayer);
        }

        if(isMyPlayer)
        {
            healthUIBehaviour.SetTag();
        }
    }

    [PunRPC]

    private void SetOnRight(bool isMyPlayer)
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetParent(GameObject.Find(parentPath).transform);
        rt.localScale = Vector3.one;
        rt.anchorMin = rt.anchorMax = rt.pivot = isMyPlayer ? new Vector2(0, 1) : new Vector2(1, 1);
        healthUIBehaviour.SetOrientationRight();
        nicknameText.alignment = TMPro.TextAlignmentOptions.Right;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-225, GetComponent<RectTransform>().anchoredPosition.y); 
        healthUIBehaviour.OverrideColor(new Color(0.9254901960784314f,0.49411764705882355f,0.803921568627451f));
    }
}
