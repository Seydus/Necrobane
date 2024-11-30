using UnityEngine;

public class BossProfile : MonoBehaviour
{
    public BossProfileSO bossProfileSO;

    private float bossHealth;

    [Header("Attack Settings")]
    private float projectileDamage;
    private float bulletHellDamage;
    private float bossSpikeDamage;

    [Header("Summoning")]
    private float numberOfSpawns;
    private GameObject entityPrefab;

    private void Start()
    {
        bossHealth = bossProfileSO.bossHealth;
        projectileDamage = bossProfileSO.projectileDamage;
        bulletHellDamage = bossProfileSO.bulletHellDamage;
        bossSpikeDamage = bossProfileSO.bossSpikeDamage;
        numberOfSpawns = bossProfileSO.numberOfSpawns;
        entityPrefab = bossProfileSO.entityPrefab;
    }

    private void Update()
    {
        HealthState();
    }

    private void HealthState()
    {
        GameManager.Instance.uIManager.bossHealthSlider.value = bossHealth;

    }

    public void RestoreHealth(float heal)
    {
        if(bossHealth >= bossProfileSO.bossHealth)
        {
            bossHealth = bossProfileSO.bossHealth;
        }

        bossHealth += heal;
    }

    public void DeductHealth(float damage)
    {
        if(bossHealth <= 0)
        {
            Debug.Log("Boss dead");
            bossHealth = 0;
        }

        bossHealth -= damage;
    }
}
