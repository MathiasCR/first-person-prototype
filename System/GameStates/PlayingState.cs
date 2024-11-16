using UnityEngine;

public class PlayingState : GameState
{
    public PlayingState(GameState previousState) : base(previousState)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Playing State from " + m_PreviousState);

        //Le joueur peut manipuler le personnage
        GameManager.Instance.PlayerManager.ChangeControls(true);
    }

    public override void UpdateState()
    {
        //Enregistrer le temps passé sur le jeu

        //Verifier si le joueur est mort
        if (GameManager.Instance.PlayerManager.PlayedDead)
        {
            OnPlayedDead();
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Playing State");

        //Enlever les contrôles du joueur
        GameManager.Instance.PlayerManager.ChangeControls(false);
    }

    private void OnPlayedDead()
    {
        GameManager.Instance.SetGameState(new GameOverState(this));
    }
}
