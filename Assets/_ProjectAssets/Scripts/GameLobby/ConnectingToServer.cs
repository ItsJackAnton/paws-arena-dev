using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingToServer : MonoBehaviour
{
    private const string DEFAULT_KITTY = "https://webapiwithssl20230210160824.azurewebsites.net/download/files/blackKitty.svg";
    
    [SerializeField] private Button connect;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private GameObject failedToAuth;

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
        logText.text = "Waiting the connection with ICP Wallet to be approved...";
        JavaScriptManager.Instance.AuthenticateAbstract(HanleAuthResponse);
    }

    private void HanleAuthResponse(AuthResponse _auth)
    {
        if (!_auth.DidAuth)
        {
            failedToAuth.SetActive(true);
            return;
        }
        
        GameState.principalId = _auth.WalletAddress;
        FinishConnecting();
    }

    private void FinishConnecting()
    {
        logText.text = "Connection made!";
        FirebaseManager.Instance.Authenticate(GameState.principalId, SetupData);
    }

    private void SetupData()
    {
        DataManager.Instance.Setup(FinishSetup);
    }

    public void FinishSetup()
    {
        DataManager.Instance.PlayerData.Nft = (new NFT { imageUrl = DEFAULT_KITTY });
        SceneManager.Instance.LoadNftSelection();
    }
}
