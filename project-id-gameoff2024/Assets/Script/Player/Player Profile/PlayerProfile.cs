using UnityEngine;

public class PlayerProfile : MonoBehaviour
{

    public PlayerProfileSO profile;
    private PlayerCombatCamera playerCombatCam;
    public bool isDefending { get; set; }
    public float playerHealth { get; set; }
    public float playerStamina { get; set; }

    private void Awake()
    {
        playerCombatCam = GetComponent<PlayerCombatCamera>();
    }

    private void Start()
    {
        playerHealth = profile.PlayerHealth;
        playerStamina = profile.PlayerStamina;
    }

    private void Update()
    {
        IncreaseStamina();
        Init();
    }

    public void Init()
    {
        GameManager.Instance.uIManager.playerHealthTxt.SetText("{0:1}", playerHealth);
        GameManager.Instance.uIManager.playerStaminaTxt.SetText("{0:1}", playerStamina);
    }

    public void DeductHealth(float damage)
    {
        if (isDefending)
            return;

        if (playerHealth >= 0)
        {
            playerHealth -= damage;
            StartCoroutine(playerCombatCam.CameraShake(new CameraCombatInfo(0.25f, 0.025f, Vector3.zero)));

            if(playerHealth < 0)
            {
                playerHealth = 0;
            }
        }
    }

    public void DeductStamina(float cost)
    {
        if(playerStamina >= 0)
        {
            playerStamina -= cost;

            if(playerStamina < 0)
            {
                playerStamina = 0;
            }
        }
    }

    private void IncreaseStamina()
    {
        if (isDefending)
            return;

        if (playerStamina < profile.PlayerStamina)
        {
            playerStamina += Time.deltaTime;
        }
        else
        {
            playerStamina = profile.PlayerStamina;
        }
    }
}
