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

    public void TriggerInitBasicAttackEvent()
    {
        PlayerCombat.OnInitBasicAttackTriggered?.Invoke();
    }

    public void TriggerInitSuperAttackEvent()
    {
        PlayerCombat.OnInitSuperAttackTriggered?.Invoke();
    }

    public void TriggerPerformBasicAttackEvent()
    {
        PlayerCombat.OnPerformBasicAttackTriggered?.Invoke();
    }

    public void TriggerPerformSuperAttackEvent()
    {
        PlayerCombat.OnPerformSuperAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        PlayerCombat.OnFinishAttackTriggered?.Invoke();
    }
}