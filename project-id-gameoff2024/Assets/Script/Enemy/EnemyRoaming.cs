using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : IEnemyRoaming
{
    private enum EnemyState
    {
        Patrol,
        Attack
    }

    private EnemyState enemyState;

    public Enemy Enemy { get; set; }

    [Header("Enemy Settings")]
    public float MinRoamWaitTime { get; set; }
    public float MaxRoamWaitTime { get; set; }
    public float RoamDetectionRadius { get; set; }
    public float RoamingRotateSpeed { get; set; }
    public float AngleSetDifference;
    public float MinRoamDistance { get; set; }
    public float MaxRoamDistance { get; set; }
    private float randomRoamValue;
    private float currentWaitTime;
    private bool walkable = false;
    private bool reachedDestination;
    private bool decideRandomValue;
    private bool isRoaming;

    public float RoamDirectionChangeChance { get; set; }
    public Transform GroundPos { get; set; }

    private Vector3 roamTargetPosition;
    private Vector3 directionToPlayer;

    [Header("Enemy Detection")]
    public float DetectRadius { get; set; }
    private RaycastHit enemyHit;
    public LayerMask EnvironmentMask { get; set; }
    public LayerMask WallMask { get; set; }
    public float EngageCooldownDuration { get; set; }
    public float DisengageCooldownDuration { get; set; }
    private bool startEngage = false;
    private bool startDisengage = false;

    private float engageCooldown;
    private float disengageCooldown;

    public bool isPlayerDetected { get; set; }

    [Header("Others")]
    public NavMeshSurface NavMeshSurface { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    private NavMeshHit navHit;

    public void Awake()
    {
        NavMeshAgent.updatePosition = true;
        NavMeshAgent.updateRotation = false;
    }

    public void Start()
    {
        disengageCooldown = DisengageCooldownDuration;
        engageCooldown = EngageCooldownDuration;

        roamTargetPosition = GetNewPosition();
    }

    public void Update()
    {
        Init();
    }

    private void Init()
    {
        DetectPlayer();

        if (isPlayerDetected)
        {
            if(!startEngage)
            {
                HandleEngagement();
            }

            disengageCooldown = DisengageCooldownDuration;
        }
        else
        {
            if(startDisengage)
            {
                HandleDisengagement();
            }

            engageCooldown = EngageCooldownDuration;
        }

        HandleEnemyState();
    }
   
    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(Enemy.transform.position, DetectRadius, EnvironmentMask);

        if (hitColliders.Length > 0)
        {
            Transform playerTransform = hitColliders[0].transform;
            Vector3 direction = (playerTransform.position - Enemy.transform.position).normalized;
            float distance = Vector3.Distance(Enemy.transform.position, playerTransform.position);

            // EnvironmentMask: Obstacle, Player
            if (Physics.SphereCast(Enemy.transform.position, 0.1f, direction, out enemyHit, 10f, EnvironmentMask))
            {
                if (enemyHit.collider.CompareTag("Player"))
                {
                    isPlayerDetected = true;
                    Enemy.isDetected = true;
                    Enemy.player = playerTransform;
                }
                else
                {
                    isPlayerDetected = false;
                    Enemy.isDetected = false;
                }
            }
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    private Vector3 GetNewPosition()
    {
        Vector3 newPosition = Vector3.zero;
        walkable = false;

        for (int i = 0; i < 50 && !walkable; i++)
        {
            Vector3 randomPoint = GroundPos.position + Random.insideUnitSphere * RoamDetectionRadius;
            randomPoint.y = GroundPos.position.y;

            if (NavMesh.SamplePosition(randomPoint, out navHit, MaxRoamDistance, NavMesh.AllAreas))
            {
                float distanceToOrigin = Vector3.Distance(GroundPos.position, navHit.position);

                if (distanceToOrigin >= MinRoamDistance)
                {
                    newPosition = navHit.position;
                    walkable = true;
                }
            }
        }

        if (!walkable)
        {
            newPosition = Enemy.transform.position;
        }

        return newPosition;
    }

    private void HandleEnemyState()
    {
        switch (enemyState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Attack:
                HandleEnemyType();
                break;
        }
    }

    private void HandleEnemyType()
    {
        if (Enemy is IEnemyCombat enemyCombat)
        {
            enemyCombat.HandleAttack();
        }
    }

    private void HandlePatrol()
    {
        if (!Enemy.isAttacking)
        {
            if (!isRoaming && Vector3.Distance(roamTargetPosition, Enemy.transform.position) <= 2f)
            {
                isRoaming = true;
                Enemy.StartCoroutine(InitRoaming(Random.Range(MinRoamWaitTime, MaxRoamWaitTime)));
            }

            Vector3 directionToTargetPosition = (NavMeshAgent.desiredVelocity).normalized;
            directionToTargetPosition.y = 0;
 
            Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
            Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            NavMeshAgent.SetDestination(roamTargetPosition);

            if(Enemy is IEnemyRoaming enemyRoaming)
            {
                enemyRoaming.WalkSound();
            }

            if (Vector3.Distance(roamTargetPosition, Enemy.transform.position) > 1.21f)
            {
                Enemy.transform.localRotation = Quaternion.Slerp(
                    Enemy.transform.localRotation,
                    yAxisOnlyRotation,
                    RoamingRotateSpeed * Time.deltaTime
                );

                Vector3 velocity = NavMeshAgent.desiredVelocity;

                Enemy.transform.position += velocity.normalized * Enemy.roamingMoveSpeed * Time.deltaTime;
            }
            else
            {
                NavMeshAgent.ResetPath();
            }
        }
        else
        {
            NavMeshAgent.ResetPath();
        }
    }

    private IEnumerator InitRoaming(float value)
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        yield return new WaitForSeconds(value);

        for(int i = 0; i < 100 && navMeshPath.status != NavMeshPathStatus.PathComplete; i++)
        {
            roamTargetPosition = GetNewPosition();
            NavMeshAgent.CalculatePath(roamTargetPosition, navMeshPath);
        }

        isRoaming = false;
    }

    private void HandleEngagement()
    {
        if (engageCooldown <= 0)
        {
            enemyState = EnemyState.Attack;
            EnemyManager.Instance.enemyAttackingList.Add(this.Enemy);
            engageCooldown = 0f;

            startEngage = true;
            startDisengage = true;
        }
        else
        {
            engageCooldown -= Time.deltaTime;
        }

        disengageCooldown = 0f;
    }

    private void HandleDisengagement()
    {
        if (disengageCooldown <= 0)
        {
            enemyState = EnemyState.Patrol;
            EnemyManager.Instance.enemyAttackingList.Remove(this.Enemy);
            Debug.LogError("ENEMY PATROLLING");
            disengageCooldown = 0f;

            startEngage = false;
            startDisengage = false;
        }
        else
        {
            disengageCooldown -= Time.deltaTime;
        }

        engageCooldown = 0f;
    }
    public void WalkSound() { }

    public void FootSteps() { }

}
