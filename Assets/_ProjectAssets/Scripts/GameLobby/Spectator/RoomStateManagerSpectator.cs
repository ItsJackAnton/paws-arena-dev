using Photon.Pun;

public class RoomStateManagerSpectator : RoomStateManager
{
    public static bool IsMasterInSpectator => int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[PhotonManager.SEAT].ToString()) == 3;
        
    public override void SetFirstPlayerTurn()
    {
        if (int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[PhotonManager.SEAT].ToString())==3)
        {
            SetState(new MyTurnState());
        }
        else
        {
            SetState(new OtherPlayerTurnState());
        }
    }

    public override bool WasMyRound()
    {
        return (lastPlayerRound == 3 && photonManager.GetLocalPlayerProperty(PhotonManager.SEAT)==3)
               || (lastPlayerRound == 4 && photonManager.GetLocalPlayerProperty(PhotonManager.SEAT)==4);
    }

    protected override bool IsMasterClient()
    {
        return IsMasterInSpectator;
    }

    public override void SendRetreatRPC()
    {
        int _isMaster = photonManager.GetLocalPlayerProperty(PhotonManager.SEAT);
        SingleAndMultiplayerUtils.RpcOrLocal(
            this,
            photonView,
            false,
            "Retreat",
            RpcTarget.MasterClient,
            _isMaster
        );
    }
    
    protected override void HandleNextRoundMultiplayer(int _playerNumber)
    {
        if (_playerNumber == 3)
        {
            SetState(
                IsMasterClient()
                    ? new MyTurnState()
                    : new OtherPlayerTurnState()
            );
        }
        else
        {
            SetState(
                IsMasterClient()
                    ? new OtherPlayerTurnState()
                    : new MyTurnState()
            );
        }
    }

    protected override int ExpectedAmountOfPlayers()
    {
        return 4;
    }
}
