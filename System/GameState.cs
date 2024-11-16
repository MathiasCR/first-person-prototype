public abstract class GameState
{
    protected GameState m_PreviousState;

    public GameState(GameState previousState)
    {
        this.m_PreviousState = previousState;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
