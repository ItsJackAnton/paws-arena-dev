using Photon.Pun;

public class SendAlivePingSpectator : SendAlivePing
{
    protected override PhotonView GetPhoton()
    {
        return gameObject.AddComponent<PhotonView>();
    }
}
