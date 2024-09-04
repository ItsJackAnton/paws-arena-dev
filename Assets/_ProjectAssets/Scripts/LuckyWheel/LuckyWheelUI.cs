using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BoomDaoWrapper;

public class LuckyWheelUI : MonoBehaviour
{
    private const string BATTLE_WON_ACTION_KEY = "battle_outcome_won";

    public static Action OnClaimed;
    
    [SerializeField] private GameObject playerPlatform;
    [SerializeField] private LuckyWheel luckyWheel;
    [SerializeField] private LuckyWheelClaimDisplay rewardDisplay;
    // [SerializeField] private Button respinButton;
    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject insuficiantSnacksForRespin;
    [SerializeField] private TextMeshProUGUI insuficiantSnacksText;
    [SerializeField] private EquipmentsConfig equipments;
    [SerializeField] private bool CloseOnClaim;
    [SerializeField] private BuyMilk buyMilk;
    private LuckyWheelRewardSO choosenReward;
    public static EquipmentData EquipmentData = null;

    private bool requestedToSeeReward;
    private double currentRespinPrice;

    public void RequestReward()
    {
        List<ActionParameter> _parameters = new()
        {
            new ActionParameter { Key = PlayerData.EARNED_XP_KEY, Value = DamageDealingDisplay.XpEarned.ToString()}
        };
        BoomDaoUtility.Instance.ExecuteActionWithParameter(BATTLE_WON_ACTION_KEY,_parameters, OnGotRewards);
    }

    public void RequestRewardChallenges()
    {
        BoomDaoUtility.Instance.ExecuteAction(ChallengesManager.CHALLENGES_REWARD_LUCKY_SPIN, OnGotRewards);
    }
    
    private void OnGotRewards(List<ActionOutcome> _rewards)
    {
        if (_rewards==null)
        {
            return;
        }

        foreach (var _reward in _rewards)
        {
            switch (_reward.Name)
            {
                case PlayerData.COMMON_SHARD:
                case PlayerData.UNCOMMON_SHARD:
                case PlayerData.RARE_SHARD:
                case PlayerData.EPIC_SHARD:
                case PlayerData.LEGENDARY_SHARD:
                case PlayerData.MILK_BOTTLE:
                    choosenReward = LuckyWheelRewardSO.Get(_reward.Name);
                    break;
            }
        }

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

    private void Setup()
    {
        if (playerPlatform!=null)
        {
            playerPlatform.SetActive(false);
        }
        gameObject.SetActive(true);

        // respinButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);
        
        currentRespinPrice = DataManager.Instance.GameData.RespinPrice;
        SpinWheel();
    }

    private void OnEnable()
    {
        claimButton.onClick.AddListener(ClaimReward);
        // respinButton.onClick.AddListener(Respin);
    }

    private void OnDisable()
    {
        claimButton.onClick.RemoveListener(ClaimReward);
        // respinButton.onClick.RemoveListener(Respin);
    }

    private void ClaimReward()
    {
        Claim(choosenReward);
    }

    private void Claim(LuckyWheelRewardSO _reward)
    {
        rewardDisplay.Setup(_reward,CloseAfterClaim);
    }

    private void CloseAfterClaim()
    {
        if (CloseOnClaim)
        {
            gameObject.SetActive(false);
        }
        
        OnClaimed?.Invoke();
    }

    private void Respin()
    {
        //todo remove previous reward
        if (DataManager.Instance.PlayerData.Snacks< currentRespinPrice)
        {
            insuficiantSnacksForRespin.SetActive(true);
            insuficiantSnacksText.text = $"You don't have enaught Snacks.\n(takes {currentRespinPrice} for respin)";
            buyMilk.Setup();
            return;
        }

        // DataManager.Instance.PlayerData.Snacks -= currentRespinPrice;
        // respinButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);
        choosenReward = null;
        currentRespinPrice *= 2;
        RequestReward();
    }

    private void SpinWheel()
    {
        luckyWheel.Spin(EnableButtons, choosenReward);
    }

    private void EnableButtons()
    {
        StartCoroutine(ShowRewardAnimationRoutine());
    }

    private IEnumerator ShowRewardAnimationRoutine()
    {
        yield return new WaitForSeconds(1);
        claimButton.gameObject.SetActive(true);
        // respinButton.gameObject.SetActive(true);
    }

    public void Close()
    {
        playerPlatform.SetActive(true);
        gameObject.SetActive(false);
    }
}
