using System.Collections;
using UnityEngine;
using UnityEngine.AI;


// Note: Coroutine doesn't work unless you add Enemy.StartCoroutine
public class EnemyCombat : IEnemyCombat
{
    protected IEnemyCombat enemyCombat;
    public Enemy Enemy { get; set; }

    [Header("Enemy Combat")]
    public float RotateSpeed { get; set; }

    public float AttackDelay { get; set; }

    protected float AngleSetDifference;
    public bool IsAttacking { get; set; }

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

            if (!IsAttacking && Enemy is IEnemyCombat enemyCombat)
            {
                Enemy.StartCoroutine(enemyCombat.InitAttack(AttackDelay));
            }
        }
        else
        {
            if (!IsAttacking)
            {
                navMeshAgent.SetDestination(player.position);
            }
            else
            {
                navMeshAgent.ResetPath();
            }
        }
    }

    public IEnumerator InitAttack(float delay) { yield return null; }

    public Ray GetEnemyDirection() { return new Ray(); }


    public IEnumerator InitAttack(Transform player, float delay) { Debug.Log("Enemy Attack is coming from the EnemyCombat"); yield return null; }
}
