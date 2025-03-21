using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        logText.text = "Waiting the connection with ICP Wallet to be approved...";
        if (Application.isEditor)
        {
            GameState.principalId = "UnityEditor123asdK";
            FinishConnecting();
        }
        else
        {
            //todo fix me Abstract
            // var _loginDataResult = BoomDaoUtility.Instance.GetLoginData;
            // var _loginDataAsOk = _loginDataResult.AsOk();
            //
            // GameState.principalId = _loginDataAsOk.principal;
        }
    }

    private void FinishConnecting()
    {
        logText.text = "Connection made!";
        DataManager.Instance.Setup();
        DataManager.Instance.PlayerData.Nft = (new NFT { imageUrl = DEFAULT_KITTY });
        SceneManager.Instance.LoadNftSelection();
    }
}
