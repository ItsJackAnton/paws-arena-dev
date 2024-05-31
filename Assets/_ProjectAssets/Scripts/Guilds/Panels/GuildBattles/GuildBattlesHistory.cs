using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildBattlesHistory : MonoBehaviour
{
    [SerializeField] private List<Image> results;
    [SerializeField] private Sprite victory;
    [SerializeField] private Sprite lose;

    public void Setup(List<int> _history)
    {
        _history.Reverse();
        int _counter = 0;

        foreach (var _result in results)
        {
            _result.gameObject.SetActive(false);
        }
        
        foreach (var _outcome in _history)
        {
            if (_counter> results.Count-1)
            {
                break;
            }

            var _resultHolder = results[_counter];
            _resultHolder.sprite = _outcome == 1 ? victory : lose;
            _resultHolder.gameObject.SetActive(true);
            _counter++;
        }
    }
}
