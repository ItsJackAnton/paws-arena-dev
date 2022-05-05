using Anura.Templates.MonoSingleton;
using UnityEngine;
using NaughtyAttributes;
using System;
using Anura.ConfigurationModule.ScriptableObjects;
using Anura.ConfigurationModule.Managers;
using Photon.Pun;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public event Action<int> onHealthUpdated;
    [SerializeField] private PlayerComponent player;

    private PlayerComponent myPlayer;
    private int myPlayerHealth;
    [HideInInspector]
    public int otherPlayerHealth = int.MaxValue;

    public PlayerComponent GetPlayer()
    {
        return player;
    }

    public void RegisterMyPlayer(PlayerComponent playerComponent)
    {
        myPlayer = playerComponent;
        SetMyPlayerHealth(ConfigurationManager.Instance.Config.GetPlayerTotalHealth());
    }

    public GameResolveState GetWinnerByHealth()
    {
        if (myPlayerHealth > 0 && otherPlayerHealth > 0)
        {
            return GameResolveState.NO_WIN;
        }
        else if (myPlayerHealth <= 0 && otherPlayerHealth <= 0)
        {
            return GameResolveState.DRAW;
        }
        else if ((myPlayerHealth > 0 && PhotonNetwork.LocalPlayer.IsMasterClient) || (otherPlayerHealth > 0 && !PhotonNetwork.LocalPlayer.IsMasterClient))
        {
            return GameResolveState.PLAYER_1_WIN;
        }
        else return GameResolveState.PLAYER_2_WIN;
    }

    private void SetMyPlayerHealth(int value)
    {
        value = Math.Max(0, value);
        myPlayerHealth = value;
        onHealthUpdated?.Invoke(myPlayerHealth);
    }

    public void AreaDamage(Vector2 position, float area, int maxDamage, bool damageByDistance, bool hasPushForce, float pushForce)
    {
        Vector3 playerPos = myPlayer.transform.position;
        float dmgDistance = Vector3.Distance(playerPos, position);
        if (dmgDistance > area) return;

        float damagePercentage = (area - dmgDistance) / area;
        int dmgToBeDone = damageByDistance ? (int)Math.Floor(damagePercentage * maxDamage) : maxDamage;
        SetMyPlayerHealth(myPlayerHealth - dmgToBeDone);

        if (hasPushForce)
        {
            Vector2 direction = new Vector2(playerPos.x, playerPos.y) - position;
            PushPlayer(damagePercentage * pushForce, direction);
        }
    }

    private void PushPlayer(float force, Vector2 direction)
    {
        myPlayer.GetComponent<Rigidbody2D>().AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }
}
