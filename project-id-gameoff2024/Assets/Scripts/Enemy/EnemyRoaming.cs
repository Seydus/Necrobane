using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour
{
    [Header("Waiting Time")]
    [SerializeField] private float setWaitingTime;
    private float waitingTime;

    private Vector3 newPosition;

    private enum EnemyState
    {
        Patrol,
        Attack
    }

    [SerializeField] private EnemyState enemyState;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        newPosition = GetNewPosition();
    }

    private Vector3 GetNewPosition()
    {
        return Vector3.zero;
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

    }

    private void HandleAttack()
    {

    }
}
