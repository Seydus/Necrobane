using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


// Becareful not to use the the first-capital letter variables
public class Skeleton : Enemy, IEnemyRoaming, IEnemyCombat
{

    IEnemyCombat enemyCombat;
    IEnemyRoaming enemyRoaming;

    private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Transform weaponPos;

    [Header("Events")]
    public UnityEvent OnPerformAttackTriggered = new UnityEvent();
    public UnityEvent OnFinishAttackTriggered = new UnityEvent();

    private Ray sphereSkeletonRay;
    private RaycastHit skeletonHit;

    #region Misc
    public float AttackSpeed { get; set; }
    public float SphereRadius { get; set; }
    public float MaxDistance { get; set; }
    public LayerMask CombatLayer { get; set; }
    public float RotateSpeed { get; set; }
    public AK.Wwise.Event _HitPlayer { get; set; }
    public float MinRoamWaitTime { get; set; }
    public float MinRoamDistance { get; set; }
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
    public float AttackDelay { get; set; }
    public float RoamingRotateSpeed { get; set; }
    public float RoamingMoveSpeed { get; set; }
    public bool isBlockedByWall { get; set; }
    #endregion

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

        skeletonAnimation = GetComponent<SkeletonAnimation>();

        OnPerformAttackTriggered.AddListener(PerformAttack);
        OnFinishAttackTriggered.AddListener(FinishAttack);
    }

    private void OnDestroy()
    {
        OnPerformAttackTriggered.RemoveListener(PerformAttack);
        OnFinishAttackTriggered.RemoveListener(FinishAttack);
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
        return new Ray(weaponPos.position + transform.forward * -0.3f, transform.forward);
    }

    public void HandleAttack(NavMeshAgent agent, float range)
    {
        enemyCombat.HandleAttack(agent, range);
    }

    public IEnumerator InitAttack(float delay)
    {
        enemyRoaming.NavMeshAgent.velocity = Vector3.zero;
        isAttacking = true;
        enemyCombat.IsAttacking = true;
        yield return new WaitForSeconds(delay);

        skeletonAnimation.SkeletonAttack(true);
    }

    public void PerformAttack()
    {
        sphereSkeletonRay = GetEnemyDirection();

        if (Physics.SphereCast(sphereSkeletonRay, sphereRadius, out skeletonHit, maxDistance, combatLayer))
        {
            EnemyHit = true;

            if (skeletonHit.transform.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
            {
                if(playerManager.PlayerProfile.isDefending)
                {
                    if (playerManager.PlayerCombat.WeaponHolder.weapon == null)
                        return;

                    playerManager.PlayerProfile.DeductStamina(playerManager.PlayerCombat.WeaponHolder.weapon.weaponSO.WeaponStaminaCost);
                }
                else
                {
                    playerManager.PlayerProfile.DeductHealth(EnemyDamage);
                }

                Debug.Log("Player hit");
                AkSoundEngine.PostEvent("Play_Chops", gameObject);

                if (playerManager.transform != null)
                {
                    // _HitPlayer.Post(playerManager.transform.gameObject);
                }
            }
        }
    }

    private void FinishAttack()
    {
        EnemyHit = false;
        skeletonAnimation.SkeletonAttack(false);
        isAttacking = false;
        enemyCombat.IsAttacking = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = EnemyHit ? Color.green : Color.red;

        sphereSkeletonRay = GetEnemyDirection();

        if (EnemyHit)
        {
            Gizmos.DrawRay(sphereSkeletonRay.origin, skeletonHit.point - sphereSkeletonRay.origin);
            Gizmos.DrawWireSphere(skeletonHit.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereSkeletonRay.origin, sphereSkeletonRay.direction * maxDistance);
        }
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
                if (Time.time - LastFootstepTime > 0.7f / enemyRoaming.NavMeshAgent.desiredVelocity.sqrMagnitude)
                {
                    FootstepIsPlaying = false;
                }
            }
        }
    }


    public void FootSteps()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out skeletonHit, Mathf.Infinity))
        {
            PhysMat_Last = PhysMat;

            PhysMat = skeletonHit.collider.tag;

            if (PhysMat != PhysMat_Last)
            {
                AkSoundEngine.SetSwitch("Material", PhysMat, gameObject);

                print(PhysMat);
            }
        }

        AkSoundEngine.PostEvent("Play_BoneSteps", gameObject);
    }
}
