using UnityEngine;

public class BossAnimEventManager : MonoBehaviour
{
    public void TriggerPerformSpikeAttackEvent()
    {
        BossCombat.OnPerformSpikeAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        BossCombat.OnFinishAttackTriggered?.Invoke();
    }
}
