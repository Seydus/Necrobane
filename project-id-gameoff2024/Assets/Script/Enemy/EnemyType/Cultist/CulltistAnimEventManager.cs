using UnityEngine;

public class CulltistAnimEventManager : MonoBehaviour
{
    public void TriggerPerformAttackEvent()
    {
        Cultist.OnPerformAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        Cultist.OnFinishAttackTriggered?.Invoke();
    }
}
