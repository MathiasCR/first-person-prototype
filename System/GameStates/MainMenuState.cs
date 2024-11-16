using UnityEngine;

public class MainMenuState : GameState
{
    public MainMenuState(GameState previousState) : base(previousState)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Main Menu State from " + m_PreviousState);
        //Afficher l'interface du menu principal
    }

    public override void UpdateState()
    {
        //Animation du bg ?
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Main Menu State");
        //Cacher l'interface du menu principal
    }
}
