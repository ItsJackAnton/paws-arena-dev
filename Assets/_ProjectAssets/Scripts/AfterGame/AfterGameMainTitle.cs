using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.colorfulcoding.AfterGame
{
    public class AfterGameMainTitle : MonoBehaviour
    {
        public GameObject winTitle;
        public GameObject loseTitle;
        public GameObject drawTitle;
        public Image bg;

        public TextMeshProUGUI totalCoinsValue;
        public TextMeshProUGUI deltaPoints;
        public Color winColor;
        public Color loseColor;
        public Color drawColor;

        public GameObject reasonText;

        [Header("Cat Stand")]
        public SpriteRenderer standGlow;

        private void Start()
        {
            int checkIfIWon;
            EventsManager.OnPlayedMatch?.Invoke();
            
            SaveKittyHealth();
            
            //If unexpected error happened, we override result type
            if (GameState.pointsChange.gameResultType == 0)
            {
                checkIfIWon = 0;
            }
            else
            {
                checkIfIWon = GameResolveStateUtils.CheckIfIWon(GameState.gameResolveState);
            }
            
            if (checkIfIWon > 0)
            {
                if (DataManager.Instance.PlayerData.CanFight)
                {
                    EventsManager.OnWonGameWithFullHp?.Invoke();
                }

                if (PlayerManager.HealthAtEnd<=10)
                {
                    EventsManager.OnWonWithHpLessThan10?.Invoke();
                }
                if (PlayerManager.HealthAtEnd<=20)
                {
                    EventsManager.OnWonWithHpLessThan20?.Invoke();
                }
                if (PlayerManager.HealthAtEnd<=30)
                {
                    EventsManager.OnWonWithHpLessThan30?.Invoke();
                }
                
                EventsManager.OnWonGame?.Invoke();
                winTitle.SetActive(true);
                bg.GetComponent<Image>().color = winColor;
                standGlow.color = winColor;
            }
            else if (checkIfIWon < 0)
            {
                EventsManager.OnLostGame?.Invoke();
                loseTitle.SetActive(true);
                bg.GetComponent<Image>().color = loseColor;
                standGlow.color = loseColor;
            }
            else
            {
                drawTitle.SetActive(true);
                bg.GetComponent<Image>().color = drawColor;
                standGlow.color = drawColor;
            }

            totalCoinsValue.text = "" + GameState.pointsChange.oldPoints;
            int _earnings = GameState.pointsChange.points;

            if (_earnings>0)
            {
                EventsManager.OnWonLeaderboardPoints?.Invoke(_earnings);
            }

            if (GameState.pointsChange.points != 0)
            {
                LeanTween.value(gameObject, 0, GameState.pointsChange.points, 2f).setOnUpdate(val =>
                {
                    totalCoinsValue.text = "" + Math.Floor(GameState.pointsChange.oldPoints + val);
                    deltaPoints.text = "+" + Math.Floor(val);
                }).setEaseInOutCirc().setDelay(1f).setOnComplete(() =>
                {
                }
                );
            }

            if (!string.IsNullOrEmpty(GameState.pointsChange.reason))
            {
                reasonText.SetActive(true);
                reasonText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameState.pointsChange.reason;
            }
        }

        private void SaveKittyHealth()
        {
            if (CreateFriendlyMatch.IsFriendly)
            {
                return;
            }
            
            float _maxHp = 100;
            float _minutesItWillTakeToRecover = (RecoveryHandler.RecoveryInMinutes / _maxHp) * (_maxHp - PlayerManager.HealthAtEnd);
            if (_minutesItWillTakeToRecover <= 1)
            {
                return;
            }

            DateTime _recoveryEnds = DateTime.UtcNow.AddMinutes(_minutesItWillTakeToRecover);
            DataManager.Instance.PlayerData.RecoveryEndDate = _recoveryEnds;
        }
    }
}