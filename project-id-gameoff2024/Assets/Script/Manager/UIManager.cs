using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerStaminaTxt;
    public GameObject playerGrabTxt;
    public GameObject playerDropTxt;
    public GameObject playerAttackTxt;

    [Header("Drone UI")]
    public TextMeshProUGUI droneBatteryTxt;
    public TextMeshProUGUI droneMarkItemTxt;

    [Header("Game UI")]
    public GameObject gameOverUI;
}
