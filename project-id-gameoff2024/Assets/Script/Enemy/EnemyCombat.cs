using System.Collections;
using TMPro.Examples;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    private EnemyHolder enemyHolder;

    [Header("Enemy Combat")]
    [SerializeField] private float attackDelay;
    private float setAttackDelay;
    private bool initAttack;
    private bool damagedPlayer;

    [SerializeField] private float sphereRadius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask combatLayer;
    private Ray sphereRay;
    private RaycastHit hitInfo;

    [Header("Debugging")]
    private bool isHit;

    private void Awake()
    {
        enemyHolder = GetComponent<EnemyHolder>();
    }

    private Ray GetEnemyDirection()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent)
    {
        if (Vector3.Distance(player.position, transform.position) >= 2.5f)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath();

            if (!initAttack)
            {
                StartCoroutine(InitAttack());
                initAttack = true;
            }
        }
    }

    private IEnumerator InitAttack()
    {
        sphereRay = GetEnemyDirection();

        if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, maxDistance, combatLayer))
        {
            isHit = true;

            Debug.Log("Detected player");

            yield return new WaitForSeconds(attackDelay);

            if (!damagedPlayer)
            {
                if (hitInfo.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.PlayerProfile.DeductHealth(enemyHolder.EnemyProfile.enemyDamage);
                    Debug.Log("Enemy damaged player");
                }

                damagedPlayer = true;
            }
        }

        yield return new WaitForSeconds(0.1f);

        damagedPlayer = false;
        initAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        sphereRay = GetEnemyDirection();

        if (isHit)
        {
            Gizmos.DrawRay(sphereRay.origin, hitInfo.point - sphereRay.origin);
            Gizmos.DrawWireSphere(hitInfo.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction.normalized * maxDistance);
        }
    }
}
