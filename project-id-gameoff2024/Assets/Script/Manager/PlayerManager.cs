using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public PlayerProfile PlayerProfile { get; set; }
    public PlayerController PlayerController { get; set; }
    public PlayerInteract PlayerInteract { get; set; }
    public PlayerCombat PlayerCombat { get; set; }

    public GameObject cameraHolder;
    public DroneController droneController;

    public Transform dronePosition;

    public bool enableDrone { get; set; }

    private void Awake()
    {
        PlayerProfile = GetComponent<PlayerProfile>();
        PlayerController = GetComponent<PlayerController>();
        PlayerInteract = GetComponent<PlayerInteract>();
        PlayerCombat = GetComponent<PlayerCombat>();
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

        if (!GameManager.Instance.GameState)
            return;

        PlayerController.Init();
        PlayerInteract.Init();
        PlayerCombat.Init();
    }

    private IEnumerator BackToDrone()
    {
        yield return new WaitForSeconds(0.1f);
        enableDrone = true;
        droneController.gameObject.SetActive(true);
        cameraHolder.SetActive(false);
        GameManager.Instance.uIManager.playerCrosshairLine.gameObject.SetActive(false);
        GameManager.Instance.uIManager.playerCrosshair.gameObject.SetActive(true);
    }
}
