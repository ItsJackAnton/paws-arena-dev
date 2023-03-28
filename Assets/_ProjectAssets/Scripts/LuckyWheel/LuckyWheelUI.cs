using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckyWheelUI : MonoBehaviour
{
    [SerializeField] GameObject playerPlatform;
    [SerializeField] LuckyWheel luckyWheel;
    [SerializeField] LuckyWheelClaimDisplay rewardDisplay;
    [SerializeField] Button respinButton;
    [SerializeField] Button claimButton;

    [SerializeField] decimal respinPrice;
    LuckyWheelRewardSO choosenReward;

    bool requestedToSeeReward = false;

    public void RequestReward()
    {
        //TODO ask server for the random reward id
        int _rewardId = 5;
        choosenReward = LuckyWheelRewardSO.Get(_rewardId);
        if (requestedToSeeReward)
        {
            Setup();
        }
    }

    public void ShowReward()
    {
        requestedToSeeReward = true;
        if (choosenReward == null)
        {
            return;
        }

        Setup();
    }

    void Setup()
    {
        playerPlatform.SetActive(false);
        gameObject.SetActive(true);

        respinButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);

        claimButton.onClick.AddListener(ClaimReward);
        respinButton.onClick.AddListener(Respin);
        SpinWheel();
    }

    private void OnDisable()
    {
        claimButton.onClick.RemoveListener(ClaimReward);
        respinButton.onClick.RemoveListener(Respin);
    }

    void ClaimReward()
    {
        Claim(choosenReward);
        //TODO tell server that client claimed reward
    }

    void Claim(LuckyWheelRewardSO _reward)
    {
        //todo implement me claim reward
        switch (_reward.Type)
        {
            case LuckyWheelRewardType.Lime:
                break;
            case LuckyWheelRewardType.Green:
                break;
            case LuckyWheelRewardType.Blue:
                break;
            case LuckyWheelRewardType.Purple:
                break;
            case LuckyWheelRewardType.Orange:
                break;
            case LuckyWheelRewardType.Gift:
                break;
            default:
                throw new System.Exception("Dont know how to reward reward type: " + _reward.Type);
        }

        rewardDisplay.Setup(_reward);
    }

    void Respin()
    {
        respinButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);
        choosenReward = null;
        RequestReward();
    }

    void SpinWheel()
    {
        luckyWheel.Spin(EnableButtons, choosenReward);
    }

    void EnableButtons()
    {
        StartCoroutine(ShowRewardAnimationRoutine());
    }

    IEnumerator ShowRewardAnimationRoutine()
    {
        yield return new WaitForSeconds(1);
        claimButton.gameObject.SetActive(true);
        respinButton.gameObject.SetActive(true);
    }

    public void Close()
    {
        playerPlatform.SetActive(true);
        gameObject.SetActive(false);
    }
}
