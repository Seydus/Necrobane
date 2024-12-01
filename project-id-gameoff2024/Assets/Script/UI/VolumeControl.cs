using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider thisSlider;
    public float MasterVolume;
    public float BGMVolume;
    public float SFXVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = thisSlider.value;

        if (whatValue == "Master")
        {
            MasterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("Master", MasterVolume);
        }

        if (whatValue == "SFX") 
        {
            SFXVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("SFX", SFXVolume);
        }

        if (whatValue == "BGM") 
        {
            BGMVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("BGM", BGMVolume);
        }

    }
}


