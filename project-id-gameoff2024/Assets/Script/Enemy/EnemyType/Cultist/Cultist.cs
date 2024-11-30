using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Cultist : Enemy, IEnemyRoaming, IEnemyCombat
{
    IEnemyRoaming enemyRoaming;
    IEnemyCombat enemyCombat;

    private CultistAnimation cultistAnimation;

    [Header("Cultist Settings")]
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject projectileObj;
    private bool enemyShoot;
    private bool enemyAttacking;
    private RaycastHit cultistHit;

    [Header("Summon Settings")]
    [SerializeField] private float setSummonCooldown;
    [SerializeField] private int numberOfEnemySummon;
    [SerializeField] private float enemySummonRadius;
    [SerializeField] private float minEnemyDistanceSummon;
    [SerializeField] private Transform enemySummonGroundPos;
    [SerializeField] private List<Vector3> enemySummonPosition = new List<Vector3>();
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private NavMeshSurface skeletonSummonSurface;
    private float summonCooldown;
    private bool isSummoning;
    private NavMeshHit navHit;

    [Header("Events")]
    [HideInInspector] public UnityEvent OnPerformAttackTriggered = new UnityEvent();
    [HideInInspector] public UnityEvent OnFinishAttackTriggered = new UnityEvent();
    [HideInInspector] public UnityEvent OnPerformSummonTriggered = new UnityEvent();

    #region Misc
    public float AttackSpeed { get; set; }
    public float SphereRadius { get; set; }
    public float MaxDistance { get; set; }
    public LayerMask CombatLayer { get; set; }
    public float RotateSpeed { get; set; }
    public AK.Wwise.Event _HitPlayer { get; set; }
    public float MinRoamWaitTime { get; set; }
    public float MaxRoamWaitTime { get; set; }
    public float RoamDetectionRadius { get; set; }
    public float MinRoamDistance { get; set; }
    public float MaxRoamDistance { get; set; }
    public float RoamDirectionChangeChance { get; set; }
    public Transform GroundPos { get; set; }
    public float DetectRadius { get; set; }
    public LayerMask EnvironmentMask { get; set; }
    public float EngageCooldownDuration { get; set; }
    public float DisengageCooldownDuration { get; set; }
    public NavMeshSurface NavMeshSurface { get; set; }
    public Enemy Enemy { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public bool IsAttacking { get; set; }
    public float AttackDelay { get; set; }
    public float RoamingRotateSpeed { get; set; }
    public float RoamingMoveSpeed { get; set; }
    public bool isBlockedByWall { get; set; }
    #endregion

    public override void Awake()
    {
        base.Awake();

        enemyCombat = new EnemyCombat();
        enemyRoaming = new EnemyRoaming();

        enemyCombat.RotateSpeed = rotateSpeed;
        enemyCombat.AttackDelay = attackDelay;
        enemyCombat.IsAttacking = isAttacking;

        enemyRoaming.MinRoamWaitTime = minRoamWaitTime;
        enemyRoaming.MaxRoamWaitTime = maxRoamWaitTime;
        enemyRoaming.RoamDetectionRadius = roamDetectionRadius;
        enemyRoaming.MinRoamDistance = minRoamDistance;
        enemyRoaming.MaxRoamDistance = maxRoamDistance;
        enemyRoaming.RoamDirectionChangeChance = roamDirectionChangeChance;
        enemyRoaming.GroundPos = groundPos;
        enemyRoaming.NavMeshSurface = navMeshSurface;
        enemyRoaming.RoamingRotateSpeed = roamingRotateSpeed;

        enemyRoaming.DetectRadius = detectRadius;
        enemyRoaming.EnvironmentMask = playerMask;
        enemyRoaming.EngageCooldownDuration = engageCooldownDuration;
        enemyRoaming.DisengageCooldownDuration = disengageCooldownDuration;
        enemyRoaming.NavMeshAgent = base.navMeshAgent;

        enemyCombat.Enemy = this;
        enemyRoaming.Enemy = this;

        enemyCombat.Awake();

        cultistAnimation = GetComponent<CultistAnimation>();

        OnPerformAttackTriggered.AddListener(PerformAttack);
        OnFinishAttackTriggered.AddListener(FinishAttack);
        OnPerformSummonTriggered.AddListener(PerformSummon);
    }

    private void OnDestroy()
    {
        OnPerformAttackTriggered.RemoveListener(PerformAttack);
        OnFinishAttackTriggered.RemoveListener(FinishAttack);
        OnPerformSummonTriggered.RemoveListener(PerformSummon);
    }

    public new void Start()
    {
        base.Start();

        enemyRoaming.Start();

        summonCooldown = setSummonCooldown;
    }

    public new void Update()
    {
        base.Update();

        if (enableRoaming)
        {
            enemyRoaming.Update();
            cultistAnimation.CultistWalking(enemyRoaming.NavMeshAgent.desiredVelocity.magnitude);
        }
    }

    public Ray GetEnemyDirection()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void HandleAttack()
    {
        enemyCombat.HandleAttack();

        if (isSummoning)
            return;

        if(summonCooldown <= 0)
        {
            isSummoning = true;

            InitSummonAttack();

            summonCooldown = setSummonCooldown;
        }
        else
        {
            summonCooldown -= Time.deltaTime;
        }
    }

    public IEnumerator InitAttack(float delay)
    {
        enemyRoaming.NavMeshAgent.velocity = Vector3.zero;
        isAttacking = true;
        enemyCombat.IsAttacking = true;
        yield return new WaitForSeconds(delay);
        cultistAnimation.CultistAttack(true);
    }

    private void PerformAttack()
    {
        if(player && projectilePos)
        {
            Vector3 direction = player.position + (Vector3.up * 0.35f) - projectilePos.position;
            direction.Normalize();

            GameObject newProjectile = Instantiate(projectileObj, projectilePos.position, Quaternion.identity);
            newProjectile.GetComponent<EnemyProjectileBullet>().Init(direction);
        }
    }

    public void InitSummonAttack()
    {
        cultistAnimation.CultistSummon();
    }

    public void PerformSummon()
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

        for(int i = 0; i < numberOfEnemySummon && enemySummonPosition.Count != 0; i++)
        {
            GameObject summonedEnemy = Instantiate(skeletonPrefab, enemySummonPosition[i], Quaternion.identity);
            summonedEnemy.GetComponent<Enemy>().navMeshSurface = skeletonSummonSurface;
            summonedEnemy.GetComponent<Enemy>().player = this.player;

            if(summonedEnemy.GetComponent<Enemy>() is IEnemyRoaming enemyRoaming)
            {
                enemyRoaming.DetectRadius *= 2f;
            }

            summonedEnemy.GetComponent<Skeleton>().skeletonIsSummoned = true;

        }

        isSummoning = false;
    }

    private void FinishAttack()
    {
        cultistAnimation.CultistAttack(false);
        enemyCombat.IsAttacking = false;
        isAttacking = false;
        enemyCombat.IsAttacking = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyProfile.EnemyRange);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectRadius, EnvironmentMask);

        if (hitColliders.Length > 0)
        {
            Debug.Log("Found a player!");
            Vector3 direction = hitColliders[0].transform.position - Enemy.transform.position;
            direction.Normalize();

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, direction * DetectRadius);
        }
    }

    public void WalkSound()
    {
        if (!FootstepIsPlaying)
        {
            FootSteps();
            LastFootstepTime = Time.time;
            FootstepIsPlaying = true;
        }
        else
        {
            if (enemyRoaming.NavMeshAgent.desiredVelocity.sqrMagnitude >= 1f)
            {
                if (Time.time - LastFootstepTime > 0.7f / enemyRoaming.NavMeshAgent.desiredVelocity.sqrMagnitude)
                {
                    FootstepIsPlaying = false;
                }
            }
        }
    }

    public void FootSteps()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out cultistHit, Mathf.Infinity))
        {
            PhysMat_Last = PhysMat;
            PhysMat = cultistHit.collider.tag;

            if (PhysMat != PhysMat_Last)
            {
                AkSoundEngine.SetSwitch("Material", PhysMat, gameObject);

                print(PhysMat);
            }
        }

        AkSoundEngine.PostEvent("Play_Footsteps", gameObject);
    }

    public void EnemySummonAttack()
    {
  
    }
}
