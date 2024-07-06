using Photon.Pun;
using UnityEngine;

public class OtherPlayerTurnState : IRoomState
{
    public void Init(RoomStateManager context)
    {
        if (CreateFriendlyMatch.AllowSpectators)
        {
            context.lastPlayerRound = RoomStateManagerSpectator.IsMasterInSpectator ? 4 : 3;
        }
        else
        {
            context.lastPlayerRound = PhotonNetwork.LocalPlayer.IsMasterClient ? 1 : 0;
        }
    }

    public void OnExit()
    {
    }
}