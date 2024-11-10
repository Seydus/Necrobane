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

    public void Init()
    {
        GameManager.Instance.uIManager.playerHealthTxt.text = "" + playerHealth;
        GameManager.Instance.uIManager.playerStaminaTxt.text = "" + playerStamina;
    }

    public void DeductHealth(float damage)
    {
        playerHealth -= damage;
    }

    public void DeductStamina(float cost)
    {
        playerStamina -= cost;
    }
}
