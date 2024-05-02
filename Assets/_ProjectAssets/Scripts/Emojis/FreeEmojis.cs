using System;
using System.Collections.Generic;
using BoomDaoWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeEmojis : MonoBehaviour
{
   private const string CLAIM_FREE_EMOJI = "claimFreeEmoji";
   [SerializeField] private GameObject holder;
   [SerializeField] private Button claim;
   [SerializeField] private GameObject rewardHolder;
   [SerializeField] private TextMeshProUGUI rewardName;
   [SerializeField] private Image rewardImage;
   [SerializeField] private Button close;

   private void OnEnable()
   {
      claim.onClick.AddListener(Claim);
      close.onClick.AddListener(Close);
   }

   private void OnDisable()
   {
      claim.onClick.RemoveListener(Claim);
      close.onClick.RemoveListener(Close);
   }

   private void Claim()
   {
      claim.interactable = false;
      BoomDaoUtility.Instance.ExecuteAction(CLAIM_FREE_EMOJI, OnClaimedNewEmoji);
   }

   private void Close()
   {
      holder.SetActive(false);
   }

   private void OnClaimedNewEmoji(List<ActionOutcome> _outcomes)
   {
      EmojiSO _emojiSo = null;
      foreach (var _outcome in _outcomes)
      {
         _emojiSo = EmojiSO.Get(_outcome.Name);
         if (_emojiSo==null)
         {
            continue;
         }
         break;
      }

      rewardName.text = _emojiSo.Name;
      rewardImage.sprite = _emojiSo.Sprite;
      rewardHolder.transform.localScale = Vector3.zero;
      rewardHolder.SetActive(true);
      rewardHolder.transform.LeanScale(Vector3.one, 1);
   }

   private void Start()
   {
      if (DataManager.Instance.PlayerData.HasClaimedFreeEmoji)
      {
         return;
      }

      if (DateTime.UtcNow > new DateTime(2024,5,15,0,0,0,DateTimeKind.Utc))
      {
         return;
      }
      
      holder.SetActive(true);
   }
}
