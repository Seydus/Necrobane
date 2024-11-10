using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerProfile PlayerProfile { get; set; }
    public PlayerController PlayerController { get; private set; }
    public PlayerInteract PlayerInteract { get; private set; }

    private void Awake()
    {
        PlayerProfile = GetComponent<PlayerProfile>();
        PlayerController = GetComponent<PlayerController>();
        PlayerInteract = GetComponent<PlayerInteract>();
    }

    private void Update()
    {
        if(GameManager.Instance.GameState)
        {
            PlayerProfile.Init();
            PlayerController.Init();
            PlayerInteract.Init();
        }
    }
}
