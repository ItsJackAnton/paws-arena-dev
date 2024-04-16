using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameResolveState
{
    NO_WIN,
    DRAW, 
    PLAYER_1_WIN,
    PLAYER_2_WIN,
}

public class GameResolveStateUtils{
    public static int CheckIfIWon(GameResolveState state)
    {
        if ((state == GameResolveState.PLAYER_1_WIN && PhotonNetwork.LocalPlayer.IsMasterClient) ||
            (state == GameResolveState.PLAYER_2_WIN && !PhotonNetwork.LocalPlayer.IsMasterClient))
        {
            Debug.Log("----- Decided in Unity that I won");
            return 1;
        }
        else if ((state == GameResolveState.PLAYER_1_WIN && !PhotonNetwork.LocalPlayer.IsMasterClient) ||
            (state == GameResolveState.PLAYER_2_WIN && PhotonNetwork.LocalPlayer.IsMasterClient))
        {
            Debug.Log("----- Decided in Unity that I lost");
            return -1;
        }
        
        Debug.Log("----- Decided in Unity that it is a draw");
        return 0;
    }
}