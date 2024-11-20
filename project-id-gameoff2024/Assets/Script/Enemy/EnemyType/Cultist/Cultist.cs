using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Cultist : Enemy, IEnemyRoaming, IEnemyCombat
{
    IEnemyRoaming enemyRoaming;
    IEnemyCombat enemyCombat;

    [Header("Cultist Settings")]
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject projectileObj;
    private bool enemyShoot;
    private bool enemyAttacking;

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

    public new void Awake()
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
    }

    public new void Start()
    {
        base.Start();

        enemyRoaming.Start();
    }

    public new void Update()
    {
        base.Update();

        if (enableRoaming)
            enemyRoaming.Update();
    }

    public Ray GetEnemyDirection()
    {
        return new Ray(Enemy.transform.position, Enemy.transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range)
    {
        enemyCombat.HandleAttack(player, navMeshAgent, range);
    }

    public void InitAttack()
    {
        enemyCombat.IsAttacking = true;
        sphereRay = GetEnemyDirection();
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
        enemyCombat.IsAttacking = false;
    }
}
