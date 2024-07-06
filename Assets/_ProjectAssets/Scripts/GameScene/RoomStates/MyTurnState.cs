using Anura.ConfigurationModule.Managers;
using Photon.Pun;
using UnityEngine;

public class MyTurnState : IRoomState
{
    public void Init(RoomStateManager context)
    {
        if (!context.isMultiplayer)
        {
            context.lastPlayerRound = 0;
        }
        else
        {
            if (CreateFriendlyMatch.AllowSpectators)
            {
                context.lastPlayerRound = RoomStateManagerSpectator.IsMasterInSpectator ? 3 : 4;
            }
            else
            {
                context.lastPlayerRound = PhotonNetwork.LocalPlayer.IsMasterClient ? 0 : 1;
            }
        }
    }

    public void OnExit()
    {
    }
}
