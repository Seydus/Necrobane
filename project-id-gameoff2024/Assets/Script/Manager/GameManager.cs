using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public PlayerManager playerManager;
    public UIManager uIManager;

    private bool switchDrone;
    public GameObject player;
    public GameObject drone;

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

        if(Input.GetKeyDown(KeyCode.T))
        {
            switchDrone = !switchDrone;
        }

        if(switchDrone)
        {
            player.SetActive(false);
            drone.SetActive(true);
        }
        else
        {
            player.SetActive(true);
            drone.SetActive(false);
        }
    }

    private void HandleGameState()
    {
        if (playerManager == null)
            return;

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
