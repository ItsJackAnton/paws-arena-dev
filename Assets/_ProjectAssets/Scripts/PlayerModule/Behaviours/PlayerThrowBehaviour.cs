using Anura.ConfigurationModule.Managers;
using Anura.ConfigurationModule.ScriptableObjects;
using Anura.Extensions;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowBehaviour : MonoBehaviour
{
    public static event Action onLaunchPreparing;
    [SerializeField] private PlayerComponent playerComponent;

    [SerializeField] private PlayerIndicatorBehaviour indicator;
    [SerializeField] private Transform launchPoint;

    private Config config => ConfigurationManager.Instance.Config;

    private PhotonView photonView;
    private bool isEnabled = false;

    private List<GameObject> projectiles;

    private void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        isEnabled = true;
        if (photonView.IsMine)
        {
            PlayerActionsBar.OnShoot += PrepareLaunch;
        }
    }

    private void OnDisable()
    {
        isEnabled = false;
        if (photonView.IsMine)
        {
            PlayerActionsBar.OnShoot -= PrepareLaunch;
        }
    }

    public void RegisterThrowCallbacks(GameInputActions.PlayerActions playerActions)
    {
        playerActions.Approve.performed += _ => PrepareLaunch();
    }

    private void PrepareLaunch()
    {
        if (!isEnabled) return;
        isEnabled = false;

        int weaponIdx = playerComponent.state.weaponIdx;
        var weapon = ConfigurationManager.Instance.Weapons.GetWeapon(weaponIdx);

        projectiles = new List<GameObject>();

        for (int i = 0; i < weapon.numberOfProjectiles; i++) {
            var obj = PhotonNetwork.Instantiate("Bullets/" + weapon.bulletPrefab.name, launchPoint.position, Quaternion.Euler(transform.rotation.eulerAngles));
            projectiles.Add(obj);
            if(i != weapon.numberOfProjectiles / 2)
            {
                obj.GetComponent<BulletComponent>().hasEnabledPositionTracking = false;
            }
        }
        onLaunchPreparing?.Invoke();
    }

    public void Launch()
    {

        float deviation = 10;
        for (int i = 0; i < projectiles.Count; i++)
        {
            float angle = deviation * (i - projectiles.Count / 2);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * launchPoint.up;

            projectiles[i].GetComponent<BulletComponent>().Launch(direction, GetBulletSpeed());
        }
        RoomStateManager.Instance.SetProjectileLaunchedState();
    }



    private float GetBulletSpeed()
    {
        return config.GetBulletSpeed(indicator.currentPower);
    }
}