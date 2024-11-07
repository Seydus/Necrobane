using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 8f;
    public float moveSpeed { get; set; }
    [SerializeField] private float accelerationSpeed = 7f;
    [SerializeField] private float decelerationSpeed = 9f;
    private float setAcceleration;
    private float setDeceleration;

    [Header("Jump Settings")]
    [SerializeField] private float airDrag = 1f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -29.43f;

    private Vector3 savedDirection = Vector3.zero;

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

        setAcceleration = accelerationSpeed;
        setDeceleration = decelerationSpeed;
    }

    private void Update()
    {
        HandleMovement();
        HandleView();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetCameraRelativeMovement();

        if(moveDirection.x != 0 || moveDirection.z != 0)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, maxSpeed, setAcceleration * Time.deltaTime);
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
                moveSpeed = Mathf.Lerp(moveSpeed, 0, setDeceleration * Time.deltaTime);
            }
        }

        Vector3 movement = savedDirection * moveSpeed;

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
                Debug.Log("Jump!");
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            setDeceleration = decelerationSpeed;
        }
        else
        {
            setDeceleration = airDrag;
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
