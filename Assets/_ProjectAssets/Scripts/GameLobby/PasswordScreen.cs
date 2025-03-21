using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordScreen : MonoBehaviour
{
    public LobbyUIManager uiManager;

    public TMPro.TMP_InputField passwordInput;
    public GameObject startButton;
    public GameObject logText;

    public void OnSubmit()
    {
        if(passwordInput.text.ToUpper() == "PAWS")
        {
            StartCoroutine(WalletConnectionCoroutine());
        }
    }

    private IEnumerator WalletConnectionCoroutine()
    {
        startButton.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        logText.SetActive(true);

        var text = logText.GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "Pretending we connect to ICP Wallet...";

        GameState.walletId = "asd";

        yield return new WaitForSeconds(1f);
        text.text = "Pretending we get info from server...";

        uiManager.OpenNFTSelectionScreen();
    }
}
