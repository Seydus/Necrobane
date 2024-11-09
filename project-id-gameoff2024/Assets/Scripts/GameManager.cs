using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    [SerializeField] private PlayerController playerController;

    [Header("State")]
    public bool PlayerState { get; private set; } = true;

    public bool GameState { get; private set; } = true;


    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;

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
        if(playerController.health <= 0)
        {
            gameOverUI.SetActive(true);
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
