using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public class Cultist : Enemy, IEnemyRoaming, IEnemyCombat
{
    IEnemyRoaming enemyRoaming;
    IEnemyCombat enemyCombat;

    private CultistAnimation cultistAnimation;

    [Header("Cultist Settings")]
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject projectileObj;
    private bool enemyShoot;
    private bool enemyAttacking;
    private RaycastHit cultistHit;

    [Header("Events")]
    public static UnityAction OnPerformAttackTriggered;
    public static UnityAction OnFinishAttackTriggered;

    // can be improved
    private Transform player;

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
    public float MinRoamDistance { get; set; }
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
    public float AttackDelay { get; set; }
    public float RoamingRotateSpeed { get; set; }
    public float RoamingMoveSpeed { get; set; }
    #endregion

    public void OnEnable()
    {
        OnPerformAttackTriggered += PerformAttack;
        OnFinishAttackTriggered += FinishAttack;
    }

    public void OnDisable()
    {
        OnPerformAttackTriggered -= PerformAttack;
        OnFinishAttackTriggered -= FinishAttack;
    }

    public override void Awake()
    {
        base.Awake();

        enemyCombat = new EnemyCombat();
        enemyRoaming = new EnemyRoaming();

        enemyCombat.RotateSpeed = rotateSpeed;
        enemyCombat.AttackDelay = attackDelay;
        enemyCombat.IsAttacking = isAttacking;

        enemyRoaming.MinRoamWaitTime = minRoamWaitTime;
        enemyRoaming.MaxRoamWaitTime = maxRoamWaitTime;
        enemyRoaming.RoamDetectionRadius = roamDetectionRadius;
        enemyRoaming.MinRoamDistance = minRoamDistance;
        enemyRoaming.MaxRoamDistance = maxRoamDistance;
        enemyRoaming.RoamDirectionChangeChance = roamDirectionChangeChance;
        enemyRoaming.GroundPos = groundPos;
        enemyRoaming.NavMeshSurface = navMeshSurface;
        enemyRoaming.RoamingRotateSpeed = roamingRotateSpeed;

        enemyRoaming.DetectRadius = detectRadius;
        enemyRoaming.PlayerMask = playerMask;
        enemyRoaming.EngageCooldownDuration = engageCooldownDuration;
        enemyRoaming.DisengageCooldownDuration = disengageCooldownDuration;
        enemyRoaming.NavMeshAgent = base.navMeshAgent;

        enemyCombat.Enemy = this;
        enemyRoaming.Enemy = this;

        enemyCombat.Awake();

        cultistAnimation = GetComponent<CultistAnimation>();
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
        {
            enemyRoaming.Update();
            cultistAnimation.CultistWalking(enemyRoaming.NavMeshAgent.desiredVelocity.magnitude);
        }
    }

    public Ray GetEnemyDirection()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range)
    {
        this.player = player;
        enemyCombat.HandleAttack(player, navMeshAgent, range);
    }

    public IEnumerator InitAttack(float delay)
    {
        enemyRoaming.NavMeshAgent.velocity = Vector3.zero;
        isAttacking = true;
        enemyCombat.IsAttacking = true;
        yield return new WaitForSeconds(delay);
        cultistAnimation.CultistAttack(true);
    }

    private void PerformAttack()
    {
        Vector3 direction = (player.position + (Vector3.up * 0.35f) - projectilePos.position).normalized;

        GameObject newProjectile = Instantiate(projectileObj, projectilePos.position, Quaternion.identity);
        newProjectile.GetComponent<EnemyProjectileBullet>().Init(direction);
    }

    private void FinishAttack()
    {
        cultistAnimation.CultistAttack(false);
        enemyCombat.IsAttacking = false;
        isAttacking = false;
        enemyCombat.IsAttacking = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyProfile.EnemyRange);
    }

    public void WalkSound()
    {
        if (!FootstepIsPlaying)
        {
            FootSteps();
            LastFootstepTime = Time.time;
            FootstepIsPlaying = true;
        }
        else
        {
            if (enemyRoaming.NavMeshAgent.desiredVelocity.sqrMagnitude >= 1f)
            {
                if (Time.time - LastFootstepTime > 2.5f / enemyRoaming.NavMeshAgent.desiredVelocity.sqrMagnitude)
                {
                    FootstepIsPlaying = false;
                }
            }
        }
    }

    public void FootSteps()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out cultistHit, Mathf.Infinity))
        {
            PhysMat_Last = PhysMat;
            PhysMat = cultistHit.collider.tag;

            if (PhysMat != PhysMat_Last)
            {
                AkSoundEngine.SetSwitch("Material", PhysMat, gameObject);

                print(PhysMat);
            }
        }

        AkSoundEngine.PostEvent("Play_Footsteps", gameObject);
    }
}
