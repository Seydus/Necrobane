using UnityEngine.Events;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static UnityAction OnPunchTriggered;
    public static UnityAction OnJumpTriggered;

    private void OnEnable()
    {
        OnPunchTriggered += PlayPunch;
        OnJumpTriggered += PlayJump;
    }

    private void OnDisable()
    {
        OnPunchTriggered -= PlayPunch;
        OnJumpTriggered -= PlayJump;
    }

    private void PlayPunch()
    {
        Debug.Log("Punch");
        AkSoundEngine.PostEvent("Play_FistSwing", gameObject);
    }

    private void PlayJump()
    {
        Debug.Log("Jump");
        AkSoundEngine.PostEvent("Play_Jump", gameObject);
    }
}
