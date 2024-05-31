using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class ConnectingToServer : MonoBehaviour
{
    public const string DEFAULT_KITTY = "https://webapiwithssl20230210160824.azurewebsites.net/download/files/blackKitty.svg";
    [SerializeField] private Button connect;
    [SerializeField] private TextMeshProUGUI logText;

    private void OnEnable()
    {
        connect.onClick.AddListener(Connect);
    }

    private void OnDisable()
    {
        connect.onClick.RemoveListener(Connect);
    }

    private void Connect()
    {
        BoomDaoUtility.Instance.Login(FinishConnecting);
        logText.text = "Waiting the connection with ICP Wallet to be approved...";
    }

    private void FinishConnecting()
    {
        logText.text = "Connection made!";
        SetupNftData();

        var _loginDataResult = BoomDaoUtility.Instance.GetLoginData;
        var _loginDataAsOk = _loginDataResult.AsOk();

        GameState.principalId = _loginDataAsOk.principal;
        DataManager.Instance.Setup();
        ChallengesManager.Instance.Setup();
        
        SceneManager.Instance.LoadNftSelection();
    }
    
    private void SetupNftData()
    {
        UpdateNfts();
    }

    private void MockNfts()
    {
        if (Application.isEditor)
        {
            GameState.nfts.Add(new NFT()
            {
                imageUrl = "https://rw7qm-eiaaa-aaaak-aaiqq-cai.raw.icp0.io/?tokenid=45rna-aykor-uwiaa-aaaaa-cqace-eaqca-aadhw-q"
            });  
        }
    }

    public static void ReloadNfts()
    {
        GameState.nfts.Clear();
        UpdateNfts();
    }

    private static void UpdateNfts()
    {
        var _nftCollectionsResult = BoomDaoUtility.Instance.GetNftData;
        if (_nftCollectionsResult.IsErr)
        {
            Debug.Log($"{_nftCollectionsResult.AsErr()} "+ nameof(ConnectingToServer));
            return;
        }

        var _nftCollectionsAsOk = _nftCollectionsResult.AsOk();

        foreach (var _keyValue in _nftCollectionsAsOk.elements)
        {
            var _collection = _keyValue.Value;

            if (_collection.canisterId != BoomDaoUtility.ICK_KITTIES)
            {
                continue;
            }
            
            foreach (var _token in _collection.tokens)
            {
                GameState.nfts.Add(new NFT { imageUrl = _token.url });
            }
            break;
        }
        
        if (GameState.nfts.Count==0)
        {
            GameState.nfts.Add(new NFT()
            {
                imageUrl = DEFAULT_KITTY
            });
        }
    }

    [Button()]
    private void Test()
    {
        Debug.Log(Utilities.NanosecondsToDateTime(138567934127000000));
    }
}
