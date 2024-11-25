using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float punchResetTime = 1.5f;
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 2f;
    public float oldMaxSpeed { get; set; }

    public bool IsAttacking { get; set; }
    private bool isKnockedBack = false;

    private Vector3 knockbackDirection;

    public Transform powerGlovesPos;

    [Header("Casting")]
    public float sphereRadius;
    public float combatDistance;
    public Ray sphereRay { get; set; }
    public RaycastHit sphereCastHit;
    public LayerMask enemyLayer;
    [SerializeField] private Camera cam;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFXPrefab;

    [Header("Others")]
    public WeaponHolder WeaponHolder { get; set; }
    public PlayerProfile PlayerProfile { get; set; }
    public PlayerController PlayerController { get; set; }
    public PlayerInteract PlayerInteract { get; set; }
    public PlayerCombatCamera PlayerCombatCamera { get; set; }
    public PlayerAnimation PlayerAnimation { get; set; }

    [Header("Debugging")]
    private bool isHit;

    [Header("Events")]
    public static UnityAction OnPerformBasicPunchAttackTriggered;
    public static UnityAction OnPerformSuperPunchAttackTriggered;
    public static UnityAction OnPerformBasicSwordAttackTriggered;
    public static UnityAction OnPerformDefendSwordTriggered;
    public static UnityAction OnFinishAttackTriggered;

    private void OnEnable()
    {
        OnPerformBasicPunchAttackTriggered += PerformBasicAttack;
        OnPerformSuperPunchAttackTriggered += PerformSuperAttack;

        OnPerformBasicSwordAttackTriggered += PerformBasicAttack;
        OnPerformDefendSwordTriggered += PerformSuperAttack;

        OnFinishAttackTriggered += FinishAttack;
    }

    private void OnDisable()
    {
        OnPerformBasicPunchAttackTriggered -= PerformBasicAttack;
        OnPerformSuperPunchAttackTriggered -= PerformSuperAttack;

        OnPerformBasicSwordAttackTriggered -= PerformBasicAttack;
        OnPerformDefendSwordTriggered -= PerformSuperAttack;

        OnFinishAttackTriggered -= FinishAttack;
    }

    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerProfile = GetComponent<PlayerProfile>();
        PlayerInteract = GetComponent<PlayerInteract>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        PlayerCombatCamera = GetComponent<PlayerCombatCamera>();
    }

    private void Start()
    {
        oldMaxSpeed = PlayerController.maxSpeed;
    }

    public Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }

    public bool WeaponCheckCastInfo() => Physics.SphereCast(sphereRay, sphereRadius, out sphereCastHit, combatDistance, enemyLayer, QueryTriggerInteraction.Collide);

    public void TargetUICast()
    {
        if (!WeaponHolder)
            return;

        if (Physics.SphereCast(sphereRay, sphereRadius, out sphereCastHit, combatDistance, enemyLayer, QueryTriggerInteraction.Collide))
        {
            GameManager.Instance.uIManager.playerCrosshairLine.SetActive(true);
            GameManager.Instance.uIManager.playerCrosshair.SetActive(false);
            isHit = true;
        }
        else
        {
            GameManager.Instance.uIManager.playerCrosshairLine.SetActive(false);
            GameManager.Instance.uIManager.playerCrosshair.SetActive(true);
            isHit = false;
        }
    }

    private void Update()
    {
        sphereRay = HandleCameraDirection();

        TargetUICast();
        InitAttack();
    }

    private void InitAttack()
    {
        if (IsAttacking || WeaponHolder == null)
        {
            Invoke("CanAttack", 2);
        }

        if(WeaponHolder != null)
        {
            if (WeaponHolder.weapon.PlayerCombat == null)
            {
                WeaponHolder.weapon.PlayerCombat = this;
            }
        }

        WeaponHolder.weapon.SetAnimationLayer();

        //Weapon currentWeapon = null;

        //switch(WeaponHolder.weapon.weaponSO.WeaponType)
        //{
        //    case WeaponSO.Weapons.PowerGlove:
        //        currentWeapon = (Gloves)WeaponHolder.weapon;
        //        break;
        //    case WeaponSO.Weapons.Sword:
        //        currentWeapon = (Sword)WeaponHolder.weapon;
        //        break;
        //    default:
        //        break;
        //}


        //if (currentWeapon == null)
        //{
        //    Debug.LogError("Current weapon can't be found");
        //    return;
        //}

        WeaponHolder.weapon.HandleFirstAttack();
        WeaponHolder.weapon.HandleSecondaryAttack();
    }

    private void PerformBasicAttack()
    {
        if(WeaponHolder == null)
        {
            return;
        }

        WeaponHolder.weapon.PerformFirstAttack();
    }

    private void PerformSuperAttack()
    {
        if (WeaponHolder == null)
        {
            return;
        }

        WeaponHolder.weapon.PerformSecondaryAttack();
    }

    private void FinishAttack()
    {
        if (WeaponHolder == null)
        {
            IsAttacking = false;
            return;
        }

        WeaponHolder.weapon.FinishAttack();
    }

    public void ApplyKnockback(Vector3 sourcePosition, NavMeshAgent agent)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;

            agent.isStopped = true;

            knockbackDirection = (sourcePosition - transform.position).normalized;
            knockbackDirection.y = 0;

            StartCoroutine(KnockbackRoutine(agent));
        }
    }

    public IEnumerator KnockbackRoutine(NavMeshAgent agent)
    {
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            agent.Move(knockbackDirection * knockbackSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isKnockedBack = false;
    }

    public void InitHitVFX(Vector3 point)
    {
       GameObject hitVFX = Instantiate(hitVFXPrefab, point, Quaternion.identity);
       hitVFX.GetComponent<ParticleSystem>().Play();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.cyan : Color.magenta;

        if (isHit)
        {
            Gizmos.DrawRay(sphereRay.origin, sphereCastHit.point - sphereRay.origin);
            Gizmos.DrawWireSphere(sphereCastHit.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction.normalized * combatDistance);
        }
    }

    void CanAttack()
    {
        IsAttacking = false;
    }
}
