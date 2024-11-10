using System.Collections;
using TMPro.Examples;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Attack
    }

    private EnemyState enemyState;

    private EnemyHolder enemyHolder;

    [Header("Enemy Settings")]
    [SerializeField] private float minRoamWaitTime;
    [SerializeField] private float maxRoamWaitTime;
    [SerializeField] private float roamDetectionRadius;
    [SerializeField] private float maxRoamDistance;
    private float randomRoamValue;
    private float currentWaitTime;
    private bool walkable = false;
    private bool reachedDestination;
    private bool decideRandomValue;
    private bool isRoaming;

    [Space]
    [SerializeField] private float roamDirectionChangeChance;
    [SerializeField] private Transform groundCheckOrigin;

    private Vector3 roamTargetPosition;

    [Header("Enemy Detection")]
    [SerializeField] private float detectRadius = 5.0f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float engageCooldownDuration = 2.0f;
    [SerializeField] private float disengageCooldownDuration = 2.0f;

    private Transform player;
    private float engageCooldown;
    private float disengageCooldown;
    private bool isPlayerDetected;

    [Header("Others")]
    // can be better
    [SerializeField] private NavMeshSurface navMeshSurface;
    private NavMeshAgent navMeshAgent;
    private NavMeshHit navHit;

    private void Awake()
    {
        enemyHolder = GetComponent<EnemyHolder>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        roamTargetPosition = GetNewPosition();
    }

    public void Init()
    {
        DetectPlayer();

        if (isPlayerDetected)
        {
            HandleEngagement();
        }
        else
        {
            HandleDisengagement();
        }

        HandleEnemyState();
        enemyHolder.EnemyProfile.EnemyStatus(gameObject);
    }

    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius, playerMask);
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

        while (!walkable)
        {
            Vector3 randomPoint = groundCheckOrigin.position + Random.insideUnitSphere * roamDetectionRadius;

            if (NavMesh.SamplePosition(randomPoint, out navHit, maxRoamDistance, NavMesh.AllAreas))
            {
                newPosition = navHit.position;
                walkable = true;
            }
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
                enemyHolder.EnemyCombat.HandleAttack(player, navMeshAgent);
                break;
        }
    }

    private void HandlePatrol()
    {
        if (!isRoaming && Vector3.Distance(roamTargetPosition, transform.position) <= 1.2f)
        {
            isRoaming = true;
            StartCoroutine(InitRoaming(Random.Range(minRoamWaitTime, maxRoamWaitTime)));
        }

        navMeshAgent.SetDestination(roamTargetPosition);
    }

    private IEnumerator InitRoaming(float value)
    {
        yield return new WaitForSeconds(value);
        roamTargetPosition = GetNewPosition();
        yield return null;
        isRoaming = false;
    }


    private void HandleEngagement()
    {
        if (engageCooldown >= engageCooldownDuration)
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
        if (disengageCooldown >= disengageCooldownDuration)
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
