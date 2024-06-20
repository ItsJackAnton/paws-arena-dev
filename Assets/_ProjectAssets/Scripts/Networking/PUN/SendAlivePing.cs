using Photon.Pun;
using UnityEngine;

public class SendAlivePing : MonoBehaviour
{
    private PhotonView _photonView;

    private void OnEnable()
    {
        return;
        _photonView = GetPhoton();
        InvokeRepeating("SendPing", 0, 1);
    }

    protected virtual PhotonView GetPhoton()
    {
        return GetComponent<PhotonView>();
    }

    private void OnDisable()
    {
        CancelInvoke("SendPing");
    }

    public void SendPing()
    {
        _photonView.RPC("SendPingToAllRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void SendPingToAllRPC()
    {

    }
}
