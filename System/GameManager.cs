using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static readonly float GRAVITY = 9.81f;
    public static readonly float MINIMUM_CROUCH_HEIGHT = 6.2f;
    public static readonly float RELIEF_DIFFERENCE = 1f;

    [SerializeField] private GameObject m_Player;
    public PlayerManager PlayerManager { get => m_PlayerManager; }
    public GameState PreviousGameState { get => m_PreviousGameState; }
    public GameState CurrentGameState { get => m_CurrentGameState; }


    private GameState m_PreviousGameState;
    private GameState m_CurrentGameState;
    private PlayerManager m_PlayerManager;

    private void Awake()
    {
        Instance = this;

        if (m_Player.TryGetComponent(out PlayerManager playerManager))
        {
            m_PlayerManager = playerManager;
        }
    }

    private void Start()
    {
        SetGameState(new PlayingState(null));
    }

    private void Update()
    {
        if (m_CurrentGameState != null)
        {
            m_CurrentGameState.UpdateState();
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return m_Player.transform.position;
    }

    public void SetGameState(GameState gameState)
    {
        if (m_CurrentGameState != null)
        {
            m_CurrentGameState.ExitState();
            m_PreviousGameState = m_CurrentGameState;
        }

        m_CurrentGameState = gameState;
        m_CurrentGameState.EnterState();
    }
}
