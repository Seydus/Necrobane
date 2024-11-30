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

    public void TriggerFinishAttackEvent()
    {
        BossController.OnFinishAttackTriggered?.Invoke();
    }
}
