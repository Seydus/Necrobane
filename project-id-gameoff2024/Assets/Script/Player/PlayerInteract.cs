using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PlayerInteract : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float sphereRadius;
    private Ray sphereRay;
    private RaycastHit hitInfo;

    [Header("Interact")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float interactDistance;
    private bool isEquippedWeapon;

    [Header("Combat")]
    [SerializeField] private Transform powerGlovePos;
    [SerializeField] private LayerMask combatLayer;
    [SerializeField] private float combatDistance;
    [SerializeField] private float initialForce;
    [SerializeField] private float collisionTime;
    private WeaponHolder weaponHolder;
    private ItemHolder itemHolder;
    [SerializeField] private GameObject hitVFXPrefab;
    private bool isSuperAttack;
    private bool isBasicAttack;
    bool sphereCast;
    private Coroutine knockbackRoutine;

    private Vector3 originalPosition;

    [Header("Combat Anim")]
    private bool isRightPunchNext = true;
    private bool isPunching = false;
    public float punchResetTime = 1.5f;
    private float timeSinceLastPunch = 0f;
    [SerializeField] private Animator anim;

    [Header("Others")]
    [SerializeField] private Camera cam;
    private PlayerProfile playerProfile;

    [Header("Debugging")]
    private bool isHit;

    private void Awake()
    {
        playerProfile = GetComponent<PlayerProfile>();
    }

    private void Start()
    {
        originalPosition = cam.transform.localPosition;
    }

    public void Init()
    {
        sphereRay = HandleCameraDirection();

        HandleAttack();
        HandleInteract();
    }

    private Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }

    private void HandleInteract()
    {
        if (!isEquippedWeapon)
        {
            // Equip weapon
            GameManager.Instance.uIManager.playerDropTxt.gameObject.SetActive(false);
            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, interactDistance, interactLayer))
            {
                GameManager.Instance.uIManager.playerGrabTxt.gameObject.SetActive(true);
                isHit = true;
                Debug.Log("Interacting a weapon...");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hitInfo.transform.TryGetComponent<WeaponHolder>(out WeaponHolder _weaponHolder))
                    {
                        weaponHolder = _weaponHolder;
                        weaponHolder.SetMeshState(false);
                        weaponHolder.SetBoxCollider(false);
                        weaponHolder.SetRigidbodyKinematic(true);
                        weaponHolder.transform.SetParent(powerGlovePos);
                        weaponHolder.SetPosition(powerGlovePos.position);
                        weaponHolder.SetRotation(Vector3.zero);
                    }
                    else if (hitInfo.transform.TryGetComponent<ItemHolder>(out ItemHolder _itemHolder))
                    {
                        itemHolder = _itemHolder;
                        itemHolder.SetBoxCollider(false);
                        itemHolder.SetRigidbodyKinematic(true);
                        itemHolder.transform.SetParent(powerGlovePos);
                        itemHolder.SetRotation(Vector3.zero);
                    }

                    isEquippedWeapon = true;
                    AkSoundEngine.PostEvent("Play_Equip_Fist", gameObject);

                    Debug.Log("Succesfully equipped a weapon.");
                }
            }
            else
            {
                GameManager.Instance.uIManager.playerGrabTxt.gameObject.SetActive(false);
                isHit = false;
            }
        }
        else
        {
            GameManager.Instance.uIManager.playerGrabTxt.gameObject.SetActive(false);
            GameManager.Instance.uIManager.playerDropTxt.gameObject.SetActive(true);
        }

        // Drop weapon
        if (weaponHolder)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Succesfully dropped a weapon");
                weaponHolder.SetBoxCollider(true);
                weaponHolder.SetRigidbodyKinematic(false);
                weaponHolder.SetMeshState(true);

                AkSoundEngine.PostEvent("Play_Drop_Item", gameObject);

                weaponHolder.transform.SetParent(null);
                weaponHolder = null;
                isEquippedWeapon = false;
            }
        }

        if (itemHolder)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Succesfully dropped an item");
                itemHolder.SetBoxCollider(true);
                itemHolder.SetRigidbodyKinematic(false);

                AkSoundEngine.PostEvent("Play_Drop_Item", gameObject);

                itemHolder.transform.SetParent(null);
                itemHolder = null;
                isEquippedWeapon = false;
            }
        }
    }
    private void HandleAttack()
    {
        if (weaponHolder)
        {
            timeSinceLastPunch += Time.deltaTime;

            sphereCast = Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatDistance, combatLayer, QueryTriggerInteraction.Collide);

            if (Input.GetMouseButtonDown(0) && !isPunching && !isSuperAttack)
            {
                isBasicAttack = true;

                if (timeSinceLastPunch > punchResetTime)
                {
                    isRightPunchNext = true;
                }

                StartCoroutine(PerformBasicAttack());
            }

            if (Input.GetMouseButtonDown(1) && !isPunching && !isBasicAttack)
            {
                if(playerProfile.playerStamina > 0)
                {
                    isSuperAttack = true;
                    StartCoroutine(PerformSuperAttack());
                }
                else
                {
                    Debug.Log("You don't have enough mana.");
                }
            }

            if(sphereCast)
            {
                GameManager.Instance.uIManager.playerAttackTxt.gameObject.SetActive(true);
                isHit = true;
            }
            else
            {
                GameManager.Instance.uIManager.playerAttackTxt.gameObject.SetActive(false);
                isHit = false;
            }
        }
        else
        {
            GameManager.Instance.uIManager.playerGrabTxt.gameObject.SetActive(false);
            Debug.LogWarning("You haven't equiped a weapon");
        }
    }

    private IEnumerator PerformBasicAttack()
    {
        isPunching = true;
        timeSinceLastPunch = 0f;

        Vector3 swingDirection = isRightPunchNext ? Vector3.left : Vector3.right;
        // StartCoroutine(CameraSwingShake(0.13f, 0.03f, swingDirection));

        if (isRightPunchNext)
        {
            anim.SetTrigger("RightPunch");
        }
        else
        {
            anim.SetTrigger("LeftPunch");
        }

        yield return null;

        isRightPunchNext = !isRightPunchNext;
        yield return new WaitForSeconds(/*GetCurrentAnimationLength()*/ 0.1f);

        if (sphereCast)
        {
            if (hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponBasicDamage);
                StartCoroutine(CameraShake(0.15f, 0.015f));
            }
        }
        else
        {
            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatDistance))
            {
                GameObject hitVFX = Instantiate(hitVFXPrefab, hitInfo.point, Quaternion.identity);
                hitVFX.GetComponent<ParticleSystem>().Play();
            }
        }

        isPunching = false;
        isBasicAttack = false;
    }

    private IEnumerator PerformSuperAttack()
    {
        playerProfile.DeductStamina(weaponHolder.weapon.WeaponStaminaCost);
        isPunching = true;
        timeSinceLastPunch = 0f;

        anim.SetTrigger("SuperPunch");

        // Start a more intense camera shake for the super attack
        // StartCoroutine(CameraSwingShake(2.3f, 0.1f, Vector3.right));
        yield return null;

        isRightPunchNext = !isRightPunchNext;

        yield return new WaitForSeconds(GetCurrentAnimationLength());

        if (sphereCast)
        {
            if (hitInfo.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                HandleMeleeType(enemy, weaponHolder.weapon.WeaponSuperAttackDamage);
                StartCoroutine(CameraShake(0.3f, 0.05f));
            }
        }
        else
        {
            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatDistance))
            {
                GameObject hitVFX = Instantiate(hitVFXPrefab, hitInfo.point, Quaternion.identity);
                hitVFX.GetComponent<ParticleSystem>().Play();
            }
        }

        isPunching = false;
        isSuperAttack = false;
    }

    private IEnumerator CameraSwingShake(float duration, float magnitude, Vector3 direction)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float swing = Mathf.Sin(elapsed * Mathf.PI / duration) * magnitude;
            cam.transform.localPosition = originalPos + direction * swing;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }


    private float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
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

        // StartCoroutine(CameraShake(0.03f, 0.01f, punchDirection));

        GameObject hitVFX = Instantiate(hitVFXPrefab, hitInfo.point, Quaternion.identity);
        hitVFX.GetComponent<ParticleSystem>().Play();
        cam.transform.localPosition = originalPosition;
        // Knockback(initialForce, collisionTime, enemyHolder);
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

    public void Knockback(float initialForce, float collisionTime, Enemy enemy)
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

        knockbackRoutine = StartCoroutine(ApplyKnockback(initialForce, collisionTime, enemy));
    }

    private IEnumerator ApplyKnockback(float baseForce, float duration, Enemy enemy)
    {
        Vector3 direction = (enemy.transform.position - transform.position).normalized;

        float distance = Vector3.Distance(enemy.transform.position, transform.position);

        float adjustedForce = baseForce * Mathf.Clamp(1f / distance, 0.1f, 10f);

        var navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        var rigidbody = enemy.GetComponent<Rigidbody>();

        navMeshAgent.enabled = false;
        rigidbody.isKinematic = false;

        rigidbody.AddForce(direction * adjustedForce, ForceMode.Impulse);

        yield return new WaitForSeconds(duration);

        navMeshAgent.enabled = true;
        rigidbody.isKinematic = true;
    }

    private void HandleDaggerCombat(Enemy enemy, float damage)
    {
        Debug.Log("Dagger Combat Triggered");
        enemy.DeductHealth(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        if (isHit)
        {
            Gizmos.DrawRay(sphereRay.origin, hitInfo.point - sphereRay.origin);
            Gizmos.DrawWireSphere(hitInfo.point, sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction.normalized * combatDistance);
        }
    }
}
