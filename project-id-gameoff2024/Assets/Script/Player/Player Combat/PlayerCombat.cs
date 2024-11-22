using UnityEngine;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float punchResetTime = 1.5f;
    [SerializeField] private float combatDistance;
    private float timeSinceLastBasicPunch = 0f;
    private float oldMaxSpeed;

    private bool isSuperAttack = false;
    private bool isBasicAttack = false;
    private bool sphereCast = false;
    private bool isRightPunchNext = true;

    [Header("Debugging")]
    private bool isHit;

    [Header("Others")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private Transform powerGlovesPos;
    [SerializeField] private Camera cam;

    private PlayerController playerController;
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
        playerController = GetComponent<PlayerController>();
        playerProfile = GetComponent<PlayerProfile>();
        playerInteract = GetComponent<PlayerInteract>();
        playerAnim = GetComponent<PlayerAnimation>();
        playerCombatCam = GetComponent<PlayerCombatCamera>();
    }

    private void Start()
    {
        oldMaxSpeed = playerController.maxSpeed;
        sphereCastInfo = new SphereCastInfo();
    }

    public Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }

    public void HandleAttack(WeaponHolder weaponHolder)
    {
        this.weaponHolder = weaponHolder;
        sphereCastInfo.sphereRay = HandleCameraDirection();

        if (this.weaponHolder)
        {
            if (isBasicAttack || isSuperAttack)
                return;

            TargetCast();

            Debug.Log(sphereCastInfo.sphereRay);

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

    private void TryBasicAttack()
    {
        timeSinceLastBasicPunch += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            isBasicAttack = true;
            playerController.maxSpeed /= 2f;

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
            if (playerProfile.playerStamina >= weaponHolder.weapon.WeaponStaminaCost)
            {
                isSuperAttack = true;
                // Triggers the animation event for super attack
                playerController.maxSpeed /= 2;
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
        sphereCast = Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, combatDistance, enemyLayer, QueryTriggerInteraction.Collide);

        if (sphereCast)
        {
            if (sphereCastInfo.hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponBasicDamage);
                StartCoroutine(playerCombatCam.CameraShake(new CameraCombatInfo(0.25f, 0.025f, Vector3.zero)));
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

        playerController.maxSpeed = oldMaxSpeed;
    }

    private void PerformSuperAttack()
    {
        sphereCast = Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, combatDistance, enemyLayer, QueryTriggerInteraction.Collide);

        if (sphereCast)
        {
            if (sphereCastInfo.hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponSuperAttackDamage);
                StartCoroutine(playerCombatCam.CameraShake(new CameraCombatInfo(0.25f, 0.025f, Vector3.zero)));
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

        playerController.maxSpeed = oldMaxSpeed;
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
