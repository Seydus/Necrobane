using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public PlayerManager playerManager;
    public UIManager uIManager;

    public bool GameState { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        HandleGameState();
        RestartGame();
    }

    private void HandleGameState()
    {
        if (playerManager.PlayerProfile.playerHealth <= 0)
        {
            uIManager.gameOverUI.SetActive(true);
            GameState = false;
        }
    }

    private void RestartGame()
    {
        if(!GameState)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
