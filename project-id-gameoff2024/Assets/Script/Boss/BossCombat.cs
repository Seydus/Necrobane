using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class BossCombat : MonoBehaviour
{
    [Header("Bullet Hell")]
    [SerializeField] private float bulletHellAttackSpeed;
    [SerializeField] private float bulletHellGap;
    [SerializeField] private float bulletHellDelaySpawn;
    [SerializeField] private GameObject bulletHellPrefab;

    [SerializeField] private List<Vector3> bulletHellDirection = new List<Vector3>();

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

    private GameObject player;

    private int i;

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
        GetBulletHellDirection();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        BulletHell();
    }

    private void Update()
    {
        SpikeAttack();
    }

    private void GetBulletHellDirection()
    {
        if (bulletHellGap <= 0)
            return;

        float rotationY = 0;

        while(rotationY <= 360)
        {
            rotationY += bulletHellGap;
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
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, transform.position, Quaternion.Euler(bulletHellDirection[i]));
            GameObject bulletHellObjTwo = Instantiate(bulletHellPrefab, transform.position, Quaternion.Euler(bulletHellDirection[(bulletHellDirection.Count - 1) - i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
        }
    }

    private IEnumerator StartDominoBulletHell()
    {
        for(int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, transform.position, Quaternion.Euler(bulletHellDirection[i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
            //if (i + 1 >= bulletHellDirection.Count)
            //    i = 0;
        }
    }

    private IEnumerator StartReverseSequentialBulletHell()
    {
        for (int i = bulletHellDirection.Count - 1; i >= 0; i--)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, transform.position, Quaternion.Euler(bulletHellDirection[i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
            //if (i + 1 >= bulletHellDirection.Count)
            //    i = 0;
        }
    }

    private void StartSequentialBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, transform.position, Quaternion.Euler(bulletHellDirection[i]));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        if (isHit)
        {
            Gizmos.DrawRay(new Ray(transform.position, transform.forward).origin, hitInfo.point - new Ray(transform.position, transform.forward).origin);
            Gizmos.DrawWireSphere(hitInfo.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(new Ray(transform.position, transform.forward).origin, new Ray(transform.position, transform.forward).direction.normalized * bossDistance);
        }
    }
}
