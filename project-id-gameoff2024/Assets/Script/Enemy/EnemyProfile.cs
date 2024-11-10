using UnityEngine;

public class EnemyProfile : MonoBehaviour
{
    [SerializeField] private EnemyProfileSO profile;
    public string enemyName { get; set; }
    public float enemyHealth { get; set; }
    public float enemyDamage { get; set; }

    private void Start()
    {
        enemyName = profile.EnemyName;
        enemyHealth = profile.EnemyHealth;
        enemyDamage = profile.EnemyDamage;
    }

    public void DeductHealth(float damage)
    {
        enemyHealth -= damage;
        Debug.Log("Enemy (" + enemyName + ") health: " + enemyHealth);
    }

    public void EnemyStatus(GameObject gameObject)
    {
        if (enemyHealth <= 0)
        {
            Instantiate(profile.itemDrop, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
