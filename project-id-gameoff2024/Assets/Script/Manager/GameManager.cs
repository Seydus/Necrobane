using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public PlayerManager playerManager;
    public UIManager uIManager;
    public BossController bossController;
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
        SetBossFightState();
    }

    private void SetBossFightState()
    {
        if(bossController)
        {
            EnemyManager.Instance.bossController = bossController;
        }
    }

    private void HandleGameState()
    {
        if (playerManager == null)
            return;

        if (playerManager.PlayerProfile.playerHealth <= 0)
        {
            uIManager.playerHealthTxt.text = "" + playerManager.PlayerProfile.playerHealth;
            uIManager.playerStaminaTxt.text = "" + playerManager.PlayerProfile.playerStamina;
            uIManager.gameOverUI.SetActive(true);
            GameState = false;

            playerManager.PlayerProfile.playerHealth = 0;
            AkSoundEngine.SetState("Player", "Dead");
            AkSoundEngine.PostEvent("Stop_Ambi", gameObject);
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
