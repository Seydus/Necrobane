using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class BossCombat : MonoBehaviour
{
    [Header("Bullet Hell")]
    [SerializeField] private Transform bulletHellPos;
    [SerializeField] private float bulletHellAttackSpeed;
    private const float BULLET_HELL_GAP = 20f;
    [SerializeField] private float bulletHellDelaySpawn;
    [SerializeField] private GameObject bulletHellPrefab;

    private List<Vector3> bulletHellDirection = new List<Vector3>();

    [Header("Spike Attack")]
    [SerializeField] private Animator animSpike;
    [SerializeField] private float spikeAttackSpeed;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float bossDistance;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider spikeAnimCollider;
    [SerializeField] private float rotationSpeed;
    private bool isHit;
    private bool isSpikeAttack;
    private RaycastHit hitInfo;

    public GameObject player;

    [Header("NavMesh Settings")]
    [SerializeField] private float bossMoveSpeed;
    [SerializeField] private float bossRotationSpeed;
    [SerializeField] private float bossStoppingDistance;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private NavMeshSurface navMeshSurface;
    private NavMeshPath navMeshPath;
    private int currentCornerIndex;
    private Vector3 lastCornerPosition;

    [Header("Events")]
    public static UnityAction OnPerformSpikeAttackTriggered;
    public static UnityAction OnFinishAttackTriggered;

    private void OnEnable()
    {
        OnPerformSpikeAttackTriggered += PerformSpikeAttack;
        OnFinishAttackTriggered += FinishSpikeAttack;
    }

    private void OnDisable()
    {
        OnPerformSpikeAttackTriggered -= PerformSpikeAttack;
        OnFinishAttackTriggered -= FinishSpikeAttack;
    }

    private void Awake()
    {
        // player = GameObject.FindGameObjectWithTag("Player");

        navMeshPath = new NavMeshPath();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = true; 
        navMeshAgent.updateRotation = false;
    }

    private void Start()
    {
        GetBulletHellDirection();
        lastCornerPosition = transform.position;
        // BulletHell();
    }

    private void Update()
    {
        // SpikeAttack();
        Move();
        Rotation();
    }

    private void Move()
    {
        navMeshAgent.SetDestination(player.transform.position);
        Vector3 velocity = navMeshAgent.desiredVelocity;
        transform.position += velocity.normalized * bossMoveSpeed * Time.deltaTime;
    }

    private void Rotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.desiredVelocity.normalized);
        Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, yAxisOnlyRotation, bossRotationSpeed * Time.deltaTime);
    }

    private void GetBulletHellDirection()
    {
        float rotationY = 0;

        while(rotationY <= 360)
        {
            rotationY += BULLET_HELL_GAP;
            bulletHellDirection.Add(new Vector3(0f, rotationY, 0f));
        }
    }

    private void SpikeAttack()
    {
        if (isSpikeAttack)
            return;

        Vector3 directionToTargetPosition = player.transform.position - transform.position;
        directionToTargetPosition.y = 0;
        directionToTargetPosition.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(directionToTargetPosition);
        Quaternion yAxisOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, yAxisOnlyRotation, rotationSpeed * Time.deltaTime);


        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hitInfo, bossDistance, playerLayer))
        {
            animSpike.SetTrigger("isAttackSpike");
            isSpikeAttack = true;
            isHit = true;
        }
        else
        {
            isHit = false;
        }
    }

    private void PerformSpikeAttack()
    {
        spikeAnimCollider.enabled = true;
    }

    private void FinishSpikeAttack()
    {
        spikeAnimCollider.enabled = false;
        isSpikeAttack = false;
    }

    private void BulletHell()
    {
        StartCoroutine(InitStartBullet());
    }

    private IEnumerator InitStartBullet()
    {
        while(true)
        {
            yield return StartCoroutine(StartDominoBulletHell());

            yield return new WaitForSeconds(0.5f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.3f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(StartReverseSequentialBulletHell());

            yield return new WaitForSeconds(0.5f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.3f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(StartDominoBulletHell());

            yield return new WaitForSeconds(0.5f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.3f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.1f);

            yield return DoubleSequentialBulletHell();

            yield return new WaitForSeconds(0.5f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.3f);

            StartSequentialBulletHell();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DoubleSequentialBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            GameObject bulletHellObjTwo = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[(bulletHellDirection.Count - 1) - i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
        }
    }

    private IEnumerator StartDominoBulletHell()
    {
        for(int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
            //if (i + 1 >= bulletHellDirection.Count)
            //    i = 0;
        }
    }

    private IEnumerator StartReverseSequentialBulletHell()
    {
        for (int i = bulletHellDirection.Count - 1; i >= 0; i--)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
            //if (i + 1 >= bulletHellDirection.Count)
            //    i = 0;
        }
    }

    private void StartSequentialBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        if (isHit)
        {
            Gizmos.DrawRay(new Ray(bulletHellPos.position, bulletHellPos.forward).origin, hitInfo.point - new Ray(bulletHellPos.position, bulletHellPos.forward).origin);
            Gizmos.DrawWireSphere(hitInfo.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(new Ray(bulletHellPos.position, bulletHellPos.forward).origin, new Ray(bulletHellPos.position, bulletHellPos.forward).direction.normalized * bossDistance);
        }
    }
}