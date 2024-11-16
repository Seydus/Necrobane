using UnityEngine;

public class PlayerSounds : MonoBehaviour       
{
    [SerializeField]
    private AK.Wwise.Event play_punch;
    public void PlayPunchSound()
    {
        play_punch.Post(gameObject);
    }
    public void PlayPunch()
    {
        Debug.Log("Punch");
    }

}
