using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyProfileSO enemyProfile;

    [Header("Status")]
    [SerializeField] protected bool enableRoaming = true;

    [Header("Enemy Roaming")]
    [SerializeField] protected float roamingMoveSpeed = 10f;
    [SerializeField] protected float minRoamWaitTime = 2f;
    [SerializeField] protected float maxRoamWaitTime = 3f;
    [SerializeField] protected float roamDetectionRadius = 12f;
    [SerializeField] protected float minRoamDistance = 3f;
    [SerializeField] protected float maxRoamDistance = 5f;
    [SerializeField] protected float roamDirectionChangeChance = 0.3f;
    [SerializeField] protected Transform groundPos;
    [SerializeField] protected NavMeshSurface navMeshSurface;
    [SerializeField] protected float roamingRotateSpeed = 1f;

    [Header("Enemy Detection")]
    [SerializeField] protected float detectRadius = 6f;
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] protected float engageCooldownDuration = 0.3f;
    [SerializeField] protected float disengageCooldownDuration = 0.2f;

    [Header("Enemy Combat")]
    [SerializeField] protected float sphereRadius = 0.4f;
    [SerializeField] protected float maxDistance = 0.9f;
    [SerializeField] protected LayerMask combatLayer;
    [SerializeField] protected float rotateSpeed = 0.7f;
    [SerializeField] protected float attackDelay = 0.5f;
    public bool isAttacking { get; set; }
    public bool EnemyHit { get; set; }
    public AK.Wwise.Event HitPlayer;

    private bool isTouchPlayer;
    [SerializeField] private float enemyTouchDamageCooldown;
    private float currentTouchDamageCooldown;


    [Header("SphereCast")]
    protected Ray sphereRay;
    protected RaycastHit enemyHitInfo;

    [Header("Enemy Stats")]
    public string EnemyName { get; set; }
    public float EnemyHealth { get; set; }
    public float EnemyDamage { get; set; }

    [Header("Others")]
    protected NavMeshAgent _NavMeshAgent;

    public virtual void Awake()
    {
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void Start()
    {
        EnemyName = enemyProfile.EnemyName;
        EnemyHealth = enemyProfile.EnemyHealth;
        EnemyDamage = enemyProfile.EnemyDamage;
    }

    public virtual void Update()
    {
        EnemyStatus(gameObject);
        UpdateTouchedEnemy();
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
            Instantiate(enemyProfile.itemDrop, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    public void SetEnemyForce(Vector3 direction, float currentForce)
    {
        Debug.Log(direction + " and " + currentForce);
        // myBody.AddForce(direction * currentForce * Time.deltaTime, ForceMode.Impulse);
    }

    private void UpdateTouchedEnemy()
    {
        if (currentTouchDamageCooldown <= 0)
        {
            isTouchPlayer = true;
            currentTouchDamageCooldown = enemyTouchDamageCooldown;
        }
        else
        {
            currentTouchDamageCooldown -= Time.deltaTime;
        }
    }

    public void TouchedEnemy(Transform player)
    {
        if (isTouchPlayer)
        {
            isTouchPlayer = false;
            player.GetComponent<PlayerProfile>().DeductHealth(EnemyDamage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log(collision.collider);
            TouchedEnemy(collision.transform);
        }
    }
}
