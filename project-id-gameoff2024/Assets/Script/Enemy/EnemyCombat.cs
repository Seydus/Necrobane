using System.Collections;
using UnityEngine;
using UnityEngine.AI;


// Note: Coroutine doesn't work unless you add Enemy.StartCoroutine
public class EnemyCombat : MonoBehaviour, IEnemyCombat
{
    protected IEnemyCombat enemyCombat;
    public Enemy Enemy { get; set; }

    [Header("Enemy Combat")]
    public float AttackDelay { get; set; }
    protected float setAttackDelay;
    protected bool initAttack;
    protected bool damagedPlayer;

    public float SphereRadius { get; set; }
    public float MaxDistance { get; set; }
    public LayerMask CombatLayer { get; set; }
    public float RotateSpeed { get; set; }
    protected Ray sphereRay;
    protected RaycastHit hitInfo;

    protected float angleSetDifference;

    [Header("Wwise")]
    public AK.Wwise.Event _HitPlayer { get; set; }

    [Header("Debugging")]
    protected bool isHit;

    protected Transform playerTransform;

    public void Awake()
    {
        enemyCombat = this;
        setAttackDelay = AttackDelay;
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent)
    {
        if (Vector3.Distance(player.position, Enemy.transform.position) <= 2.5f)
        {
            navMeshAgent.ResetPath();
            playerTransform = player;

            Vector3 directionToPlayer = player.position - Enemy.transform.position;
            directionToPlayer.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float angleDifference = Quaternion.Angle(Enemy.transform.localRotation, targetRotation);

            if (angleDifference > angleSetDifference)
            {
                Enemy.transform.localRotation = Quaternion.Slerp(Enemy.transform.localRotation, targetRotation, RotateSpeed * Time.deltaTime);
            }

            if (!initAttack)
            {
                Enemy.StartCoroutine(InitAttack(AttackDelay));
                initAttack = true;
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.position);
        }

    }

    Ray IEnemyCombat.GetEnemyDirection()
    {
        return new Ray(Enemy.transform.position, Enemy.transform.forward);
    }

    public IEnumerator InitAttack(float delay)
    {
        sphereRay = enemyCombat.GetEnemyDirection();

        yield return new WaitForSeconds(delay);

        if (Physics.SphereCast(sphereRay, SphereRadius, out hitInfo, MaxDistance, CombatLayer))
        {
            Debug.Log("Player damaged");
            isHit = true;

            if (!damagedPlayer)
            {
                if (hitInfo.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.PlayerProfile.DeductHealth(Enemy.EnemyDamage);

                    if (playerTransform != null)
                    {
                        _HitPlayer.Post(playerTransform.gameObject);
                    }
                }

                damagedPlayer = true;
            }
        }

        yield return new WaitForSeconds(0.1f);

        damagedPlayer = false;
        initAttack = false;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        sphereRay = enemyCombat.GetEnemyDirection();

        if (isHit)
        {
            Gizmos.DrawRay(sphereRay.origin, hitInfo.point - sphereRay.origin);
            Gizmos.DrawWireSphere(hitInfo.point, SphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction * MaxDistance);
        }
    }
}
