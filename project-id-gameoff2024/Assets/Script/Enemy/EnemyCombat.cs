using System.Collections;
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

    [SerializeField] private float sphereRadius = 0.4f;
    [SerializeField] private float maxDistance = 0.9f;
    [SerializeField] private LayerMask combatLayer;
    [SerializeField] private float rotateSpeed;
    private Ray sphereRay;
    private RaycastHit hitInfo;

    private float angleSetDifference;

    [Header("Wwise")]
    public AK.Wwise.Event HitPlayer;

    [Header("Debugging")]
    private bool isHit;

    // Store reference to player
    private Transform playerTransform;

    private void Awake()
    {
        enemyHolder = GetComponent<EnemyHolder>();
    }

    private void Start()
    {
        setAttackDelay = attackDelay;
    }

    private Ray GetEnemyDirection()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = -0.115f;
        return new Ray(newPosition, transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent)
    {
        if (Vector3.Distance(player.position, transform.position) <= 2.5f)
        {
            navMeshAgent.ResetPath();
            // Store player reference
            playerTransform = player;

            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float angleDifference = Quaternion.Angle(transform.localRotation, targetRotation);

            Debug.Log(angleDifference);

            if (angleDifference > angleSetDifference)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
            }

            if (!initAttack)
            {
                StartCoroutine(InitAttack(attackDelay));
                initAttack = true;
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.position);
        }

    }

    private IEnumerator InitAttack(float delay)
    {
        sphereRay = GetEnemyDirection();

        yield return new WaitForSeconds(delay);

        if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, maxDistance, combatLayer))
        {
            isHit = true;

            Debug.Log("Detected player");

            if (!damagedPlayer)
            {
                if (hitInfo.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.PlayerProfile.DeductHealth(enemyHolder.EnemyProfile.enemyDamage);

                    // Use playerTransform reference here
                    HitPlayer.Post(playerTransform.gameObject);

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
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction * maxDistance);
        }
    }
}
