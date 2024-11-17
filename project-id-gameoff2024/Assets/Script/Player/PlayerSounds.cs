using UnityEngine;

public class PlayerSounds : MonoBehaviour       
{
    public void PlayPunch()
    {
        Debug.Log("Punch");
        AkSoundEngine.PostEvent("Play_FistSwing", gameObject);
    }

    public void PlayJump()
    {
        Debug.Log("Jump");
        //AkSoundEngine.PostEvent("Play_Jump", gameObject);
    }

}
