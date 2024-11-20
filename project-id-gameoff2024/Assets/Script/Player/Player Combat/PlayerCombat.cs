using UnityEditor.PackageManager;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float punchResetTime = 1.5f;
    [SerializeField] private float combatDistance;
    private float timeSinceLastBasicPunch = 0f;

    private bool isSuperAttack = false;
    private bool isBasicAttack = false;
    private bool sphereCast = false;
    private bool isRightPunchNext = true;

    [Header("Debugging")]
    private bool isHit;

    [Header("Others")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private Transform powerGlovesPos;

    private PlayerProfile playerProfile;
    private PlayerInteract playerInteract;
    private PlayerAnimation playerAnim;
    private PlayerCombatCamera playerCombatCam;

    private SphereCastInfo sphereCastInfo;
    private WeaponHolder weaponHolder;

    public Transform PowerGlovesPos { get => powerGlovesPos; private set => powerGlovesPos = value; }

    [Header("Events")]
    public static UnityAction OnInitBasicAttackTriggered;
    public static UnityAction OnInitSuperAttackTriggered;
    public static UnityAction OnPerformBasicAttackTriggered;
    public static UnityAction OnPerformSuperAttackTriggered;

    private void OnEnable()
    {
        OnInitBasicAttackTriggered += InitBasicAttackEvent;
        OnPerformBasicAttackTriggered += PerformBasicAttackEvent;
        
        OnInitSuperAttackTriggered += InitSuperAttackEvent;
        OnPerformSuperAttackTriggered += PerformSuperAttackEvent;
    }

    private void OnDisable()
    {
        OnInitBasicAttackTriggered -= InitBasicAttackEvent;
        OnPerformBasicAttackTriggered -= PerformBasicAttackEvent;

        OnInitSuperAttackTriggered -= InitSuperAttackEvent;
        OnPerformSuperAttackTriggered -= PerformSuperAttackEvent;
    }

    private void Awake()
    {
        playerProfile = GetComponent<PlayerProfile>();
        playerInteract = GetComponent<PlayerInteract>();
        playerAnim = GetComponent<PlayerAnimation>();
        playerCombatCam = GetComponent<PlayerCombatCamera>();
    }

    public void HandleAttack(SphereCastInfo _sphereCastInfo, WeaponHolder _weaponHolder)
    {
        sphereCastInfo = _sphereCastInfo;
        weaponHolder = _weaponHolder;

        if (weaponHolder)
        {
            if (isBasicAttack || isSuperAttack)
                return;

            TargetCast();

            TryBasicAttack();
            TrySuperAttack();
        }
        else
        {
            Debug.LogWarning("You haven't equiped a weapon");
        }
    }

    private void TargetCast()
    {
        sphereCast = Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, combatDistance, enemyLayer, QueryTriggerInteraction.Collide);

        if (sphereCast)
        {
            GameManager.Instance.uIManager.playerAttackTxt.SetActive(true);
            isHit = true;
        }
        else
        {
            GameManager.Instance.uIManager.playerAttackTxt.SetActive(false);
            isHit = false;
        }
    }

    private void TryBasicAttack()
    {
        timeSinceLastBasicPunch += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            isBasicAttack = true;

            if (timeSinceLastBasicPunch > punchResetTime)
            {
                isRightPunchNext = true;
            }

            // Triggers the animation event for super attack
            playerAnim.PeformBasicAttackAnim(isRightPunchNext);
        }
    }

    private void TrySuperAttack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (playerProfile.playerStamina > 0)
            {
                isSuperAttack = true;

                // Triggers the animation event for super attack
                playerAnim.PerformSuperAttackAnim();
            }
            else
            {
                Debug.Log("You don't have enough mana.");
            }
        }
    }

    private void InitBasicAttackEvent()
    {
        timeSinceLastBasicPunch = 0f;
        isRightPunchNext = !isRightPunchNext;
    }
    private void PerformBasicAttackEvent()
    {
        PerformBasicAttack();
        isBasicAttack = false;
    }

    private void InitSuperAttackEvent()
    {
        timeSinceLastBasicPunch = 0f;
        playerProfile.DeductStamina(weaponHolder.weapon.WeaponStaminaCost);
        isRightPunchNext = !isRightPunchNext;
    }

    private void PerformSuperAttackEvent()
    {
        PerformSuperAttack();

        isSuperAttack = false;
    }

    private void PerformBasicAttack()
    {
        if (sphereCast)
        {
            if (sphereCastInfo.hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponBasicDamage);
                StartCoroutine(playerCombatCam.CameraShake(new CameraCombatInfo(0.15f, 0.015f, Vector3.zero)));
                AkSoundEngine.PostEvent("Play_HitBones", gameObject);
            }
        }
        else
        {
            if (Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, combatDistance))
            {
                GameObject hitVFX = Instantiate(hitVFXPrefab, sphereCastInfo.hitInfo.point, Quaternion.identity);
                hitVFX.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    private void PerformSuperAttack()
    {
        if (sphereCast)
        {
            if (sphereCastInfo.hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponSuperAttackDamage);
                StartCoroutine(playerCombatCam.CameraShake(new CameraCombatInfo(0.15f, 0.025f, Vector3.zero)));
                AkSoundEngine.PostEvent("Play_HitBones", gameObject);
            }
        }
        else
        {
            if (Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, combatDistance))
            {
                GameObject hitVFX = Instantiate(hitVFXPrefab, sphereCastInfo.hitInfo.point, Quaternion.identity);
                hitVFX.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    private void HandleMeleeType(Enemy enemy, float damage)
    {
        switch (weaponHolder.weapon.WeaponType)
        {
            case Weapon.Weapons.PowerGlove:
                HandleHandCombat(enemy, damage);
                break;
            case Weapon.Weapons.Sword:
                HandleSwordCombat(enemy, damage);
                break;
            case Weapon.Weapons.Axe:
                HandleSwordCombat(enemy, damage);
                break;
            case Weapon.Weapons.Dagger:
                HandleSwordCombat(enemy, damage);
                break;
        }
    }

    private void HandleHandCombat(Enemy enemy, float damage)
    {
        Debug.Log("Hand Combat Triggered");
        enemy.DeductHealth(damage);

        Vector3 punchDirection = isRightPunchNext ? Vector3.right : Vector3.left;

        GameObject hitVFX = Instantiate(hitVFXPrefab, sphereCastInfo.hitInfo.point, Quaternion.identity);
        hitVFX.GetComponent<ParticleSystem>().Play();
    }

    private void HandleSwordCombat(Enemy enemy, float damage)
    {
        Debug.Log("Sword Combat Triggered");
        enemy.DeductHealth(damage);
    }

    private void HandleAxeCombat(Enemy enemy, float damage)
    {
        Debug.Log("Axe Combat Triggered");
        enemy.DeductHealth(damage);
    }

    private void HandleDaggerCombat(Enemy enemy, float damage)
    {
        Debug.Log("Dagger Combat Triggered");
        enemy.DeductHealth(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.cyan : Color.magenta;

        if (sphereCastInfo == null)
        {
            return;
        }

        if (isHit)
        {
            Gizmos.DrawRay(sphereCastInfo.sphereRay.origin, sphereCastInfo.hitInfo.point - sphereCastInfo.sphereRay.origin);
            Gizmos.DrawWireSphere(sphereCastInfo.hitInfo.point, sphereCastInfo.sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereCastInfo.sphereRay.origin, sphereCastInfo.sphereRay.direction.normalized * combatDistance);
        }
    }
}
