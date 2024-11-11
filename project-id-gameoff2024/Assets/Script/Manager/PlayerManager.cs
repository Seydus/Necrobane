using UnityEngine;
using System.Collections;

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
        if(Input.GetKeyDown(KeyCode.T) && !enableDrone)
        {
            StartCoroutine(BackToDrone());

        }

        if(!enableDrone)
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

    private IEnumerator BackToDrone()
    {
        enableDrone = true;
        yield return new WaitForSeconds(0.5f);
        droneController.gameObject.SetActive(true);
        cameraHolder.SetActive(false);
    }
}
