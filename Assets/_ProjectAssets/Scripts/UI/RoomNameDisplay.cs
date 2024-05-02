using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomNameDisplay : MonoBehaviour
{
    [SerializeField] private Button copyName;
    private TextMeshProUGUI roomNameDisplay;

    private void OnEnable()
    {
        copyName.onClick.AddListener(CopyName);
        PUNRoomUtils.onPlayerJoined += DisableView;
        PUNRoomUtils.onPlayerLeft += ShowView;
    }

    private void OnDisable()
    {
        copyName.onClick.RemoveListener(CopyName);
        PUNRoomUtils.onPlayerJoined -= DisableView;
        PUNRoomUtils.onPlayerLeft -= ShowView;
    }

    private void DisableView(string _arg1, string _arg2)
    {
        copyName.gameObject.SetActive(false);
        roomNameDisplay.text = string.Empty;
    }

    private void ShowView()
    {
        Start();
    }

    private void CopyName()
    {
        Utilities.DoCopyToClipboard(PhotonNetwork.CurrentRoom.Name);
    }

    private void Awake()
    {
        copyName.gameObject.SetActive(false);
        roomNameDisplay = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            roomNameDisplay.text = string.Empty;
            return;
        }

        if (PhotonNetwork.CurrentRoom.IsVisible)
        {
            roomNameDisplay.text = string.Empty;
            return;
        }

        roomNameDisplay.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        copyName.gameObject.SetActive(true);
    }
}
