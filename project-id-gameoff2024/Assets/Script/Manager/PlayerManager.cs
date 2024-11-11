using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerProfile PlayerProfile { get; set; }
    public PlayerController PlayerController { get; private set; }
    public PlayerInteract PlayerInteract { get; private set; }

    public GameObject cameraHolder;
    public DroneController droneController;

    public Transform dronePosition;

    public bool enableDrone { get; set; }

    private void Awake()
    {
        PlayerProfile = GetComponent<PlayerProfile>();
        PlayerController = GetComponent<PlayerController>();
        PlayerInteract = GetComponent<PlayerInteract>();
    }

    private void Start()
    {
        droneController.transform.position = dronePosition.position;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            enableDrone = true;
        }

        if(enableDrone)
        {
            droneController.gameObject.SetActive(true);
            cameraHolder.SetActive(false);
        }
        else
        {
            droneController.transform.position = dronePosition.position;
            cameraHolder.SetActive(true);
        }

        if(GameManager.Instance.GameState && !enableDrone)
        {
            PlayerProfile.Init();
            PlayerController.Init();
            PlayerInteract.Init();
        }
    }
}
