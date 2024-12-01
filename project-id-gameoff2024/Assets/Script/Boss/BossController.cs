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
    [HideInInspector] public int currentPhase = 0;
    [SerializeField] private List<MethodData> attackPatternList = new List<MethodData>();
    private bool isPattern;
    private float currentCooldown;

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

    [SerializeField] private int minEnemyCurrently = 5;

    private List<Vector3> enemySummonPosition = new List<Vector3>();
    private NavMeshHit navHit;

    [Header("BulletHell")]
    [SerializeField] private float setBulletHellCooldown;
    private float bulletHellCooldown;
    private Coroutine bulletHellCoroutine;

    private bool isBulletHell;
    private bool enableBulletHell;

    [Header("Spike Attack")]
    [SerializeField] private float setSpikeCooldown;
    private float spikeCooldown;
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
        OnTriggerPerformAttackEvent += PerformProjectileAttack;
        OnTriggerPerformSummonEvent += PerformSummonAttack;
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
        OnTriggerPerformAttackEvent -= PerformProjectileAttack;
        OnTriggerPerformSummonEvent -= PerformSummonAttack;
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

        spikeCooldown = setSpikeCooldown;
    }

    private void Start()
    {
        State();
    }

    private void Update()
    {
        InitPath();

        // Move();
        Rotation();
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
        if (isSummon || isBulletHell || isSpike)
            return;

        targetRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
        yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, yAxisOnlyRotation, bossRotationSpeed * Time.deltaTime);
    }

    public void State()
    {
        if (currentPhase == 0 && bossProfile.bossHealth >= bossProfile.bossProfileSO.bossHealth * 0.70f)
        {
            currentPhase = 1;
            StartCoroutine(InitFirstPhase());

            Debug.LogError("First Phase");
        }
        else if (currentPhase == 1 && bossProfile.bossHealth < bossProfile.bossProfileSO.bossHealth * 0.70f &&
            bossProfile.bossHealth >= bossProfile.bossProfileSO.bossHealth * 0.30f)
        {
            SecondPhaseData();

            currentPhase = 2;
            StartCoroutine(InitSecondPhase());

            Debug.LogError("Second Phase");
        }
        else if (currentPhase == 2 && bossProfile.bossHealth < bossProfile.bossProfileSO.bossHealth * 0.30f)
        {
            ThirdPhaseData();

            currentPhase = 3;
            StartCoroutine(InitThirdPhase());

            Debug.LogError("Third Phase");
        }
    }

    private void SecondPhaseData()
    {
        projectileSpeed += 10f;

        enableBulletHell = true;

        setNumberOfEnemySummon += 2f;
        setSummonCooldown -= 2f;
        minEnemyCurrently += 1;
    }

    private void ThirdPhaseData()
    {
        projectileSpeed += 20f;

        enableSpikeAttack = true;

        setBulletHellCooldown -= 2f;
        setNumberOfEnemySummon += 4f;
        minEnemyCurrently += 1;
    }

    private void InitPath()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private IEnumerator InitFirstPhase()
    {
        while(currentPhase == 1)
        {
            yield return FirstPhaseAttackPattern();
            yield return null;
        }
    }

    private IEnumerator InitSecondPhase()
    {
        if(bulletHellCoroutine == null && !isSpike)
            bulletHellCoroutine = StartCoroutine(HandleBulletHell());

        while (currentPhase == 2)
        {
            yield return new WaitForSeconds(currentCooldown);
            yield return SecondPhaseAttackPattern();
            yield return null;
        }
    }

    private IEnumerator InitThirdPhase()
    {
        if (bulletHellCoroutine == null && !isSpike)
            bulletHellCoroutine = StartCoroutine(HandleBulletHell());

        while (currentPhase == 3)
        {
            yield return new WaitForSeconds(currentCooldown);
            yield return HandleSpikeAttack();
            yield return ThirdPhaseAttackPattern();

            yield return null;
        }
    }

    private IEnumerator FirstPhaseAttackPattern()
    {
        InitProjectile();

        yield return new WaitForSeconds(currentCooldown);

        if (EnemyManager.Instance.enemyAttackingList.Count + 2 < minEnemyCurrently)
        {
            float randomSpawn = Random.Range(0f, 1f);

            if (randomSpawn > 0.8f)
            {
                InitSummonAttack();
                yield return new WaitForSeconds(currentCooldown);
            }
        }

        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator SecondPhaseAttackPattern()
    {
        InitProjectile();

        yield return new WaitForSeconds(currentCooldown);

        if (EnemyManager.Instance.enemyAttackingList.Count + 2 < minEnemyCurrently)
        {
            float randomSpawn = Random.Range(0f, 1f);

            if (randomSpawn > 0.7f)
            {
                InitSummonAttack();
                yield return new WaitForSeconds(currentCooldown);
            }
        }

        yield return new WaitForSeconds(0.6f);
    }

    private IEnumerator ThirdPhaseAttackPattern()
    {
        InitProjectile();

        yield return new WaitForSeconds(currentCooldown);

        if (EnemyManager.Instance.enemyAttackingList.Count + 2 < minEnemyCurrently)
        {
            float randomSpawn = Random.Range(0f, 1f);

            if (randomSpawn > 0.5f)
            {
                InitSummonAttack();
                yield return new WaitForSeconds(currentCooldown);
            }
        }

        yield return new WaitForSeconds(0.4f);
    }

    ///

    public void InitProjectile()
    {
        if (isProjectile)
            return;

        isProjectile = true;
        navMeshAgent.ResetPath();
        bossAnimation.TriggerProjectileShootAnimation(true);
        currentCooldown = bossAnimation.GetCurrentAnimationLength();
    }

    private void PerformProjectileAttack()
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

    ///

    public void InitSummonAttack()
    {
        if (isSummon)
            return;

        isSummon = true;
        navMeshAgent.ResetPath();
        bossAnimation.TriggerSummonAnimation(true);
        currentCooldown = bossAnimation.GetCurrentAnimationLength();
    }

    private void PerformSummonAttack()
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
        currentCooldown = bossAnimation.GetCurrentAnimationLength();
    }

    ///

    private IEnumerator HandleBulletHell()
    {

        while (true) 
        {
            if(!isBulletHell)
            {
                if (bulletHellCooldown <= 0)
                {
                    isBulletHell = true;
                    InitBulletHell();
                    bulletHellCooldown = setBulletHellCooldown;
                }
                else
                {
                    bulletHellCooldown -= Time.deltaTime;
                }
            }

            yield return null;
        }
    }


    public void InitBulletHell()
    {
        navMeshAgent.ResetPath();
        bossAnimation.TriggerBulletHellAnimation(true);
        currentCooldown = bossAnimation.GetCurrentAnimationLength();
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

    ///

    private IEnumerator HandleSpikeAttack()
    {
        if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out hitInfo, bossDistance, playerLayer))
        {
            Debug.LogError("SPIKE ATTACK");
            navMeshAgent.ResetPath();
            bossAnimation.TriggerSpikeAttackAnimation(true);
            currentCooldown = bossAnimation.GetCurrentAnimationLength();
            isSpike = true;
        }

        yield return new WaitForSeconds(currentCooldown);
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
        spikeCooldown = setSpikeCooldown;
    }

    private void FinishSpikeAttack()
    {
        bossAnimation.TriggerSpikeAttackAnimation(false);

        spikeObj.GetComponentInChildren<BoxCollider>().enabled = false;
        spikeObj = null;

        isSpike = false;
    }
}