using System;
using Photon.Pun;
using UnityEngine;

public class SyncPlatformsBehaviourSpectator : SyncPlatformsBehaviour
{
    [SerializeField] private PlatformPose spectator1pose;
    [SerializeField] private PlatformPose spectator2pose;
    
    public override PlatformPose GetMySeatPosition(PhotonView _photonView, bool _isBot)
    {
        foreach (var (_number,_player) in PhotonNetwork.CurrentRoom.Players)
        {
            if (!Equals(_photonView.Owner, _player))
            {
                continue;
            }

            switch (_number)
            {
                case 1: return spectator1pose;
                case 2: return spectator2pose;
                case 3: return player1Pose;
                case 4: return player2Pose;
            }
        }

        throw new Exception("Cant find pose for " + _photonView.Owner);
    }
}