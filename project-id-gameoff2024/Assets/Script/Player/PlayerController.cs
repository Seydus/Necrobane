using UnityEngine;
using AK.Wwise;
using UnityEditor.PackageManager;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 8f;
    public float moveSpeed { get; set; }
    [SerializeField] private float setAccelerationSpeed = 7f;
    [SerializeField] private float setDecelerationSpeed = 9f;
    private float accelerationSpeed;
    private float decelerationSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float airDrag = 1f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -29.43f;
    [SerializeField] private float ceilingPullPower = 2f;
    [SerializeField] private float ceilingHeight = 2f;
    [SerializeField] private float ceilingSphereRadius = 2f;
    private bool hitCeiling;

    private Vector3 savedDirection = Vector3.zero;
    private Vector3 velocity;

    private RaycastHit ceilingHit;
    private CharacterController characterController;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float viewClampYMin = -70f;
    [SerializeField] private float viewClampYMax = 80f;
    [SerializeField] private float viewXSensitivity = 300f;
    [SerializeField] private float viewYSensitivity = 150f;
    [SerializeField] private bool viewXInverted = false;
    [SerializeField] private bool viewYInverted = false;

    private Vector3 cameraRotation;
    private Vector3 characterRotation;


    // Wwise
    private bool FootstepIsPlaying = false;
    private bool LandingIsPlaying = false;
    private bool IsJumping = false;
    private float LastFootstepTime = 0;
    private int _speed;
    private RaycastHit hit;
    private string PhysMat;
    private string PhysMat_Last;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        cameraRotation = cameraHolder.localRotation.eulerAngles;
        characterRotation = transform.localRotation.eulerAngles;

        accelerationSpeed = setAccelerationSpeed;
        decelerationSpeed = setDecelerationSpeed;

    }

    public void Init()
    {
        HandleMovement();
        HandleView();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetCameraRelativeMovement();

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, maxSpeed, accelerationSpeed * Time.deltaTime);
            savedDirection = moveDirection;
        }
        else
        {
            if (moveSpeed < 0.05f)
            {
                moveSpeed = 0;
            }
            else
            {
                moveSpeed = Mathf.Lerp(moveSpeed, 0, decelerationSpeed * Time.deltaTime);
            }

        }

        Vector3 movement = savedDirection * moveSpeed;
        ApplyGravityAndJump();
        Vector3 finalMovement = movement + velocity;
        characterController.Move(finalMovement * Time.deltaTime);

        if (!FootstepIsPlaying && !IsJumping)
        {
            PlayerFootsteps();
            LastFootstepTime = Time.time;
            FootstepIsPlaying = true;
        }
        else
        {
            if (moveSpeed > 1)
            {
                if (Time.time - LastFootstepTime > 2.5 / moveSpeed)
                {
                    FootstepIsPlaying = false;
                }
            }
        }
    }

    private Vector3 GetCameraRelativeMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraHolder.forward;
        Vector3 right = cameraHolder.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 direction = forward.normalized * verticalInput + right.normalized * horizontalInput;

        return direction.normalized;
    }

    private void ApplyGravityAndJump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("Jump!");
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                IsJumping = true;
            }
            else
            {
                if (IsJumping)
                {
                    PlayerJump();
                }
                IsJumping = false;
            }

            decelerationSpeed = setDecelerationSpeed;
            hitCeiling = false;
        }
        else
        {
            decelerationSpeed = airDrag;

            if (Physics.SphereCast(transform.position, ceilingSphereRadius, Vector3.up, out ceilingHit, ceilingHeight))
            {
                hitCeiling = true;
                Debug.Log("Hit ceilling");
            }

            velocity.y += gravity * (Time.deltaTime * (hitCeiling ? ceilingPullPower : 1));
        }

    }

    private void HandleView()
    {
        float mouseX = Input.GetAxis("Mouse X") * viewXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * viewYSensitivity * Time.deltaTime;

        if (viewXInverted) mouseX = -mouseX;
        if (viewYInverted) mouseY = -mouseY;

        characterRotation.y += mouseX;
        transform.localRotation = Quaternion.Euler(characterRotation);

        cameraRotation.x -= mouseY;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, viewClampYMin, viewClampYMax);
        cameraHolder.localRotation = Quaternion.Euler(cameraRotation);
    }

    private void PlayerFootsteps()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            PhysMat_Last = PhysMat;

            PhysMat = hit.collider.tag;

            if (PhysMat != PhysMat_Last)
            {
                AkSoundEngine.SetSwitch("Material", PhysMat, gameObject);

                print(PhysMat);
            }
        }
        if (!characterController.isGrounded)
        {
            return;
        }

        AkSoundEngine.PostEvent("Play_Footsteps", gameObject);


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = hitCeiling ? Color.green : Color.red;

        if (hitCeiling)
        {
            Gizmos.DrawRay(transform.position, Vector3.up * ceilingHeight);
            Gizmos.DrawWireSphere(ceilingHit.point, ceilingSphereRadius);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector3.up * ceilingHeight);
        }
    }

    private void PlayerJump()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            PhysMat_Last = PhysMat;

            PhysMat = hit.collider.tag;

            if (PhysMat != PhysMat_Last)
            {
                AkSoundEngine.SetSwitch("Material", PhysMat, gameObject);

                print(PhysMat);
            }
        }
        if (!characterController.isGrounded)
        {
            return;
        }

        AkSoundEngine.PostEvent("Play_Jump", gameObject);
    }
}