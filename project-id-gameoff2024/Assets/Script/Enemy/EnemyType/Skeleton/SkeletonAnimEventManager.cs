using UnityEngine;

public class SkeletonAnimEventManager : MonoBehaviour
{
    public void TriggerPerformAttackEvent()
    {
        Skeleton.OnPerformAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        Skeleton.OnFinishAttackTriggered?.Invoke();
    }
}
