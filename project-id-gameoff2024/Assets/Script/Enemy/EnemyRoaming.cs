using System.Collections;
using TMPro.Examples;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
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
    public float RoamingMoveSpeed { get; set; }
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
    public LayerMask PlayerMask { get; set; }
    public float EngageCooldownDuration { get; set; }
    public float DisengageCooldownDuration { get; set; }

    private Transform player;
    private float engageCooldown;
    private float disengageCooldown;
    private bool isPlayerDetected;

    [Header("Others")]
    // can be better
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
        roamTargetPosition = GetNewPosition();
    }

    public void Update()
    {
        Init();
    }

    private void Init()
    {
        DetectPlayer();

        if (!Enemy.isAttacking)
        {
            if (isPlayerDetected)
            {
                HandleEngagement();
            }
            else
            {
                HandleDisengagement();
            }
        }

        HandleEnemyState();
    }

    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(Enemy.transform.position, DetectRadius, PlayerMask);
        isPlayerDetected = hitColliders.Length > 0;

        if (isPlayerDetected)
        {
            player = hitColliders[0].transform;
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
            enemyCombat.HandleAttack(player, NavMeshAgent, Enemy.enemyProfile.EnemyRange);
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

            Vector3 directionToTargetPosition = roamTargetPosition - Enemy.transform.position;
            directionToTargetPosition.y = 0;
            directionToTargetPosition.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
            Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            NavMeshAgent.SetDestination(roamTargetPosition);

            if (Vector3.Distance(roamTargetPosition, Enemy.transform.position) > 1.21f)
            {
                Enemy.transform.localRotation = Quaternion.Slerp(
                    Enemy.transform.localRotation,
                    yAxisOnlyRotation,
                    RoamingRotateSpeed * Time.deltaTime
                );

                Vector3 velocity = NavMeshAgent.desiredVelocity;
                Enemy.transform.position += velocity.normalized * RoamingMoveSpeed * Time.deltaTime;
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
        yield return new WaitForSeconds(value);

        roamTargetPosition = GetNewPosition();
        isRoaming = false;
    }


    private void HandleEngagement()
    {
        if (engageCooldown >= EngageCooldownDuration)
        {
            enemyState = EnemyState.Attack;
            engageCooldown = 0f;
        }
        else
        {
            engageCooldown += Time.deltaTime;
        }

        disengageCooldown = 0f;
    }

    private void HandleDisengagement()
    {
        if (disengageCooldown >= DisengageCooldownDuration)
        {
            enemyState = EnemyState.Patrol;
            disengageCooldown = 0f;
        }
        else
        {
            disengageCooldown += Time.deltaTime;
        }

        engageCooldown = 0f;
    }
}
