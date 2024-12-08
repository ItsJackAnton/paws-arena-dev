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

        StartCoroutine(ShowPlayerNamesRoutine());
    }
    
    private IEnumerator ShowPlayerNamesRoutine()
    {
        while (gameObject)
        {
            foreach (SeatGameobject _seat in seats)
            {
                _seat.occupierNickname.text = "-";
            }
            foreach (var _photonPlayer in PhotonNetwork.CurrentRoom.Players)
            {
                int _seat = Convert.ToInt32(_photonPlayer.Value.CustomProperties[PhotonManager.SEAT]);
                _seat--;
                if (_seat<0)
                {
                    continue;
                }

                if (_seat>= seats.Count)
                {
                    continue;
                }
                string _name = _photonPlayer.Value.CustomProperties[PhotonManager.NAME].ToString();
                Debug.Log($"----- {_seat}: {_name}");
                seats[_seat].occupierNickname.text = _name;
            }

            yield return new WaitForSeconds(1);
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
        }
        
        startButton.gameObject.SetActive(Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties[PhotonManager.SEAT])<=2 && PhotonNetwork
        .CurrentRoom.Players.Count==4);
    }

    protected override void OnPlayerJoined(string _opponentNickname, string _userId)
    {
        
    }

    protected override void HandleStartButton()
    {
        startButton.gameObject.SetActive(false);
    }
}
