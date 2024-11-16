using System.Collections;
using UnityEngine;
using UnityEngine.AI;


// Note: Coroutine doesn't work unless you add Enemy.StartCoroutine
public class EnemyCombat : IEnemyCombat
{
    protected IEnemyCombat enemyCombat;
    public Enemy Enemy { get; set; }

    [Header("Enemy Combat")]
    public float AttackSpeed { get; set; }
    public float RotateSpeed { get; set; }

    protected float AngleSetDifference;

    public void Awake()
    {
        enemyCombat = this;
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range)
    {
        if (Vector3.Distance(player.position, Enemy.transform.position) <= range)
        {
            navMeshAgent.ResetPath();

            Vector3 directionToPlayer = player.position - Enemy.transform.position;
            directionToPlayer.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float angleDifference = Quaternion.Angle(Enemy.transform.localRotation, targetRotation);

            if (angleDifference > AngleSetDifference)
            {
                Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, targetRotation, RotateSpeed * Time.deltaTime);
            }

            if (Enemy is IEnemyCombat enemyCombat)
            {
                Enemy.StartCoroutine(ExecuteAttack(enemyCombat, player, AttackSpeed));
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    public Ray GetEnemyDirection() { return new Ray(); }

    private IEnumerator ExecuteAttack(IEnemyCombat enemyCombat, Transform player, float attackDelay)
    {
        // Wait for the enemy's InitAttack coroutine to complete
        yield return Enemy.StartCoroutine(enemyCombat.InitAttack(player, attackDelay));
    }

    public IEnumerator InitAttack(Transform player, float delay) { Debug.Log("Enemy Attack is coming from the EnemyCombat"); yield return null; }
}
