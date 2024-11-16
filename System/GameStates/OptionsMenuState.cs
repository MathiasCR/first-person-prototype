using UnityEngine;

public class OptionsMenuState : GameState
{
    public OptionsMenuState(GameState previousState) : base(previousState)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Options Menu State from " + m_PreviousState);
        //Afficher l'interface du menu d'options
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Options Menu State");
        //Cacher l'interface du menu d'options
    }
}
