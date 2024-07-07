using System.Collections;
using UnityEngine;

namespace com.colorfulcoding.AfterGame
{
    public class AfterGameCharacter : MonoBehaviour
    {
        public Animator animator;

        private void Start()
        {
            StartCoroutine(CharacterAnimationCoroutine());
        }

        private IEnumerator CharacterAnimationCoroutine()
        {
            int checkIfIWon = DidIWin();
            yield return new WaitForSeconds(1.5f);
            if (checkIfIWon < 0)
            {
                animator.SetBool("isDead", true);
            }
        }

        protected virtual int DidIWin()
        {
            return GameResolveStateUtils.CheckIfIWon(GameState.gameResolveState);
        }
    }
}
