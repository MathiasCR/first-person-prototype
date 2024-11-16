using UnityEngine;

public class PauseState : GameState
{
    public PauseState(GameState previousState) : base(previousState)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Pause State from " + m_PreviousState);
        //Afficher le menu pause
    }
    public override void UpdateState()
    {
        //Mettre en pause le jeu
    }
    public override void ExitState()
    {
        Debug.Log("Exiting Pause State");
        //Enlever le menu pause
    }
}
