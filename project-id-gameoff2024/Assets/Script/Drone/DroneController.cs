using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Drone Settings")]
    [SerializeField] private float droneSpeed = 3.81f;
    [SerializeField] private float liftValue = 5f;
    private bool droneMoving = false;
    private bool droneEnable = false;

    private Vector3 direction;
    private Vector3 previousDirection;

    [Header("Drone Physics")]
    [SerializeField] private float forceValue = 9.81f;
    [SerializeField] private float proportionalGain = 5f;
    [SerializeField] private float integralGain = 0.5f;
    [SerializeField] private float derivativeGain = 4f;
    [SerializeField] private float integralMax = 10f;

    private float integralError = 0f;
    private float previousError = 0f;

    private Rigidbody myBody;
    private float targetHeight;

    [Header("Camera Settings")]
    [SerializeField] private float viewSmooth;
    [SerializeField] private float viewXSensitivity = 300f;
    [SerializeField] private float viewYSensitivity = 150f;
    [SerializeField] private bool viewXInverted = false;
    [SerializeField] private bool viewYInverted = false;

    private Vector3 currentRotation;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        targetHeight = transform.position.y;
        currentRotation = transform.localRotation.eulerAngles;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if(droneEnable)
        {
            PIDController();
            Movement();
            HandleView();
        }
    }

    private void PIDController()
    {
        // SRC: https://en.wikipedia.org/wiki/Proportional%E2%80%93integral%E2%80%93derivative_controller

        float currentHeight = transform.position.y;
        float error = targetHeight - currentHeight;

        float proportional = proportionalGain * error;

        integralError += error * Time.fixedDeltaTime;
        integralError = Mathf.Clamp(integralError, -integralMax, integralMax);
        float integral = integralGain * integralError;

        float derivative = (error - previousError) / Time.fixedDeltaTime;
        float derivativeTerm = derivativeGain * derivative;

        float correctiveForce = proportional + integral + derivativeTerm;

        myBody.AddForce(Vector3.up * (forceValue + correctiveForce), ForceMode.Acceleration);

        previousError = error;
    }

    private void Update()
    {
        DroneState();
    }

    private void DroneState()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            droneEnable = !droneEnable;
        }
    }

    private void Movement()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.Space))
        {
            targetHeight += liftValue * Time.fixedDeltaTime;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            targetHeight -= liftValue * Time.fixedDeltaTime;
        }

        if(direction.magnitude >= 0.1f)
        {
            myBody.AddRelativeForce(direction * droneSpeed, ForceMode.Acceleration);
        }
    }

    private void HandleView()
    {
        float mouseX = Input.GetAxis("Mouse X") * viewXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * viewYSensitivity * Time.deltaTime;

        if (viewXInverted) mouseX = -mouseX;
        if (viewYInverted) mouseY = -mouseY;

        currentRotation.y += mouseX;
        currentRotation.x -= mouseY;
        currentRotation.z = 0f;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -90f, 90f);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(currentRotation), viewSmooth * Time.deltaTime);
    }
}
