using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerStaminaTxt;
    public TextMeshProUGUI playerGrabTxt;
    public TextMeshProUGUI playerDropTxt;
    public TextMeshProUGUI playerAttackTxt;

    [Header("Drone UI")]
    public TextMeshProUGUI droneBatteryTxt;

    [Header("Game UI")]
    public GameObject gameOverUI;
}
