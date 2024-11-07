using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float sphereRadius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask combatLayer;

    private enum MeleeType
    {
        Hand,
        Sword
    };

    [SerializeField] private MeleeType meleeType = MeleeType.Hand;

    private Ray sphereRay;
    private RaycastHit hitInfo;

    [SerializeField] private Camera cam;
    private PlayerController playerController;

    [Header("Debugging")]
    private bool isHit;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleMelee();
    }

    private Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }

    private void HandleMelee()
    {
        sphereRay = HandleCameraDirection();

        if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, maxDistance, combatLayer))
        {
            isHit = true;

            if(Input.GetMouseButtonDown(0))
            {
                PlayerControllerEvent();
                HandleMeleeType(meleeType);
            }
        }
        else
        {
            isHit = false;
        }
    }

    private void PlayerControllerEvent()
    {
        
    }

    private void HandleMeleeType(MeleeType type)
    {
        switch (type)
        {
            case MeleeType.Hand:
                HandleHandCombat();
                break;
            case MeleeType.Sword:
                HandleSwordCombat();
                break;
        }
    }

    private void HandleHandCombat()
    {
        Debug.Log("Hand Combat Triggered");
    }

    private void HandleSwordCombat()
    {
        Debug.Log("Sword Combat Triggered");
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
            Gizmos.DrawRay(sphereRay.origin, sphereRay.direction.normalized * maxDistance);
        }
    }
}
