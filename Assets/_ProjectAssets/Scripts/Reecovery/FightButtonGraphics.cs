using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FightButtonGraphics : MonoBehaviour
{
    [SerializeField] private Image fightImage;
    [SerializeField] private Sprite normalFightSprite;
    [SerializeField] private Sprite injuredFightSprite;
    
    private void Start()
    {
        StartCoroutine(FightButtonGraphicsRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FightButtonGraphicsRoutine()
    {
        while (gameObject.activeSelf)
        {
            fightImage.sprite = DataManager.Instance.PlayerData.CanFight ? normalFightSprite : injuredFightSprite;
            yield return new WaitForSeconds(2);
        }
    }
}
