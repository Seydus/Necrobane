using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class WeaponDataHolder
{
    public WeaponHolder weaponHolder { get; set; }
    public Weapon weapon { get; set; }

    public WeaponDataHolder(WeaponHolder weaponHolder, Weapon weapon)
    {
        this.weaponHolder = weaponHolder;
        this.weapon = weapon;
    }
}

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 2f;
    public float oldMaxSpeed { get; set; }
    private bool isSwitchWeapon = false;
    private int numberOfWeapons;

    public bool IsAttacking { get; set; }
    private bool isKnockedBack = false;
    private bool isAnimLayer;

    private Vector3 knockbackDirection;

    public Transform powerGlovesPos;


    public GameObject[] gloves;
    public GameObject[] sword;

    [Header("Casting")]
    public float sphereRadius;
    public float combatDistance;
    public Ray sphereRay { get; set; }
    public RaycastHit sphereCastHit;
    public LayerMask enemyLayer;
    [SerializeField] private Camera cam;

    private RaycastHit[] sphereCastHits;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFXPrefab;

    [Header("Others")]
    public List<WeaponDataHolder> WeaponHolderList = new List<WeaponDataHolder>();
    public WeaponHolder CurrentWeaponHolder { get; set; }
    public PlayerProfile PlayerProfile { get; set; }
    public PlayerController PlayerController { get; set; }
    public PlayerInteract PlayerInteract { get; set; }
    public PlayerCombatCamera PlayerCombatCamera { get; set; }
    public PlayerAnimation PlayerAnimation { get; set; }

    [Header("Debugging")]
    private bool isHit;

    [Header("Events")]
    public static UnityAction OnPerformFirstAttackTriggered;
    public static UnityAction OnPerformSecondaryAttackTriggered;
    public static UnityAction OnFinishAttackTriggered;

    private void OnEnable()
    {
        OnPerformFirstAttackTriggered += PerformBasicAttack;
        OnPerformSecondaryAttackTriggered += PerformSuperAttack;

        OnFinishAttackTriggered += FinishAttack;
    }

    private void OnDisable()
    {
        OnPerformFirstAttackTriggered -= PerformBasicAttack;
        OnPerformSecondaryAttackTriggered -= PerformSuperAttack;

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
        StartCoroutine(InitSwitchWeapon());
    }

    public Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position - transform.forward * 0.5f, cam.transform.forward);
    }

    public bool WeaponCheckCastInfo() => Physics.SphereCast(HandleCameraDirection(), sphereRadius, out sphereCastHit, combatDistance, enemyLayer, QueryTriggerInteraction.Collide);

    public RaycastHit[] GetAllColliderHit()
    {
        return Physics.SphereCastAll(HandleCameraDirection().origin, sphereRadius, HandleCameraDirection().direction, combatDistance, enemyLayer);
    }

    public void TargetUICast()
    {
        if (!CurrentWeaponHolder)
            return;

        if (WeaponCheckCastInfo())
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

    public void Init()
    {
        sphereRay = HandleCameraDirection();

        TargetUICast();
        InitAttack();
    }

    public void AddWeapon(WeaponDataHolder weapon)
    {
        numberOfWeapons++;

        if (numberOfWeapons <= 2)
        {
            WeaponHolderList.Add(weapon);
        }
    }

    private void SwitchWeapon()
    {
        if (WeaponHolderList.Count <= 0)
            return;

    }

    private void InitAttack()
    {
        if (!CurrentWeaponHolder)
            return;

        CurrentWeaponHolder.weapon.PlayerCombat = this;

        CurrentWeaponHolder.weapon.HandleFirstAttack();
        CurrentWeaponHolder.weapon.HandleSecondaryAttack();
        CurrentWeaponHolder.weapon.SetAnimationLayer();
    }

    private IEnumerator InitSwitchWeapon()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!IsAttacking)
                {
                    for (int i = 0; i < WeaponHolderList.Count; i++)
                    {
                        if (WeaponHolderList[i].weapon.weaponData.WeaponName == "LangesMesser")
                        {
                            CurrentWeaponHolder = WeaponHolderList[i].weaponHolder;
                            break;
                        }
                    }

                    if (CurrentWeaponHolder != null)
                    {
                        for (int i = 0; i < sword.Length; i++)
                        {
                            sword[i].SetActive(true);
                        }

                        for (int i = 0; i < gloves.Length; i++)
                        {
                            gloves[i].SetActive(false);
                        }
                    }

                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!IsAttacking)
                {
                    for (int i = 0; i < WeaponHolderList.Count; i++)
                    {
                        if (WeaponHolderList[i].weapon.weaponData.WeaponName == "Gloves")
                        {
                            CurrentWeaponHolder = WeaponHolderList[i].weaponHolder;
                            break;
                        }
                    }

                    if (CurrentWeaponHolder != null)
                    {
                        for (int i = 0; i < gloves.Length; i++)
                        {
                            gloves[i].SetActive(true);

                        }

                        for (int j = 0; j < sword.Length; j++)
                        {
                            sword[j].SetActive(false);
                        }
                    }

                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return null;
        }
    }

    private void PerformBasicAttack()
    {
        if (!CurrentWeaponHolder)
            return;

        CurrentWeaponHolder.weapon.PerformFirstAttack();
    }

    private void PerformSuperAttack()
    {
        if (!CurrentWeaponHolder)
            return;

        CurrentWeaponHolder.weapon.PerformSecondaryAttack();
    }

    private void FinishAttack()
    {
        if (!CurrentWeaponHolder)
        {
            IsAttacking = false;
            return;
        }

        CurrentWeaponHolder.weapon.FinishAttack();
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
        if (agent)
        {
            float elapsed = 0f;

            while (elapsed < knockbackDuration)
            {
                if (agent.isOnNavMesh == false)
                    yield break;


                if (agent.isOnNavMesh)
                {
                    agent.Move(knockbackDirection * knockbackSpeed * Time.deltaTime);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            agent.isStopped = false;
            isKnockedBack = false;
        }

        yield return null;
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
