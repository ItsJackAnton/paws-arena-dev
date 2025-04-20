using Photon.Pun;
using UnityEngine;

public class SyncPlayerPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerCustomization playerCustomization;

    [HideInInspector]
    public bool isBot = false;
    [HideInInspector]
    public PUNRoomUtils punRoomUtils;

    [SerializeField] private GameObject lights;

    private PhotonView photonView;

    private void Awake()
    {
        transform.position = new Vector3(-100, 0, 0);
        if (CreateFriendlyMatch.AllowSpectators)
        {
            DoTurnOffLights();
        }
    }

    private async void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (isBot)
            {
                NFT nft = new NFT()
                {
                    imageUrl = GameState.botInfo.kittyUrl
                };

                await nft.GrabImage();
                playerCustomization.wrapper.SetActive(true);
                playerCustomization.SetTransientCat(nft.imageUrl, nft.ids);
            }
            else
            {
                ShowCat();
            }
            
            Reposition();
            return;
        }
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine && !isBot)
        {
            ShowCat();
            PUNRoomUtils.onPlayerJoined += OnPlayerJoined;
            SyncPlatformsBehaviour.onPlayersChanged += Reposition;
        }

        if (isBot)
        {
            playerCustomization.wrapper.SetActive(false);
        }

        Reposition();

        if (isBot)
        {
            NFT nft = new NFT()
            {
                imageUrl = GameState.botInfo.kittyUrl
            };

            await nft.GrabImage();
            playerCustomization.wrapper.SetActive(true);
            playerCustomization.SetTransientCat(nft.imageUrl, nft.ids);
        }
        
        if (CreateFriendlyMatch.AllowSpectators)
        {
            DoTurnOffLights();
        }
    }

    private void ShowCat()
    {
        var config = playerCustomization.SetCat(GameState.selectedNFT.imageUrl, GameState.selectedNFT.ids);
        string serializedConfig = JsonUtility.ToJson(config.GetSerializableObject());

        if (!PhotonNetwork.IsConnected)
        {
            SetCatStyle(GameState.selectedNFT.imageUrl, serializedConfig);
            return;
        }
        photonView.RPC("SetCatStyle", RpcTarget.Others, GameState.selectedNFT.imageUrl, serializedConfig);
    }

    private void OnDestroy()
    {
        PUNRoomUtils.onPlayerJoined -= OnPlayerJoined;
        SyncPlatformsBehaviour.onPlayersChanged -= Reposition;
    }

    protected void Reposition()
    {
        PlatformPose pose = SyncPlatformsBehaviour.Instance.GetMySeatPosition(photonView, isBot);
        transform.position = pose.pos;
        transform.localScale = pose.scale;

        //Set my seat on room props
        if (PhotonNetwork.IsConnected)
        {
            if ((photonView == null && !isBot) || photonView.IsMine)
            {
                punRoomUtils.AddPlayerCustomProperty("seat", "" + pose.seatIdx);
            }
        }

        if (CreateFriendlyMatch.AllowSpectators)
        {
            if (pose.seatIdx<=2)
            {
                TurnOffLights();
            }
            else
            {
                TurnOnLights();
            }
        }
        else
        {
            if (pose.seatIdx>1)
            {
                TurnOffLights();
            }
            else
            {
                TurnOnLights();
            }
        }
    }

    private void OnPlayerJoined(string nickname, string userId)
    {
        string serializedConfig = JsonUtility.ToJson(KittiesCustomizationService.GetCustomization(GameState.selectedNFT.imageUrl).GetSerializableObject());
        photonView.RPC("SetCatStyle", RpcTarget.Others, GameState.selectedNFT.imageUrl, serializedConfig);
    }

    public void TurnOffLights()
    {
        if (!PhotonNetwork.IsConnected)
        {
            DoTurnOffLights();
            return;
        }
        photonView.RPC(nameof(DoTurnOffLights), RpcTarget.AllBuffered);
    }
    
    public void TurnOnLights()
    {
        if (!PhotonNetwork.IsConnected)
        {
            DoTurnOnLights();
            return;
        }
        photonView.RPC(nameof(DoTurnOnLights), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void DoTurnOffLights()
    {
        lights.SetActive(false);
    }    
    
    [PunRPC]
    private void DoTurnOnLights()
    {
        lights.SetActive(true);
    }

    [PunRPC]
    public void SetCatStyle(string url, string configJson)
    {
        KittyCustomization customization = JsonUtility.FromJson<KittyCustomization.KittyCustomizationSerializable>(configJson).GetNonSerializable();
        playerCustomization.SetTransientCat(url, customization);
    }



}
