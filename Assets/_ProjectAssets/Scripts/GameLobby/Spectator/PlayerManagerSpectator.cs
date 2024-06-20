public class PlayerManagerSpectator : PlayerManager
{
    protected override bool AmIPlayer1Multiplayer()
    {
        return RoomStateManagerSpectator.IsMasterInSpectator;
    }
}
