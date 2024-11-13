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

    [Header("Combat Anim")]
    private bool isRightPunchNext = true;
    private bool isPunching = false;
    public float punchResetTime = 1.5f; // Time in seconds to reset punch sequence
    private float timeSinceLastPunch = 0f;


    [Header("Others")]
    [SerializeField] private Camera cam;

    [Header("Debugging")]
    private bool isHit;

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

            bool sphereCast = Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatDistance, combatLayer, QueryTriggerInteraction.Collide);

            if (Input.GetMouseButtonDown(0) && !isPunching)
            {
                if (timeSinceLastPunch > punchResetTime)
                {
                    isRightPunchNext = true;
                }

                StartCoroutine(PerformPunch());

                if (sphereCast)
                {
                    // Punched hit enemy
                    if (hitInfo.transform.TryGetComponent<EnemyHolder>(out EnemyHolder enemyHolder))
                    {
                        HandleMeleeType(enemyHolder);
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

    private IEnumerator PerformPunch()
    {
        isPunching = true;
        timeSinceLastPunch = 0f;

        if (isRightPunchNext)
        {
            weaponHolder.GetAnimator().SetTrigger("RightPunch");
        }
        else
        {
            weaponHolder.GetAnimator().SetTrigger("LeftPunch");
        }


        isRightPunchNext = !isRightPunchNext;
        yield return new WaitForSeconds(GetCurrentAnimationLength());

        isPunching = false;
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = originalPosition;
    }


    private float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = weaponHolder.GetAnimator().GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    private void HandleMeleeType(EnemyHolder enemyHolder)
    {
        switch (weaponHolder.weapon.WeaponType)
        {
            case Weapon.Weapons.PowerGlove:
                HandleHandCombat(enemyHolder);
                break;
            case Weapon.Weapons.Sword:
                HandleSwordCombat(enemyHolder);
                break;
            case Weapon.Weapons.Axe:
                HandleSwordCombat(enemyHolder);
                break;
            case Weapon.Weapons.Dagger:
                HandleSwordCombat(enemyHolder);
                break;
        }
    }

    private void HandleHandCombat(EnemyHolder enemyHolder)
    {
        Debug.Log("Hand Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(weaponHolder.weapon.WeaponDamage);
        StartCoroutine(CameraShake(0.08f, 0.03f));
        GameObject hitVFX = Instantiate(hitVFXPrefab, hitInfo.point, Quaternion.identity);
        hitVFX.GetComponent<ParticleSystem>().Play();
    }

    private void HandleSwordCombat(EnemyHolder enemyHolder)
    {
        Debug.Log("Sword Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(weaponHolder.weapon.WeaponDamage);
    }

    private void HandleAxeCombat(EnemyHolder enemyHolder)
    {
        Debug.Log("Axe Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(weaponHolder.weapon.WeaponDamage);
    }

    private void HandleDaggerCombat(EnemyHolder enemyHolder)
    {
        Debug.Log("Dagger Combat Triggered");
        enemyHolder.EnemyProfile.DeductHealth(weaponHolder.weapon.WeaponDamage);
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
