public class BotTurnState : IRoomState
{
    public void Init(RoomStateManager context)
    {
        context.lastPlayerRound = 1;
    }

    public void OnExit()
    {

    }
}
