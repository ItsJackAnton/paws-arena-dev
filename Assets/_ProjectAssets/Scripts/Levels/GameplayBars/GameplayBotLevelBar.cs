using System;

public class GameplayBotLevelBar : GameplayLevelBarBase
{
    private void Start()
    {
        Show();
    }
    
    private void Show()
    {
        float _experience = 0;
        if (DataManager.Instance.GameData.HasSeasonStarted && !DataManager.Instance.GameData.HasSeasonEnded)
        {
            DateTime _startDate = DataManager.Instance.GameData.SeasonStarts;
            DateTime _endDate = DataManager.Instance.GameData.SeasonEnds;
            TimeSpan _totalSeasonDuration = _endDate - _startDate;
            TimeSpan _timePassed = DateTime.UtcNow - _startDate;
            float _amountOfRewards = DataManager.Instance.GameData.SeasonRewards.Count;
            float _totalAmountOfExperienceRequired = _amountOfRewards * 100;

            double _proportionOfSeasonPassed = Math.Min(1.0, _timePassed.TotalDays / _totalSeasonDuration.TotalDays);
            _experience = (float)(_totalAmountOfExperienceRequired * _proportionOfSeasonPassed);
        }

        _experience *= GameState.botInfo.SeasonProgressMultiplayer;
        ShowProgress(_experience);
    }
}
