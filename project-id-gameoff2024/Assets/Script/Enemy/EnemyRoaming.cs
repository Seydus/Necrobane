using System.Collections;
using TMPro.Examples;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour, IEnemyRoaming
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

        if (isPlayerDetected)
        {
            HandleEngagement();
        }
        else
        {
            HandleDisengagement();
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

            if (NavMesh.SamplePosition(randomPoint, out navHit, MaxRoamDistance, NavMesh.AllAreas))
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
                HandleEnemyType();
                break;
        }
    }

    private void HandleEnemyType()
    {
        switch(Enemy)
        {
            case Skeleton skeleton:
                if(skeleton)
                {
                    Debug.Log(player);
                    Debug.Log(NavMeshAgent);

                    skeleton.HandleAttack(player, NavMeshAgent);
                }
                else
                {
                    Debug.LogError("Error no skeleton script");
                }
                break;
            case Cultist cultist:
                //
                break;
        }
    }

    private void HandlePatrol()
    {
        Debug.Log($"MinRoamWaitTime: {MinRoamWaitTime}, MaxRoamWaitTime: {MaxRoamWaitTime}");

        if (!isRoaming && Vector3.Distance(roamTargetPosition, Enemy.transform.position) <= 2f)
        {
            isRoaming = true;

            // Ensure MinRoamWaitTime and MaxRoamWaitTime are properly assigned
            if (MinRoamWaitTime <= 0 || MaxRoamWaitTime <= 0)
            {
                Debug.LogError("MinRoamWaitTime or MaxRoamWaitTime is not properly initialized.");
                return;
            }

            Enemy.StartCoroutine(InitRoaming(Random.Range(MinRoamWaitTime, MaxRoamWaitTime)));
        }

        NavMeshAgent.SetDestination(roamTargetPosition);
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
