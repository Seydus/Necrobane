using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject mainObj;
    public GameObject settingsObj;
    public GameObject creditsObj;

    [Header("Volume Slider")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
}
