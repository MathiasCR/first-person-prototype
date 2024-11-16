public abstract class StateMachine
{
    protected Enemy enemy;

    public StateMachine(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
