using UnityEngine;

public class CulltistAnimEventManager : MonoBehaviour
{
    private Cultist cultist;

    private void Awake()
    {
        cultist = GetComponentInParent<Cultist>();
    }

    public void TriggerPerformAttackEvent()
    {
        cultist?.OnPerformAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        cultist?.OnFinishAttackTriggered?.Invoke();
    }

    public void TriggerPerformSummonEvent()
    {
        cultist?.OnPerformSummonTriggered?.Invoke();
    }
}
