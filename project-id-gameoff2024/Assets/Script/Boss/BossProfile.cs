using UnityEngine;

public class BossProfile : MonoBehaviour
{
    public BossProfileSO bossProfileSO;
    [SerializeField] private GameObject winningUI;

    public float bossHealth { get; private set; }

    [Header("Attack Settings")]
    public float projectileDamage { get; private set; }
    public float bulletHellDamage { get; private set; }
    public float bossSpikeDamage { get; private set; }

    [Header("Summoning")]
    public float numberOfSpawns { get; private set; }
    public GameObject entityPrefab { get; private set; }

    [Header("Others")]
    private BossController bossController;

    private void Awake()
    {
        bossController = GetComponent<BossController>();
    }

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
            bossController.currentPhase = 4;
            bossHealth = 0;

            winningUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.GameState = false;
        }

        bossHealth -= damage;

        bossController.State();
    }
}
