using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerInteract : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float sphereRadius;
    private Ray sphereRay;
    private RaycastHit hitInfo;

    [Header("Interct")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float interactDistance;
    private bool isEquippedWeapon;

    [Header("Combat")]
    [SerializeField] private Transform powerGlovePos;
    [SerializeField] private LayerMask combatLayer;
    [SerializeField] private float combatDistance;
    private WeaponHolder weaponHolder;

    [Header("Others")]
    [SerializeField] private Camera cam;

    [Header("Debugging")]
    private bool isHit;

    public void Init()
    {
        sphereRay = HandleCameraDirection();

        HandleMelee();
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
            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, interactDistance, interactLayer))
            {
                isHit = true;
                Debug.Log("Interacting a weapon...");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    weaponHolder = hitInfo.collider.GetComponent<WeaponHolder>();
                    weaponHolder.transform.position = powerGlovePos.position;
                    weaponHolder.transform.SetParent(powerGlovePos);
                    weaponHolder.transform.GetComponent<Rigidbody>().isKinematic = true;
                    isEquippedWeapon = true;

                    Debug.Log("Succesfully equipped a weapon.");
                }
            }
            else
            {
                isHit = false;
            }
        }

        // Drop weapon
        if(weaponHolder)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                weaponHolder.transform.SetParent(null);
                weaponHolder.transform.GetComponent<Rigidbody>().isKinematic = false;
                weaponHolder = null;
                isEquippedWeapon = false;

                Debug.Log("Succesfully dropped a weapon");
            }
        }
    }

    private void HandleMelee()
    {
        if(weaponHolder)
        {
            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatDistance, combatLayer))
            {
                isHit = true;

                if (Input.GetMouseButtonDown(0))
                {
                    if (hitInfo.transform.TryGetComponent<EnemyHolder>(out EnemyHolder enemyHolder))
                    {
                        HandleMeleeType(enemyHolder);
                    }
                }
            }
            else
            {
                isHit = false;
            }
        }
        else
        {
            Debug.LogWarning("You haven't equiped a weapon");
        }
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
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction.normalized * interactDistance);
        }
    }
}
