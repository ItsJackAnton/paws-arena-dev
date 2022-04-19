using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticesBehaviour : MonoBehaviour
{
    public GameObject wrapper;
    public TMPro.TextMeshProUGUI label;

    private void OnEnable()
    {
        wrapper.SetActive(false);
        RoomStateManager.OnStateUpdated += OnStateUpdated;
    }

    private void OnDisable()
    {
        RoomStateManager.OnStateUpdated -= OnStateUpdated;
    }

    private void OnStateUpdated(GameSceneStates state)
    {
        if (state == GameSceneStates.WAITING_FOR_ALL_PLAYERS_TO_JOIN)
        {
            SetStatus("Waiting for players to join...");
        }else if(state == GameSceneStates.STARTING_GAME)
        {
            SetStatus("Let's go!");
        }else if(state == GameSceneStates.PLAYER_1)
        {
            SetStatus("PLAYER 1");
        }else if(state == GameSceneStates.PLAYER_2)
        {
            SetStatus("PLAYER 2");
        }else if (state == GameSceneStates.PROJECTILE_LAUNCHED)
        {
            SetStatus("Boom!");
        }
    }

    private void SetStatus(string value)
    {
        wrapper.SetActive(true);
        label.text = value;
    }
}
