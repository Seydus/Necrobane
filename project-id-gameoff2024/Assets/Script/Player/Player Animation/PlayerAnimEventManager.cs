using UnityEngine;

public class PlayerAnimEventManager : MonoBehaviour
{
    public void TriggerPunchEvent()
    {
        PlayerSounds.OnPunchTriggered?.Invoke();
    }

    public void TriggerJumpEvent()
    {
        PlayerSounds.OnJumpTriggered?.Invoke();
    }

    public void TriggerPerformFirstAttackEvent()
    {
        PlayerCombat.OnPerformFirstAttackTriggered?.Invoke();
    }

    public void TriggerPerformSecondaryAttackEvent()
    {
        PlayerCombat.OnPerformSecondaryAttackTriggered?.Invoke();
    }

    // Input the name of the method on the event animation at the end
    public void TriggerFinishAttackEvent()
    {
        PlayerCombat.OnFinishAttackTriggered?.Invoke();
    }
}