using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class BossController : MonoBehaviour
{
    [Header("Conditions")]
    [SerializeField] private float setDistanceBulletHellTrigger;
    private float distanceBulletHellTrigger;
    private bool isBulletHellDistance;

    [Header("Summon")]
    [SerializeField] private float setSummonValue;
    [SerializeField] private float numberOfEnemySummon;
    [SerializeField] private float enemySummonRadius;
    [SerializeField] private float minEnemyDistanceSummon = 5f;

    [SerializeField] private Transform enemySummonGroundPos;
    private List<Vector3> enemySummonPosition = new List<Vector3>();
    private NavMeshHit navHit;
    private float summonValue;
    private bool isSummon;

    [SerializeField] private GameObject skeletonPrefab;

    [Header("Projectile Attack")]
    [SerializeField] private float projectileDelayStart;
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject bossProjectilePrefab;
    private bool isAttacking;

    [Header("Spike Attack")]
    [SerializeField] private float setSpikeValue;
    private float spikeValue;
    [SerializeField] private Transform spikePos;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private float bossDistance;
    [SerializeField] private LayerMask playerLayer;

    private GameObject spikeObj;

    private bool isHit;
    private bool isSpikeAttack;
    private RaycastHit hitInfo;

    public GameObject player;

    [Header("NavMesh Settings")]
    [SerializeField] private float bossMoveSpeed;
    [SerializeField] private float bossRotationSpeed;
    [SerializeField] private float bossStoppingDistance;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private NavMeshSurface navMeshSurface;
    private Vector3 velocity;

    [Header("Others")]
    private BossAnimation bossAnimation;
    private BossBulletHellPattern bulletHellPattern;


    [Header("Events")]
    public static UnityAction OnTriggerPerformAttackEvent;
    public static UnityAction OnTriggerPerformSummonEvent;
    public static UnityAction OnTriggerPerformBulletHellAttackEvent;

    public static UnityAction OnTriggerSpikeEvent;
    public static UnityAction OnPerformSpikeAttackTriggered;
    public static UnityAction OnFinishAttackTriggered;

    private void OnEnable()
    {
        OnTriggerPerformAttackEvent += PerformAttack;
        OnTriggerPerformSummonEvent += PerformSummon;
        OnTriggerPerformBulletHellAttackEvent += PerformBulletHellAttack;
        OnPerformSpikeAttackTriggered += PerformSpikeAttack;
        OnFinishAttackTriggered += FinishAttack;
        OnTriggerSpikeEvent += InitSpikeAttack;
    }

    private void OnDisable()
    {
        OnTriggerPerformAttackEvent -= PerformAttack;
        OnTriggerPerformSummonEvent -= PerformSummon;
        OnTriggerPerformBulletHellAttackEvent -= PerformBulletHellAttack;
        OnPerformSpikeAttackTriggered -= PerformSpikeAttack;
        OnFinishAttackTriggered -= FinishAttack;
        OnTriggerSpikeEvent -= InitSpikeAttack;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        bossAnimation = GetComponent<BossAnimation>();
        bulletHellPattern = GetComponent<BossBulletHellPattern>();

        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = false;

        distanceBulletHellTrigger = setDistanceBulletHellTrigger;
        summonValue = setSummonValue;
        spikeValue = setSpikeValue;
    }

    private void Update()
    {

        Debug.DrawRay(transform.position, transform.forward * bossDistance);

        if (isBulletHellDistance || isSummon || isSpikeAttack)
            return;

        Move();
        Rotation();
        //Summon();
        Spike();
    }

    private void Move()
    {
        navMeshAgent.SetDestination(player.transform.position);
        velocity = navMeshAgent.desiredVelocity;

        if (Vector3.Distance(player.transform.position, transform.position) <= 0.5f + bossStoppingDistance)
        {
            StartCoroutine(InitAttack(projectileDelayStart));
        }
        else
        {
            //BulletHellDistanceTrigger();
        }

        transform.position += velocity.normalized * bossMoveSpeed * Time.deltaTime;
    }

    private void Spike()
    {
        if (spikeValue <= 0)
        {
            SpikeAttack();
        }
        else
        {
            spikeValue -= Time.deltaTime;
        }
    }

    private void Summon()
    {
        if(summonValue <= 0)
        {
            StartCoroutine(InitSummon(1f));
            summonValue = setSummonValue;
        }
        else
        {
            summonValue -= Time.deltaTime;
        }
    }

    public IEnumerator InitSummon(float delay)
    {
        isSummon = true;

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerSummonAnimation();
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

        isSummon = false;
    }


    private void BulletHellDistanceTrigger()
    {
        if (isBulletHellDistance)
            return;

        if (distanceBulletHellTrigger <= 0)
        {
            StartCoroutine(InitBulletHell(0.1f));
            distanceBulletHellTrigger = setDistanceBulletHellTrigger;
        }
        else
        {
            distanceBulletHellTrigger -= Time.deltaTime;
        }
    }

    private void Rotation()
    {
        Quaternion yAxisOnlyRotation;

        if (Vector3.Distance(player.transform.position, transform.position) >= 0.5f + bossStoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.desiredVelocity.normalized);
            yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
            yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, yAxisOnlyRotation, bossRotationSpeed * Time.deltaTime);
    }
    public IEnumerator InitAttack(float delay)
    {
        isAttacking = true;

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerProjectileShootAnimation();
    }

    public IEnumerator InitBulletHell(float delay)
    {
        isBulletHellDistance = true;
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(delay);
        bossAnimation.TriggerBulletHellAnimation();
    }

    private void PerformAttack()
    {
        Vector3 direction = (player.transform.position + (Vector3.up * 0.35f) - projectilePos.position).normalized;

        GameObject newProjectile = Instantiate(bossProjectilePrefab, projectilePos.position, Quaternion.identity);
        newProjectile.GetComponent<BossBulletHellProjectile>().Init(direction);
    }

    private void PerformBulletHellAttack()
    {
        bulletHellPattern.StartSequentialBulletHell();
    }

    private void SpikeAttack()
    {
        if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out hitInfo, bossDistance, playerLayer))
        {
            bossAnimation.TriggerSpikeAttackAnimation();
            isSpikeAttack = true;
        }
    }

    private void InitSpikeAttack()
    {
        Debug.Log("Spike Attack Initialized.");
        spikeObj = Instantiate(spikePrefab, spikePos.position, transform.rotation);
        bossAnimation.TriggerSpikeAnimation(spikeObj.GetComponentInChildren<Animator>());
        spikeObj.transform.position = spikePos.position;
    }

    private void PerformSpikeAttack()
    {
        spikeObj.GetComponentInChildren<BoxCollider>().enabled = true;
        spikeValue = setSpikeValue;
    }

    private void FinishAttack()
    {
        isSpikeAttack = false;
        isAttacking = false;
        isBulletHellDistance = false;

        if(spikeObj)
        {
            spikeObj.GetComponentInChildren<BoxCollider>().enabled = false;
        }
    }
}