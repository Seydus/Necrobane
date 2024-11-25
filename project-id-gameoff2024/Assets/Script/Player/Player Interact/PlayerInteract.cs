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
    private ItemHolder itemHolder;

    [Header("Debugging")]
    private bool isHit;

    public GameObject[] gloves;
    public GameObject[] sword;
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void Init()
    {
        HandleInteract();
        TargetUICast();
    }

    public void TargetUICast()
    {
        if (isEquipped)
            return;

        if (Physics.SphereCast(sphereCastInfo.sphereRay, sphereCastInfo.sphereRadius, out sphereCastInfo.hitInfo, interactDistance, interactLayer))
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
            Debug.Log("Interacting a weapon...");
        }
        else
        {
            isHit = false;
        }
    }

    private void EqupWeapon()
    {
        if ((Input.GetKeyDown(KeyCode.E)) && sphereCastInfo.hitInfo.transform.TryGetComponent<WeaponHolder>(out WeaponHolder weaponHolder))
        {
            

            isEquipped = true;

            playerCombat.WeaponHolder = weaponHolder;
            weaponHolder.SetMeshState(false);
            weaponHolder.SetBoxCollider(false);
            weaponHolder.SetRigidbodyKinematic(true);
            weaponHolder.transform.SetParent(playerCombat.powerGlovesPos);
            weaponHolder.SetPosition(playerCombat.powerGlovesPos.position);
            weaponHolder.SetRotation(Vector3.zero);

            if(weaponHolder.weaponbObj.name == "Gloves")
            {
                for(int i = 0; i < sword.Length; i++)
                {
                    sword[i].SetActive(false);
                }

                for (int i = 0; i < gloves.Length; i++)
                {
                    gloves[i].SetActive(true);
                }
            }

            if (weaponHolder.weaponbObj.name == "LangesMesser")
            {
                for (int i = 0; i < gloves.Length; i++)
                {
                    gloves[i].SetActive(false);

                }

                for (int j = 0; j < sword.Length; j++)
                {
                    sword[j].SetActive(true);
                }
            }
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
            itemHolder.transform.SetParent(playerCombat.powerGlovesPos);
            itemHolder.SetRotation(Vector3.zero);

            AkSoundEngine.PostEvent("Play_Equip_Fist", gameObject);
            Debug.Log("Succesfully equipped an item.");
        }
    }

    private void DropWeapon()
    {
        if (playerCombat.WeaponHolder)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Succesfully dropped a weapon");
                playerCombat.WeaponHolder.SetBoxCollider(true);
                playerCombat.WeaponHolder.SetRigidbodyKinematic(false);
                playerCombat.WeaponHolder.SetMeshState(true);
                playerCombat.PlayerController.maxSpeed = playerCombat.oldMaxSpeed;

                AkSoundEngine.PostEvent("Play_Drop_Item", gameObject);

                playerCombat.WeaponHolder.transform.SetParent(null);
                playerCombat.WeaponHolder = null;
                isEquipped = false;

                    for (int i = 0; i < sword.Length; i++)
                    {
                        sword[i].SetActive(false);
                    }

                    for (int i = 0; i < gloves.Length; i++)
                    {
                        gloves[i].SetActive(false);
                    }
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