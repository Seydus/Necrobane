using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour
{
    [SerializeField] private float targetRange;
    [SerializeField] private float setWaitingTime;
    private float waitingTime;

    // Needs to be change 
    [SerializeField] private Transform groundTransform;
    private Vector3 groundPosition;
    //

    private Vector3 newPosition;

    private enum EnemyState
    {
        Patrol,
        Attack
    }

    [SerializeField] private EnemyState enemyState;

    private NavMeshHit hit;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        groundPosition = groundTransform.position;
        newPosition = GetRandomPoint(groundPosition);

        Debug.Log(newPosition);
    }

    private void Update()
    {
        HandleEnemyState(enemyState);
    }

    private Vector3 GetRandomPoint(Vector3 center)
    {
        Vector3 result = Vector3.zero;

        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * targetRange;

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return result;
            }
        }

        return result;
    }

    private void HandleEnemyState(EnemyState state)
    {
        switch(state)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandlePatrol()
    {
        navMeshAgent.SetDestination(newPosition);
    }

    private void HandleAttack()
    {

    }
}
