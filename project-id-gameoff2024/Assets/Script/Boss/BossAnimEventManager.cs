using UnityEngine;

public class BossAnimEventManager : MonoBehaviour
{
    public void TriggerPerformAttackEvent()
    {
        BossController.OnTriggerPerformAttackEvent?.Invoke();
    }
    
    public void TriggerPerformSummonEvent()
    {
        BossController.OnTriggerPerformSummonEvent?.Invoke();
    }
    
    public void TriggerPerformBulletHellAttackEvent()
    {
        BossController.OnTriggerPerformBulletHellAttackEvent?.Invoke();
    }

    public void TriggerPerformSpikeAttackEvent()
    {
        BossController.OnPerformSpikeAttackTriggered?.Invoke();
    }

    public void TriggerPerformSpikeEvent()
    {
        BossController.OnTriggerSpikeEvent?.Invoke();
    }

    public void TriggerFinishProjectileAttackEvent()
    {
        BossController.OnFinishProjectileAttackTriggered?.Invoke();
    }
    
    public void TriggerFinishBulletHellAttackEvent()
    {
        BossController.OnFinishBulletHellAttackTriggered?.Invoke();
    }
    
    public void TriggerFinishSpikeAttackEvent()
    {
        BossController.OnFinishSpikeAttackTriggered?.Invoke();
    }
    
    public void TriggerFinishSummonAttackEvent()
    {
        BossController.OnFinishSummonAttackTriggered?.Invoke();
    }
}
