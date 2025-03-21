using System;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    public static Action OnLoadedImages;
    public Transform playerPlatformPosition;
    public GameObject playerPlatformPrefab;

    private GameObject playerPlatform;
    public static bool DidInit;


    private void Awake()
    {
        DidInit = false;
    }

    private void OnEnable()
    {
        playerPlatform = Instantiate(playerPlatformPrefab, playerPlatformPosition);
        playerPlatform.transform.position = Vector3.zero;
    }

    private void OnDisable()
    {
        if(playerPlatform != null)
        {
            Destroy(playerPlatform);
        }
    }

    private void Start()
    {
        if (DidInit)
        {
            return;
        }
        
        DidInit=true;
        _ = DataManager.Instance.PlayerData.Nft.GrabImage(() =>
        {
            OnLoadedImages?.Invoke();
        });
    }
}
