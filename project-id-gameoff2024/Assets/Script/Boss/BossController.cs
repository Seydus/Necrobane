using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.AI.Navigation;

[System.Serializable]
public class MethodData
{
    public string Name { get; set; }
    public System.Action Method { get; set; }
    public IEnumerator Coroutine { get; set; }

    public MethodData(string name, System.Action method, IEnumerator coroutine)
    {
        Name = name;
        Method = method;
        Coroutine = coroutine;
    }
}

public class BossController : MonoBehaviour
{
    private int currentPhase = 0;
    [SerializeField] private List<MethodData> attackPatternList = new List<MethodData>();
    private bool isGetPattern;

    [Header("Projectile Attack")]
    [SerializeField] private float projectileDelayStart;
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject bossProjectilePrefab;
    [SerializeField] private float setProjectileSpeed;
    private float projectileSpeed;
    private bool isProjectile;

    [Header("Summon")]
    [SerializeField] private float setSummonCooldown;
    private float summonCooldown;

    [SerializeField] private float setNumberOfEnemySummon;
    private float numberOfEnemySummon;
    
    [SerializeField] private float enemySummonRadius;
    [SerializeField] private float minEnemyDistanceSummon = 5f;
    private bool isSummon;

    [SerializeField] private Transform enemySummonGroundPos;
    [SerializeField] private GameObject skeletonPrefab;

    [SerializeField] private int minEnemyCurrently;

    private List<Vector3> enemySummonPosition = new List<Vector3>();
    private NavMeshHit navHit;

    [Header("BulletHell")]
    [SerializeField] private float setBulletHellCooldown;
    [SerializeField] private float bulletHellSummonThreshold = 8f;
    [SerializeField] private float summonThresholdCooldownIncrease = 5f;
    private float bulletHellCooldown;

    private bool isBulletHell;
    private bool enableBulletHell;

    [Header("Spike Attack")]
    [SerializeField] private float setSpikeValue;
    private float spikeValue;
    [SerializeField] private Transform spikePos;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private float bossDistance;
    [SerializeField] private LayerMask playerLayer;

    private GameObject spikeObj;

    private bool isSpike;
    private bool enableSpikeAttack;
    private RaycastHit hitInfo;

    public GameObject player;

