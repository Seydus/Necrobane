using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
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
    private WeaponHolder weaponHolder;
    private ItemHolder itemHolder;
    [SerializeField] private GameObject hitVFXPrefab;
    private bool isSuperAttack;
    private bool isBasicAttack;
    bool sphereCast;

    private Vector3 originalPosition;

    [Header("Combat Anim")]
    private bool isRightPunchNext = true;
    private bool isPunching = false;
    public float punchResetTime = 1.5f;
    private float timeSinceLastPunch = 0f;


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
                isSuperAttack = true;

                StartCoroutine(PerformSuperAttack());
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

        if (isRightPunchNext)
        {
            weaponHolder.GetAnimator().SetTrigger("RightPunch");
            // StartCoroutine(CameraShake(0.07f, 0.05f, Vector3.right));
        }
        else
        {
            weaponHolder.GetAnimator().SetTrigger("LeftPunch");
            // StartCoroutine(CameraShake(0.07f, 0.05f, Vector3.left));
        }

        yield return null;

        isRightPunchNext = !isRightPunchNext;
        yield return new WaitForSeconds(GetCurrentAnimationLength());

        if (sphereCast)
        {
            if (hitInfo.transform.TryGetComponent<EnemyHolder>(out EnemyHolder enemyHolder))
            {
                HandleMeleeType(enemyHolder, weaponHolder.weapon.WeaponBasicDamage);
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

        weaponHolder.GetAnimator().SetTrigger("SuperPunch");

        yield return null;

        isRightPunchNext = !isRightPunchNext;
        yield return new WaitForSeconds(GetCurrentAnimationLength());

        if (sphereCast)
        {
            if (hitInfo.transform.TryGetComponent<EnemyHolder>(out EnemyHolder enemyHolder))
            {
                HandleMeleeType(enemyHolder, weaponHolder.weapon.WeaponSuperAttackDamage);
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


    private float GetCurrentAnimationLength()
    {
        Debug.Log(weaponHolder.GetAnimator().GetCurrentAnimatorStateInfo(0).length);
        return weaponHolder.GetAnimator().GetCurrentAnimatorStateInfo(0).length;
    }

    private void HandleMeleeType(EnemyHolder enemyHolder, float damage)
    {
        switch (weaponHolder.weapon.WeaponType)
        {
            case Weapon.Weapons.PowerGlove:
                HandleHandCombat(enemyHolder, damage);
                break;
            case Weapon.Weapons.Sword:
                HandleSwordCombat(enemyHolder, damage);
                break;
            case Weapon.Weapons.Axe:
                HandleSwordCombat(enemyHolder, damage);
                break;
            case Weapon.Weapons.Dagger:
                HandleSwordCombat(enemyHolder, damage);
                break;
        }
    }

    private void HandleHandCombat(EnemyHolder enemyHolder, float damage)
    {
        Debug.Log("Hand Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(damage);

        Vector3 punchDirection = isRightPunchNext ? Vector3.right : Vector3.left;

        // StartCoroutine(CameraShake(0.03f, 0.01f, punchDirection));

        GameObject hitVFX = Instantiate(hitVFXPrefab, hitInfo.point, Quaternion.identity);
        hitVFX.GetComponent<ParticleSystem>().Play();
        cam.transform.localPosition = originalPosition;
    }


    private void HandleSwordCombat(EnemyHolder enemyHolder, float damage)
    {
        Debug.Log("Sword Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(damage);
    }

    private void HandleAxeCombat(EnemyHolder enemyHolder, float damage)
    {
        Debug.Log("Axe Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(damage);
    }

    private void HandleDaggerCombat(EnemyHolder enemyHolder, float damage)
    {
        Debug.Log("Dagger Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(damage);
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
