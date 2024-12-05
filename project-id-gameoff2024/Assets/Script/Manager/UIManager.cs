using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerStaminaTxt;
    public GameObject playerDropTxt;
    public GameObject playerCrosshair;
    public GameObject playerCrosshairLine;
    public GameObject uIPlayerProfileObj;

    [Header("Drone UI")]
    public TextMeshProUGUI droneBatteryTxt;
    public TextMeshProUGUI droneMarkItemTxt;

    [Header("Game UI")]
    public GameObject gameOverUI;

    [Header("Boss UI")]
    public GameObject bossUIObj;
    public Slider bossHealthSlider;
}
