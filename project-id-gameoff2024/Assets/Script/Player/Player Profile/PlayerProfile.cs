using UnityEngine;

public class PlayerProfile : MonoBehaviour
{

    [SerializeField] private PlayerProfileSO profile;
    public float playerHealth { get; set; }
    public float playerStamina { get; set; }

    private void Start()
    {
        playerHealth = profile.PlayerHealth;
        playerStamina = profile.PlayerStamina;
    }

    private void Update()
    {
        IncreaseStamina();
    }

    public void Init()
    {
        GameManager.Instance.uIManager.playerHealthTxt.SetText("{0:1}", playerHealth);
        GameManager.Instance.uIManager.playerStaminaTxt.SetText("{0:1}", playerStamina);
    }

    public void DeductHealth(float damage)
    {
        playerHealth -= damage;
    }

    public void DeductStamina(float cost)
    {
        playerStamina -= cost;
    }

    private void IncreaseStamina()
    {
        if(playerStamina < profile.PlayerStamina)
        {
            playerStamina += Time.deltaTime;
        }
        else
        {
            playerStamina = profile.PlayerStamina;
        }
    }
}
