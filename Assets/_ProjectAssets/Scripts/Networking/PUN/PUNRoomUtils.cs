using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PUNRoomUtils : MonoBehaviourPunCallbacks
{
    public static event Action<string> onPlayerJoined;
    public static event Action onPlayerLeft;
    public void TryLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void AddPlayerCustomProperty(string key, string value)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }


    public List<Player> GetOtherPlayers()
    {
        return PhotonNetwork.CurrentRoom.Players.Select(kvp => kvp.Value).Where(player => !player.IsLocal).ToList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player Joined room {newPlayer.NickName}");
        onPlayerJoined?.Invoke(newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"Player Left Room {otherPlayer.NickName}");
        onPlayerLeft?.Invoke();
    }
}
