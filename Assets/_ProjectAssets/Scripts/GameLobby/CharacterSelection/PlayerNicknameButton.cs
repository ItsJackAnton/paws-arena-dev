using Photon.Pun;
using UnityEngine;

public class PlayerNicknameButton : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI nicknameText;
    [SerializeField] private InputModal inputModal;

    private void Start()
    {
        string _nickname = DataManager.Instance.PlayerData.Username;
        nicknameText.text = "";
        if (!string.IsNullOrEmpty(_nickname.Trim()))
        {
            SetPlayerName(_nickname);
        }
        else
        {
            EnableEdit(false);
        }
    }

    private void EnableEdit(bool _isCancelable)
    {
        inputModal.Show("Nickname", "Nickname", _isCancelable, SaveNewName);
    }
    
    private void SaveNewName(string _nickname)
    {
        inputModal.ManageButton(true);
        SetPlayerName(_nickname);
    }

    private void SetPlayerName(string _newName)
    {
        GameState.nickname = PhotonNetwork.NickName = nicknameText.text = _newName;
        DataManager.Instance.PlayerData.Username = _newName;
        SendNewNicknameToServer(_newName);
        inputModal.Hide();
    }

    private async void SendNewNicknameToServer(string _nickname)
    {
        try
        {
            await NetworkManager.POSTRequest("/user/nickname", $"\"{_nickname}\"", null,null,true);
        }
        catch
        {
            // ignored
        }
    }
}
