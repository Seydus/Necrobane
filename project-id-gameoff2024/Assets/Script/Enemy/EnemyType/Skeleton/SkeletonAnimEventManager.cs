using UnityEngine;

public class SkeletonAnimEventManager : MonoBehaviour
{
    public void TriggerSwingSFX()
    {
        AkSoundEngine.PostEvent("Play_Swings2H", gameObject);
    }

    public void TriggerPerformAttackEvent()
    {
        Skeleton.OnPerformAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        Skeleton.OnFinishAttackTriggered?.Invoke();
    }
}
