using System.Collections;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;


// Becareful not to use the the first-capital letter variables
public class Skeleton : Enemy, IEnemyRoaming, IEnemyCombat
{
    IEnemyCombat enemyCombat;
    IEnemyRoaming enemyRoaming;

    private SkeletonAnimation skeletonAnimation;

    #region Misc
    public float AttackSpeed { get; set; }
    public float SphereRadius { get; set; }
    public float MaxDistance { get; set; }
    public LayerMask CombatLayer { get; set; }
    public float RotateSpeed { get; set; }
    public AK.Wwise.Event _HitPlayer { get; set; }
    public float MinRoamWaitTime { get; set; }
    public float MaxRoamWaitTime { get; set; }
    public float RoamDetectionRadius { get; set; }
    public float MaxRoamDistance { get; set; }
    public float RoamDirectionChangeChance { get; set; }
    public Transform GroundPos { get; set; }
    public float DetectRadius { get; set; }
    public LayerMask PlayerMask { get; set; }
    public float EngageCooldownDuration { get; set; }
    public float DisengageCooldownDuration { get; set; }
    public NavMeshSurface NavMeshSurface { get; set; }
    public Enemy Enemy { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public bool IsAttacking { get; set; }
    #endregion

    public override void Awake()
    {
        base.Awake();

        enemyCombat = new EnemyCombat();
        enemyRoaming = new EnemyRoaming();

        enemyCombat.RotateSpeed = rotateSpeed;
        enemyCombat.IsAttacking = isAttacking;

        enemyRoaming.MinRoamWaitTime = minRoamWaitTime;
        enemyRoaming.MaxRoamWaitTime = maxRoamWaitTime;
        enemyRoaming.RoamDetectionRadius = roamDetectionRadius;
        enemyRoaming.MaxRoamDistance = maxRoamDistance;
        enemyRoaming.RoamDirectionChangeChance = roamDirectionChangeChance;
        enemyRoaming.GroundPos = groundPos;
        enemyRoaming.NavMeshSurface = navMeshSurface;

        enemyRoaming.DetectRadius = detectRadius;
        enemyRoaming.PlayerMask = playerMask;
        enemyRoaming.EngageCooldownDuration = engageCooldownDuration;
        enemyRoaming.DisengageCooldownDuration = disengageCooldownDuration;
        enemyRoaming.NavMeshAgent = base._NavMeshAgent;

        enemyCombat.Enemy = this;
        enemyRoaming.Enemy = this;

        enemyCombat.Awake();

        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    public override void Start()
    {
        base.Start();

        enemyRoaming.Start();
    }

    public override void Update()
    {
        base.Update();

        EnemyRoaming();
    }

    private void EnemyRoaming()
    {
        if (enableRoaming)
        {
            enemyRoaming.Update();

            skeletonAnimation.SkeletonWalking(enemyRoaming.NavMeshAgent.desiredVelocity.magnitude);
        }
    }

    public Ray GetEnemyDirection()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range)
    {
        enemyCombat.HandleAttack(player, navMeshAgent, range);
    }

    public void InitAttack()
    {
        enemyCombat.IsAttacking = true;
        sphereRay = GetEnemyDirection();
        skeletonAnimation.SkeletonAttack(true);
    }

    private void PerformAttack()
    {
        if (Physics.SphereCast(sphereRay, sphereRadius, out enemyHitInfo, maxDistance, combatLayer))
        {
            if (enemyHitInfo.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                playerManager.PlayerProfile.DeductHealth(EnemyDamage);

                if (playerManager.transform != null)
                {
                    // _HitPlayer.Post(player.gameObject);
                }
            }
        }
    }

    private void FinishAttack()
    {
        skeletonAnimation.SkeletonAttack(false);
        enemyCombat.IsAttacking = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = EnemyHit ? Color.green : Color.red;

        sphereRay = GetEnemyDirection();

        if (EnemyHit)
        {
            Gizmos.DrawRay(sphereRay.origin, enemyHitInfo.point - sphereRay.origin);
            Gizmos.DrawWireSphere(enemyHitInfo.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction * maxDistance);
        }
    }
}
