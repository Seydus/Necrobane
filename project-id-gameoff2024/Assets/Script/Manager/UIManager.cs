using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerStaminaTxt;
    public GameObject playerDropTxt;
    public GameObject playerCrosshair;
    public GameObject playerCrosshairLine;

    [Header("Drone UI")]
    public TextMeshProUGUI droneBatteryTxt;
    public TextMeshProUGUI droneMarkItemTxt;

    [Header("Game UI")]
    public GameObject gameOverUI;
}
