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
    #endregion

    public override void Awake()
    {
        base.Awake();

        enemyCombat = new EnemyCombat();
        enemyRoaming = new EnemyRoaming();

        enemyCombat.AttackSpeed = attackSpeed;
        enemyCombat.RotateSpeed = rotateSpeed;

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
    }

    public override void Start()
    {
        base.Start();

        enemyRoaming.Start();
    }

    public override void Update()
    {
        base.Update();

        if (enableRoaming)
            enemyRoaming.Update();
    }

    public Ray GetEnemyDirection()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range)
    {
        enemyCombat.HandleAttack(player, navMeshAgent, range);
    }

    public IEnumerator InitAttack(Transform player, float delay)
    {
        sphereRay = GetEnemyDirection();

        yield return new WaitForSeconds(delay);

        // Skeleton Melee Sword Attack

        if (Physics.SphereCast(sphereRay, sphereRadius, out enemyHitInfo, maxDistance, combatLayer))
        {
            enemyHit = true;

            if (enemyHitInfo.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                playerManager.PlayerProfile.DeductHealth(EnemyDamage);

                if (player != null)
                {
                    _HitPlayer.Post(player.gameObject);
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = enemyHit ? Color.green : Color.red;

        sphereRay = GetEnemyDirection();

        if (enemyHit)
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
