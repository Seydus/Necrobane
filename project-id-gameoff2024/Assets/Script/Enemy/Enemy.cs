using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProfileSO profile;

    [Header("Status")]
    [SerializeField] protected bool enableRoaming = true;

    [Header("Enemy Roaming")]
    [SerializeField] protected float minRoamWaitTime = 2f;
    [SerializeField] protected float maxRoamWaitTime = 3f;
    [SerializeField] protected float roamDetectionRadius = 12f;
    [SerializeField] protected float maxRoamDistance = 5f;
    [SerializeField] protected float roamDirectionChangeChance = 0.3f;
    [SerializeField] protected Transform groundPos;
    [SerializeField] protected NavMeshSurface navMeshSurface;

    [Header("Enemy Detection")]
    [SerializeField] protected float detectRadius = 6f;
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] protected float engageCooldownDuration = 0.3f;
    [SerializeField] protected float disengageCooldownDuration = 0.2f;

    [Header("Enemy Combat")]
    [SerializeField] protected float attackDelay = 1;
    [SerializeField] protected float sphereRadius = 0.4f;
    [SerializeField] protected float maxDistance = 0.9f;
    [SerializeField] protected LayerMask combatLayer;
    [SerializeField] protected float rotateSpeed = 0.7f;
    public AK.Wwise.Event HitPlayer;

    [Header("Enemy Stats")]
    public string EnemyName { get; set; }
    public float EnemyHealth { get; set; }
    public float EnemyDamage { get; set; }

    [Header("Others")]
    protected NavMeshAgent _NavMeshAgent;

    public void Awake()
    {
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        EnemyName = profile.EnemyName;
        EnemyHealth = profile.EnemyHealth;
        EnemyDamage = profile.EnemyDamage;
    }

    public void Update()
    {
        EnemyStatus(gameObject);
    }

    public void DeductHealth(float damage)
    {
        EnemyHealth -= damage;
        Debug.Log("Enemy (" + EnemyName + ") health: " + EnemyHealth);
    }

    public void EnemyStatus(GameObject gameObject)
    {
        if (EnemyHealth <= 0)
        {
            Instantiate(profile.itemDrop, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    public void SetEnemyForce(Vector3 direction, float currentForce)
    {
        Debug.Log(direction + " and " + currentForce);
        // myBody.AddForce(direction * currentForce * Time.deltaTime, ForceMode.Impulse);
    }
}
