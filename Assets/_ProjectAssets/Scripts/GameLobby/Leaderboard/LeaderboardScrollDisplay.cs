using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScrollDisplay : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform topBounds;
    [SerializeField] private Transform botBounds;

    private void FixedUpdate()
    {
        float _clampedValue = Mathf.Clamp01(scrollbar.value);
        transform.position = Vector3.Lerp(botBounds.position, topBounds.position, _clampedValue);
    }
}
