using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 velocity;

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

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        cameraRotation = cameraHolder.localRotation.eulerAngles;
        characterRotation = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        HandleMovement();
        HandleView();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetCameraRelativeMovement();
        Vector3 movement = moveDirection * moveSpeed;

        ApplyGravityAndJump();

        Vector3 finalMovement = movement + velocity;
        characterController.Move(finalMovement * Time.deltaTime);
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
                velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
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
}
