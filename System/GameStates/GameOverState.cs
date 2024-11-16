using UnityEngine;

public class GameOverState : GameState
{
    private float m_Timer;

    public GameOverState(GameState previousState) : base(previousState)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Game Over State from " + m_PreviousState);
        m_Timer = 0.0f;
    }

    public override void UpdateState()
    {
        m_Timer += Time.deltaTime;

        if (Input.anyKeyDown && m_Timer > 10f)
        {
            GameManager.Instance.SetGameState(new PlayingState(this));
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Game Over State");
        GameManager.Instance.PlayerManager.PlayerHUD.SetDeathScreen(false);
    }
}
