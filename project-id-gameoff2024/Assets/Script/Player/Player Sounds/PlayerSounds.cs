using UnityEngine.Events;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static UnityAction OnPunchTriggered;
    public static UnityAction OnJumpTriggered;
    public static UnityAction TriggerPerformFirstAttackEvent;
    private void OnEnable()
    {
        OnPunchTriggered += PlayPunch;
        OnJumpTriggered += PlayJump;
        TriggerPerformFirstAttackEvent += PlaySlash;
    }

    private void OnDisable()
    {
        OnPunchTriggered -= PlayPunch;
        OnJumpTriggered -= PlayJump;
        TriggerPerformFirstAttackEvent -= PlaySlash;
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

    private void PlaySlash()
    {
        Debug.Log("Slash");
    }


}