    [Header("NavMesh Settings")]
    [SerializeField] private float bossMoveSpeed;
    [SerializeField] private float bossRotationSpeed;
    [SerializeField] private float bossStoppingDistance;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("Others")]
    private BossAnimation bossAnimation;
    private BossBulletHellPattern bulletHellPattern;
    private BossProfile bossProfile;
    private Quaternion yAxisOnlyRotation;
    private Quaternion targetRotation;

    [Header("Events")]
    public static UnityAction OnTriggerPerformAttackEvent;
    public static UnityAction OnTriggerPerformSummonEvent;
    public static UnityAction OnTriggerPerformBulletHellAttackEvent;

    public static UnityAction OnTriggerSpikeEvent;
    public static UnityAction OnPerformSpikeAttackTriggered;
    public static UnityAction OnFinishAttackTriggered;
    public static UnityAction OnFinishProjectileAttackTriggered;
    public static UnityAction OnFinishBulletHellAttackTriggered;
    public static UnityAction OnFinishSpikeAttackTriggered;
    public static UnityAction OnFinishSummonAttackTriggered;

    private void OnEnable()
    {
        OnTriggerPerformAttackEvent += PerformAttack;
        OnTriggerPerformSummonEvent += PerformSummon;
        OnTriggerPerformBulletHellAttackEvent += PerformBulletHellAttack;
        OnPerformSpikeAttackTriggered += PerformSpikeAttack;
        OnTriggerSpikeEvent += InitSpikeAttack;

        OnFinishProjectileAttackTriggered += FinishProjectileAttack;
        OnFinishBulletHellAttackTriggered += FinishBulletHellAttack;
        OnFinishSpikeAttackTriggered += FinishSpikeAttack;
        OnFinishSummonAttackTriggered += FinishSummonAttack;
    }

    private void OnDisable()
    {
        OnTriggerPerformAttackEvent -= PerformAttack;
        OnTriggerPerformSummonEvent -= PerformSummon;
        OnTriggerPerformBulletHellAttackEvent -= PerformBulletHellAttack;
        OnPerformSpikeAttackTriggered -= PerformSpikeAttack;
        OnTriggerSpikeEvent -= InitSpikeAttack;

        OnFinishProjectileAttackTriggered -= FinishProjectileAttack;
        OnFinishBulletHellAttackTriggered -= FinishBulletHellAttack;
        OnFinishSpikeAttackTriggered -= FinishSpikeAttack;
        OnFinishSummonAttackTriggered -= FinishSummonAttack;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        bossAnimation = GetComponent<BossAnimation>();
        bulletHellPattern = GetComponent<BossBulletHellPattern>();
        bossProfile = GetComponent<BossProfile>();

        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = false;

        projectileSpeed = setProjectileSpeed;

        bulletHellCooldown = setBulletHellCooldown;
        numberOfEnemySummon = setNumberOfEnemySummon;

        summonCooldown = setSummonCooldown;

        spikeValue = setSpikeValue;
    }

    private void Start()
    {
        State();
    }

    private void Update()
    {
        InitPath();

        Move();
        Rotation();
        Attacks();
    }

    public void State()
    {
        if (currentPhase == 0 && bossProfile.bossHealth >= bossProfile.bossProfileSO.bossHealth * 0.70f)
        {
            currentPhase = 1;
            Debug.LogError("First Phase");
        }
        else if (currentPhase == 1 && bossProfile.bossHealth < bossProfile.bossProfileSO.bossHealth * 0.70f &&
            bossProfile.bossHealth >= bossProfile.bossProfileSO.bossHealth * 0.30f)
        {
            currentPhase = 2;

            SecondPhaseData();

            Debug.LogError("Second Phase");
        }
        else if (currentPhase == 2 && bossProfile.bossHealth < bossProfile.bossProfileSO.bossHealth * 0.30f)
        {
            currentPhase = 3;

            ThirdPhaseData();

            Debug.LogError("Third Phase");
        }
    }

    private void SecondPhaseData()
    {
        bossMoveSpeed += 2f;
        projectileSpeed += 10f;

        enableBulletHell = true;

        setNumberOfEnemySummon += 2f;
        setSummonCooldown -= 2f;
    }

    private void ThirdPhaseData()
    {
        bossMoveSpeed += 5f;
        projectileSpeed += 20f;

        setBulletHellCooldown -= 2f;
        bulletHellSummonThreshold = 3f;
        summonThresholdCooldownIncrease = 2f;

        setNumberOfEnemySummon += 4f;

        enableSpikeAttack = true;
    }

    private void InitPath()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private void Move()
    {
        if (isProjectile || isSummon || isBulletHell || isSpike)
            return;

        if (Vector3.Distance(player.transform.position, transform.position) > 0.5f + bossStoppingDistance)
        {
            navMeshAgent.Move(navMeshAgent.desiredVelocity.normalized * bossMoveSpeed * Time.deltaTime);
        }
    }

    private void Rotation()
    {
        if (isSummon || isBulletHell)
            return;

        targetRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
        yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, yAxisOnlyRotation, bossRotationSpeed * Time.deltaTime);
    }

    private void Attacks()
    {
        Projectile();
        Summon();
        BulletHell();
        Spike();
    }

    private void Projectile()
    {
        if (isProjectile || isSummon || isBulletHell)
            return;

        if (Vector3.Distance(player.transform.position, transform.position) <= 0.5f + bossStoppingDistance)
        {
            isProjectile = true;
            StartCoroutine(InitAttack(projectileDelayStart));
        }
    }
    public IEnumerator InitAttack(float delay)
    {
        navMeshAgent.ResetPath();
        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerProjectileShootAnimation(true);
    }

    private void PerformAttack()
    {
        Vector3 direction = (player.transform.position + (Vector3.up * 0.35f) - projectilePos.position).normalized;

        GameObject newProjectile = Instantiate(bossProjectilePrefab, projectilePos.position, Quaternion.identity);
        newProjectile.GetComponent<BossBulletHellProjectile>().Init(direction);
        newProjectile.GetComponent<BossBulletHellProjectile>().projectileSpeed = projectileSpeed;
    }

    private void FinishProjectileAttack()
    {
        isProjectile = false;
        bossAnimation.TriggerProjectileShootAnimation(false);
    }

    // Can be used if enemy is always getting hit by the player to increase difficulty.
    // Reference in BossProfile.cs
    public void ReduceSummonCooldown(float value)
    {
        //summonCooldown -= value;
    }

    public void IncreaseSummonCooldown(float value)
    {
        //summonCooldown += value;
    }

    private void Summon()
    {
        if (isSummon)
            return;

        if (EnemyManager.Instance.enemyAttackingList.Count + 4 >= minEnemyCurrently)
            return;

        if (summonCooldown <= 0)
        {
            isSummon = true;
            StartCoroutine(InitSummon(1f));
            summonCooldown = setSummonCooldown;

            Debug.LogError("Summon Initialized.");
        }
        else
        {
            summonCooldown -= Time.deltaTime;
            //Debug.LogError("Summon Cooldown: " + summonCooldown);
        }
    }

    public IEnumerator InitSummon(float delay)
    {
        navMeshAgent.ResetPath();

        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerSummonAnimation(true);
    }

    private void PerformSummon()
    {
        for (int i = 0; i < 100 && enemySummonPosition.Count != numberOfEnemySummon; i++)
        {
            Vector3 randomPoint = enemySummonGroundPos.position + Random.insideUnitSphere * enemySummonRadius;
            randomPoint.y = enemySummonGroundPos.position.y;

            if (NavMesh.SamplePosition(randomPoint, out navHit, minEnemyDistanceSummon, NavMesh.AllAreas))
            {
                float distanceToOrigin = Vector3.Distance(enemySummonGroundPos.position, navHit.position);

                if (distanceToOrigin >= minEnemyDistanceSummon)
                {
                    enemySummonPosition.Add(navHit.position);
                }
            }
        }

        for (int i = 0; i < numberOfEnemySummon && enemySummonPosition.Count != 0; i++)
        {
            GameObject summonedEnemy = Instantiate(skeletonPrefab, enemySummonPosition[i], Quaternion.identity);
            summonedEnemy.GetComponent<Enemy>().navMeshSurface = navMeshSurface;
            summonedEnemy.GetComponent<Enemy>().player = player.transform;
            summonedEnemy.GetComponent<Skeleton>().SetSkeletonSummonState(summonedEnemy.GetComponent<Enemy>(), true);
        }
    }

    private void FinishSummonAttack()
    {
        isSummon = false;
        bossAnimation.TriggerSummonAnimation(false);
    }

    // Can be used if enemy is always getting hit by the player to increase difficulty.
    // Reference in BossProfile.cs
    public void ReduceBulletHellCooldown(float value)
    {
        // bulletHellCooldown -= value;
    }

    private void BulletHell()
    {
        if (isBulletHell || !enableBulletHell)
            return;

        if (Vector3.Distance(player.transform.position, transform.position) > 0.5f + bossStoppingDistance)
        {
            if (bulletHellCooldown <= 0)
            {
                isBulletHell = true;
                StartCoroutine(InitBulletHell(0.3f));
                bulletHellCooldown = setBulletHellCooldown;

                Debug.LogError("Bullet Hell Initialized.");
            }
            else
            {
                bulletHellCooldown -= Time.deltaTime;
            }
        }
    }

    public IEnumerator InitBulletHell(float delay)
    {
        navMeshAgent.ResetPath();

        // To avoid summon to trigger immediately after bullet hell
        //if(summonCooldown <= bulletHellSummonThreshold)
        //{
        //    IncreaseSummonCooldown(summonThresholdCooldownIncrease);
        //}

        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerBulletHellAnimation(true);
    }

    private void PerformBulletHellAttack()
    {
        bulletHellPattern.StartSequentialBulletHell();
    }

    private void FinishBulletHellAttack()
    {
        isBulletHell = false;
        isSummon = false;
        bossAnimation.TriggerBulletHellAnimation(false);
    }

    private void Spike()
    {
        if (isSpike || !enableSpikeAttack)
            return;

        if (spikeValue <= 0)
        {
            if(Vector3.Distance(player.transform.position, transform.position) >= 15)
            {
                if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out hitInfo, bossDistance, playerLayer))
                {
                    navMeshAgent.ResetPath();
                    bossAnimation.TriggerSpikeAttackAnimation(true);
                    isSpike = true;
                }
            }
        }
        else
        {

            spikeValue -= Time.deltaTime;
        }
    }

    private void InitSpikeAttack()
    {
        spikeObj = Instantiate(spikePrefab, spikePos.position, transform.rotation);
        bossAnimation.TriggerSpikeAnimation(spikeObj.GetComponentInChildren<Animator>());
        spikeObj.transform.position = spikePos.position;
    }

    private void PerformSpikeAttack()
    {
        spikeObj.GetComponentInChildren<BoxCollider>().enabled = true;
        spikeValue = setSpikeValue;
    }

    private void FinishSpikeAttack()
    {
        bossAnimation.TriggerSpikeAttackAnimation(false);

        spikeObj.GetComponentInChildren<BoxCollider>().enabled = false;
        spikeObj = null;

        isSpike = false;
    }
}