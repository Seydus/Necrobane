using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class SphereCastInfo
{
    public float sphereRadius;
    public Ray sphereRay;
    public RaycastHit hitInfo;
}

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private SphereCastInfo sphereCastInfo;

    [Header("Interact")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float interactDistance;

    private bool isEquipped;

    [Header("Others")]
    [SerializeField] private Camera cam;

    private PlayerCombat playerCombat;
    private WeaponHolder weaponHolder;
    private ItemHolder itemHolder;

    [Header("Debugging")]
    private bool isHit;

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void Init()
    {
        HandleInteract();

        playerCombat.HandleAttack(weaponHolder);
    }

    public Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }

    private void HandleInteract()
    {
        sphereCastInfo.sphereRay = HandleCameraDirection();

        if (!isEquipped)
        {
            EquipTargetCast();

            GameManager.Instance.uIManager.playerDropTxt.SetActive(false);
        }
        else
        {
            GameManager.Instance.uIManager.playerDropTxt.SetActive(true);
        }

        DropWeapon();
        DropItem();
    }

    private void EquipTargetCast()
    {
        if (Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, interactDistance, interactLayer))
        {
            isHit = true;

            EqupWeapon();
            EquipItem();

            GameManager.Instance.uIManager.playerCrosshairLine.SetActive(!isEquipped ? true : false);
            GameManager.Instance.uIManager.playerCrosshair.SetActive(!isEquipped ? false : true);

            Debug.Log("Interacting a weapon...");
        }
        else
        {
            isHit = false;

            GameManager.Instance.uIManager.playerCrosshairLine.SetActive(false);
            GameManager.Instance.uIManager.playerCrosshair.SetActive(true);
        }
    }

    private void EqupWeapon()
    {
        if ((Input.GetKeyDown(KeyCode.E)) && sphereCastInfo.hitInfo.transform.TryGetComponent<WeaponHolder>(out WeaponHolder _weaponHolder))
        {
            isEquipped = true;

            weaponHolder = _weaponHolder;
            weaponHolder.SetMeshState(false);
            weaponHolder.SetBoxCollider(false);
            weaponHolder.SetRigidbodyKinematic(true);
            weaponHolder.transform.SetParent(playerCombat.PowerGlovesPos);
            weaponHolder.SetPosition(playerCombat.PowerGlovesPos.position);
            weaponHolder.SetRotation(Vector3.zero);

            AkSoundEngine.PostEvent("Play_Equip_Fist", gameObject);
            Debug.Log("Succesfully equipped a weapon.");
        }
    }

    private void EquipItem()
    {
        if ((Input.GetKeyDown(KeyCode.E)) && sphereCastInfo.hitInfo.transform.TryGetComponent<ItemHolder>(out ItemHolder _itemHolder))
        {
            itemHolder = _itemHolder;
            itemHolder.SetBoxCollider(false);
            itemHolder.SetRigidbodyKinematic(true);
            itemHolder.transform.SetParent(playerCombat.PowerGlovesPos);
            itemHolder.SetRotation(Vector3.zero);

            AkSoundEngine.PostEvent("Play_Equip_Fist", gameObject);
            Debug.Log("Succesfully equipped an item.");
        }
    }

    private void DropWeapon()
    {
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
                isEquipped = false;
            }
        }
    }

    private void DropItem()
    {
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
                isEquipped = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isHit ? Color.green : Color.red;

        if (isHit)
        {
            Gizmos.DrawRay(sphereCastInfo.sphereRay.origin, sphereCastInfo.hitInfo.point - sphereCastInfo.sphereRay.origin);
            Gizmos.DrawWireSphere(sphereCastInfo.hitInfo.point, sphereCastInfo.sphereRadius);
        }
        else
        {
            Gizmos.DrawRay(sphereCastInfo.sphereRay.origin, sphereCastInfo.sphereRay.direction.normalized * interactDistance);
        }
    }
}