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

    public void TriggerPerformBasicPunchAttackEvent()
    {
        PlayerCombat.OnPerformBasicPunchAttackTriggered?.Invoke();
    }

    public void TriggerPerformSuperPunchAttackEvent()
    {
        PlayerCombat.OnPerformSuperPunchAttackTriggered?.Invoke();
    }

    // Input the name of the method on the event animation of the model
    public void TriggerPerformBasicSwordAttackEvent()
    {
        PlayerCombat.OnPerformBasicSwordAttackTriggered?.Invoke();
    }

    // Input the name of the method on the event animation of the model
    public void TriggerPerformDefendSwordhAttackEvent()
    {
        PlayerCombat.OnPerformDefendSwordTriggered?.Invoke();
    }

    // Input the name of the method on the event animation at the end
    public void TriggerFinishAttackEvent()
    {
        PlayerCombat.OnFinishAttackTriggered?.Invoke();
    }
}