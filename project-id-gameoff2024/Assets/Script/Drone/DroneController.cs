using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using System.Security.Cryptography;
using UnityEditor.PackageManager;

public class DroneController : MonoBehaviour
{
    private DroneProfile droneProfile;

    [Header("Drone Settings")]
    [SerializeField] private float droneSpeed = 3.81f;
    [SerializeField] private float liftValue = 5f;
    private bool backToPlayer;

    private Vector3 direction;
    private Vector3 previousDirection;
    private Vector3 resetPosition;
    private float resetHeight;

    [Header("Interact")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float sphereRadius = 1;
    [SerializeField] private float interactDistance = 2;
    private bool markedItem;
    private Ray sphereRay;
    private RaycastHit hitInfo;

    [Header("Drone Stabilization & Physics")]
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

    [Header("Others")]
    [SerializeField] private Camera cam;

    [Header("Debugging")]
    private bool isHit;

    private void Awake()
    {
        droneProfile = GetComponent<DroneProfile>();
        myBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        targetHeight = transform.position.y;
        currentRotation = transform.localRotation.eulerAngles;
        resetHeight = targetHeight;
        resetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Init();
    }

    private void Init()
    {
        if (droneProfile.droneTimer > 0 && !backToPlayer)
        {
            PIDController();
            Movement();
            HandleView();
            HandleInteract();

            droneProfile.DeductTimer(droneProfile.batteryCost);
            GameManager.Instance.uIManager.droneBatteryTxt.SetText("{0:1}", droneProfile.droneTimer);
        }
        else
        {
            StartCoroutine(BackToPlayer());
        }
    }

    private IEnumerator BackToPlayer()
    {
        backToPlayer = true;
        droneProfile.ResetTimer();
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.uIManager.droneBatteryTxt.SetText("{0:1}", droneProfile.droneTimer);
        yield return new WaitForSeconds(0.001f);
        GameManager.Instance.playerManager.enableDrone = false;
        backToPlayer = false;
        yield return new WaitForSeconds(0.001f);
        GameManager.Instance.uIManager.droneMarkItemTxt.gameObject.SetActive(false);
        gameObject.SetActive(false);
        targetHeight = resetHeight;
        transform.position = resetPosition;
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

    private Ray HandleCameraDirection()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }


    private void DroneState()
    {
        if(Input.GetKeyDown(KeyCode.T) && !backToPlayer)
        {
            StartCoroutine(BackToPlayer());
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
    private void HandleInteract()
    {
        sphereRay = HandleCameraDirection();

        // Interact an item
        if (Physics.SphereCast(sphereRay, sphereRadius, out hitInfo, interactDistance, interactLayer))
        {
            Debug.Log("Interacting an item...");
            GameManager.Instance.uIManager.droneMarkItemTxt.gameObject.SetActive(true);
            isHit = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if(hitInfo.transform.TryGetComponent<ItemHolder>(out ItemHolder itemHolder))
                {
                    itemHolder.HandleSelected(markedItem = !markedItem);
                    Debug.Log("Succesfully marked an item.");
                }
            }
        }
        else
        {
            GameManager.Instance.uIManager.droneMarkItemTxt.gameObject.SetActive(false);
            isHit = false;
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
