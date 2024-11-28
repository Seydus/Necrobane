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
    
    public void EnemySummonAttack()
    {
        if (!Enemy.navMeshAgent)
            return;

        if (Enemy is IEnemyRoaming enemyRoaming)
        {
            if (Vector3.Distance(Enemy.player.position, Enemy.transform.position) <= Enemy.enemyProfile.EnemyRange)
            {
                Enemy.navMeshAgent.ResetPath();

                Vector3 directionToTargetPosition = Enemy.player.position - Enemy.transform.position;
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
                    Vector3 directionToTargetPosition = Enemy.player.position - Enemy.transform.position;
                    directionToTargetPosition.y = 0;
                    directionToTargetPosition.Normalize();

                    Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
                    Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                    Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, yAxisOnlyRotation, RotateSpeed * Time.deltaTime);

                    Enemy.navMeshAgent.SetDestination(Enemy.player.position);

                    enemyRoaming.WalkSound();

                    Vector3 velocity = Enemy.navMeshAgent.desiredVelocity;
                    Enemy.transform.position += velocity.normalized * Enemy.roamingMoveSpeed * Time.deltaTime;
                }
                else
                {
                    Enemy.navMeshAgent.ResetPath();
                }

                isResetPath = false;
            }
        }
    }

    public void HandleAttack()
    {
        if (!Enemy.navMeshAgent)
            return;

        if (Enemy is IEnemyRoaming enemyRoaming)
        {
            if (Vector3.Distance(Enemy.player.position, Enemy.transform.position) <= Enemy.enemyProfile.EnemyRange && Enemy.isDetected)
            {
                Enemy.navMeshAgent.ResetPath();

                Vector3 directionToTargetPosition = Enemy.player.position - Enemy.transform.position;
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
                    Vector3 directionToTargetPosition = Enemy.player.position - Enemy.transform.position;
                    directionToTargetPosition.y = 0;
                    directionToTargetPosition.Normalize();

                    Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
                    Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                    Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, yAxisOnlyRotation, RotateSpeed * Time.deltaTime);

                    Enemy.navMeshAgent.SetDestination(Enemy.player.position);

                    enemyRoaming.WalkSound();

                    Vector3 velocity = Enemy.navMeshAgent.desiredVelocity;
                    Enemy.transform.position += velocity.normalized * Enemy.roamingMoveSpeed * Time.deltaTime;
                }
                else
                {
                    Enemy.navMeshAgent.ResetPath();
                }

                isResetPath = false;
            }
        }
    }


    public IEnumerator InitAttack(float delay) { yield return null; }

    public Ray GetEnemyDirection() { return new Ray(); }
}
