using Photon.Pun;
using UnityEngine;

public class GameplayPVPLevelBar : GameplayLevelBarBase
{
   
    [SerializeField] private bool isForMasterClient;

    private PhotonView photonView;

    private bool isMine => (PhotonNetwork.IsMasterClient&& isForMasterClient)||(!PhotonNetwork.IsMasterClient&&!isForMasterClient);

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    private void OnEnable()
    {
        if (isMine)
        {
            PlayerData.OnUpdatedExp += TellOpponentThatIEarnedExp;
        }
    }

    private void OnDisable()
    {
        if (isMine)
        {
            PlayerData.OnUpdatedExp -= TellOpponentThatIEarnedExp;
        }
    }

    private void Start()
    {
        if (isMine)
        {
            TellOpponentMyLevel();
            ShowMy();
        }
    }

    private void TellOpponentThatIEarnedExp()
    {
        ShowMy();
        TellOpponentMyLevel();
    }
    
    private void ShowMy()
    {
        levelDisplay.text = DataManager.Instance.PlayerData.Level.ToString();
        levelBar.fillAmount = (float)DataManager.Instance.PlayerData.ExperienceOnCurrentLevel / DataManager.Instance.PlayerData.ExperienceForNextLevel;
    }

    private void TellOpponentMyLevel()
    {
        double _experience=0;
        if (DataManager.Instance.GameData.IsSeasonActive)
        {
            _experience = DataManager.Instance.PlayerData.Experience;
        }
        photonView.RPC(nameof(TellOpponentMyExp),RpcTarget.Others,_experience);
    }
    

    [PunRPC]
    private void TellOpponentMyExp(double _experience)
    {
        Debug.Log("Opponents experience: "+_experience);
        ShowProgress(_experience);
    }
}
