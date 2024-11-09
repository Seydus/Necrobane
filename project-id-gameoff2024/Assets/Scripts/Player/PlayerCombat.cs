using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCombat : MonoBehaviour
{
    private enum MeleeType
    {
        Hand,
        Sword
    };

    [SerializeField] private MeleeType meleeType = MeleeType.Hand;

    [Header("FocusTarget")]
    [SerializeField] private float focusTargetSpeed;
    [SerializeField] private float combatTargetMaxDistance;
    [SerializeField] private LayerMask combatTargetLayer;
    private Transform currentTarget;
    private Vector3 targetPosition;
    private bool focusTarget;

    [Header("Melee")]
    [SerializeField] private float sphereRadius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask combatLayer;
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

    // Still not finish
    private void HandleSelectTarget()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            focusTarget = !focusTarget;

            if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, combatTargetMaxDistance, combatTargetLayer))
            {
                currentTarget = hitInfo.transform;
                targetPosition = hitInfo.transform.position;
            }
        }

        playerController.isTargeting = focusTarget ? true : false;

        HandleFocusTarget(focusTarget);
    }

    private void HandleFocusTarget(bool state)
    {
        if(focusTarget)
        {
            if (!currentTarget)
            {
                Debug.LogWarning("Target not found!");
                focusTarget = false;
                return;
            }

            Vector3 direction = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, focusTargetSpeed * Time.deltaTime);

            Debug.Log("Looking at the target");
        }
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

                if(hitInfo.transform.TryGetComponent<EnemyRoaming>(out EnemyRoaming enemy))
                {
                    HandleMeleeType(meleeType, enemy);
                }
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

    private void HandleMeleeType(MeleeType type, EnemyRoaming enemy)
    {
        switch (type)
        {
            case MeleeType.Hand:
                HandleHandCombat(enemy);
                break;
            case MeleeType.Sword:
                HandleSwordCombat(enemy);
                break;
        }
    }

    private void HandleHandCombat(EnemyRoaming enemy)
    {
        Debug.Log("Hand Combat Triggered");
        enemy.DamageHealth(playerController.damage);
    }

    private void HandleSwordCombat(EnemyRoaming enemy)
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
