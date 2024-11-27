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

    private bool isResetPath;

    public void Awake()
    {
        enemyCombat = this;
    }

    public void HandleAttack(Transform player, NavMeshAgent agent, float range)
    {
        if (!agent)
            return;

        if (Vector3.Distance(player.position, Enemy.transform.position) <= range)
        {
            if(!isResetPath)
            {
                agent.ResetPath();
                isResetPath = true;
            }

            Vector3 directionToTargetPosition = player.position - Enemy.transform.position;
            directionToTargetPosition.y = 0;
            directionToTargetPosition.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
            Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, yAxisOnlyRotation, RotateSpeed * Time.deltaTime);


            if (!IsAttacking && Enemy is IEnemyCombat enemyCombat)
            {
                Enemy.StartCoroutine(enemyCombat.InitAttack(AttackDelay));
            }
        }
        else
        {
            if (!IsAttacking)
            {
                if (Enemy is IEnemyRoaming enemyRoaming)
                {
                    Vector3 directionToTargetPosition = player.position - Enemy.transform.position;
                    directionToTargetPosition.y = 0;
                    directionToTargetPosition.Normalize();

                    Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
                    Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                    Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, yAxisOnlyRotation, RotateSpeed * Time.deltaTime);

                    agent.SetDestination(player.position);

                    Vector3 velocity = agent.desiredVelocity;
                    Enemy.transform.position += velocity.normalized * Enemy.roamingMoveSpeed * Time.deltaTime;
                }
            }
            else
            {
                agent.ResetPath();
            }

            isResetPath = false;
        }
    }


    public IEnumerator InitAttack(float delay) { yield return null; }

    public Ray GetEnemyDirection() { return new Ray(); }


    public IEnumerator InitAttack(Transform player, float delay) { Debug.Log("Enemy Attack is coming from the EnemyCombat"); yield return null; }
}
