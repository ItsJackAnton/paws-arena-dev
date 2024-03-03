using Anura.ConfigurationModule.Managers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BoomDaoWrapper;
using UnityEngine;
using UnityEngine.UI;

public class NFTSelection : MonoBehaviour
{
    public PagesBehaviour pages;

    public Transform playerPlatformParent;
    public GameObject playerPlatformPrefab;

    public GameObject nftButtonPrefab;
    public Transform nftButtonsParent;

    public NFTSelection_LoadingManager screenLoadingManager;

    private List<GameObject> nftButtons = new List<GameObject>();
    private GameObject playerPlatform;
    [SerializeField] private GameObject message;
    [SerializeField] private Button reload;
    [SerializeField] private Button reloadNoKitty;

    private int currentPage = 0;
    private int pageSize = 9;

    private List<NFT> currentNFTs = new List<NFT>();

    private void OnEnable()
    {
        reload.onClick.AddListener(RequestReload);
        reloadNoKitty.onClick.AddListener(RequestReload);
        InitNFTScreen();
        pages.OnClick += OnPageSelected;
        BoomDaoUtility.OnUpdatedNftsData += ReloadNfts;
        
    }

    private void OnDisable()
    {
        reload.onClick.RemoveListener(RequestReload);
        reloadNoKitty.onClick.RemoveListener(RequestReload);
        ClearShownNfts();
        if (playerPlatform != null)
        {
            Destroy(playerPlatform);
            playerPlatform = null;
        }


        pages.OnClick -= OnPageSelected;
        BoomDaoUtility.OnUpdatedNftsData -= ReloadNfts;
    }

    private void RequestReload()
    {
        BoomDaoUtility.Instance.ReloadNfts();
    }

    private void ReloadNfts()
    {
        ClearShownNfts();
        InitNFTScreen();
    }
    
    private void ClearShownNfts()
    {
        foreach (NFT nfts in currentNFTs)
        {
            Destroy(nfts.imageTex);
            nfts.imageTex = null;
        }
        currentNFTs.Clear();
    }

    public async void InitNFTScreen()
    {
        currentPage = 0;
        int maxPages = (int)Math.Floor((GameState.nfts.Count - 1) * 1.0 / pageSize);
        pages.SetNumberOfPages(maxPages + 1);
        await PopulateGridAsync();
        if (GameState.nfts.Count>0)
        {
            SelectNFT(0);
        }
        message.SetActive(GameState.nfts.Count==0);
    }

    private async void OnPageSelected(int idx)
    {
        currentPage = idx;
        await PopulateGridAsync();

    }

    private List<NFT> GetNFTs(int pageNr, int pageSize)
    {
        return GameState.nfts.Skip(pageNr * pageSize).Take(pageSize).ToList();
    }
    private async UniTask PopulateGridAsync()
    {
        screenLoadingManager.AddLoadingReason("Loading NFTs...");
        foreach (GameObject but in nftButtons)
        {
            Destroy(but);
        }

        nftButtons.Clear();

        foreach (NFT nfts in currentNFTs)
        {
            Destroy(nfts.imageTex);
            nfts.imageTex = null;
        }


        currentNFTs = GetNFTs(currentPage, pageSize);

        //Grab all images from internet
        List<UniTask> tasks = new List<UniTask>();
        int idx = 0;
        foreach (NFT nft in currentNFTs)
        {
            GameObject go = Instantiate(nftButtonPrefab, nftButtonsParent);
            nftButtons.Add(go);
            go.GetComponent<NFTImageButton>().SetLoadingState();
            tasks.Add(nft.GrabImage());
            idx++;
        }

        for (int i = currentNFTs.Count; i < 9; i++)
        {
            GameObject go = Instantiate(nftButtonPrefab, nftButtonsParent);
            nftButtons.Add(go);
        }
        await UniTask.WhenAll(tasks.ToArray());

        screenLoadingManager.StopLoadingReason("Loading NFTs...");

        //Attach to images
        idx = 0;
        foreach (NFT nft in currentNFTs)
        {
            nft.RecoveryEndDate = DateTime.MinValue;
            if (DataManager.Instance.PlayerData.IsKittyHurt(nft.imageUrl))
            {
                nft.RecoveryEndDate = DataManager.Instance.PlayerData.GetKittyRecoveryDate(nft.imageUrl);
            }
            
            nftButtons[idx].GetComponent<NFTImageButton>().SetTexture(nft.imageTex);
            nftButtons[idx].GetComponent<RecoveryHandler>().ShowRecovery(nft.RecoveryEndDate);
            nftButtons[idx].GetComponent<Button>().onClick.RemoveAllListeners();

            int crtIdx = idx;
            nftButtons[idx].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectNFT(crtIdx);
            });
            idx++;
        }

        SetSelectedNFTGraphics();
    }

    private void Update()
    {

    }

    private void LoadedNft(NFT nft, int index)
    {
        nftButtons[index].GetComponent<NFTImageButton>().SetTexture(nft.imageTex);
        nftButtons[index].GetComponent<Button>().onClick.RemoveAllListeners();
        nftButtons[index].GetComponent<Button>().onClick.AddListener(() => { SelectNFT(index); });
    }

    private void SelectNFT(int idx)
    {
        if (playerPlatform != null)
        {
            Destroy(playerPlatform);
        }

        if (ConfigurationManager.Instance.GameConfig.enableDevLogs)
        {
            Debug.Log("Selected " + currentNFTs[idx].imageUrl);
        }

        GameState.SetSelectedNFT(currentNFTs[idx]);

        playerPlatform = GameObject.Instantiate(playerPlatformPrefab, playerPlatformParent);
        playerPlatform.transform.localPosition = Vector3.zero;

        SetSelectedNFTGraphics();
    }

    private void SetSelectedNFTGraphics()
    {
        NFT selectedNFT = GameState.selectedNFT;
        for (int i = 0; i < nftButtons.Count; i++)
        {
            if (i >= currentNFTs.Count)
            {
                break;
            }
            if (currentNFTs[i] == null)
            {
                break;
            }
            if (selectedNFT != null && currentNFTs[i].imageUrl == selectedNFT.imageUrl)
            {
                nftButtons[i].GetComponent<NFTImageButton>().Select();
            }
            else
            {
                nftButtons[i].GetComponent<NFTImageButton>().Deselect();
            }
        }
    }
}
