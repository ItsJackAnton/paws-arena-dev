using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class GameMatchingScreenSpectator : GameMatchingScreen
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties[PhotonManager.ALLOW_SPECTATORS] != null)
        {
            CreateFriendlyMatch.AllowSpectators = Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties[PhotonManager.ALLOW_SPECTATORS]) == 1;
        }
    }

    protected override void Init()
    {
        notices.SetActive(false);
        SetSeats();

        startButton.gameObject.SetActive(false);

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            return;
        }
        int _mapIdx = Random.Range(0, 7);
        punRoomUtils.AddRoomCustomProperty("mapIdx", _mapIdx);
    }

    public override void SetSeats()
    {
        foreach (SeatGameobject seat in seats)
        {
            FreeSeat(seat);
        }

        foreach (var (_number,_photonPlayer) in PhotonNetwork.CurrentRoom.Players)
        {
            if (_photonPlayer.IsLocal)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
                {
                    {PhotonManager.SEAT, _number}
                });
            }
            
            int _seat = Convert.ToInt32(_photonPlayer.CustomProperties[PhotonManager.SEAT]);
            string _name = _photonPlayer.CustomProperties[PhotonManager.NAME].ToString();
            OccupySeat(seats[_seat], _name);
        }
        
        startButton.gameObject.SetActive(Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties[PhotonManager.SEAT])<=2 && PhotonNetwork
        .CurrentRoom.Players.Count==4);
    }

    protected override void OnPlayerJoined(string _opponentNickname, string _userId)
    {
        StartCoroutine(OnPlayerJoinedRoutine());
        
        IEnumerator OnPlayerJoinedRoutine()
        {
            yield return new WaitForSeconds(2);
            int _playerIndex = -1;
            Player _player = null;

            foreach (var (_index,_playerInRoom) in PhotonNetwork.CurrentRoom.Players)
            {
                if (_playerInRoom.UserId!=_userId)
                {
                    continue;
                }

                _playerIndex = _index;
                _player = _playerInRoom;
            }
            
            Debug.Log($"{_opponentNickname}: {_userId} = {_playerIndex}");
        
            OccupySeat(seats[_playerIndex-1], _player.CustomProperties[PhotonManager.NAME].ToString());
        }
    }

    protected override void HandleStartButton()
    {
        startButton.gameObject.SetActive(false);
    }
}
