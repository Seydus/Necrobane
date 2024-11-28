using UnityEngine;

public class SkeletonAnimEventManager : MonoBehaviour
{
    private Skeleton skeleton;

    private void Awake()
    {
        skeleton = GetComponentInParent<Skeleton>();
    }

    public void TriggerSwingSFX()
    {
        AkSoundEngine.PostEvent("Play_Swings2H", gameObject);
    }

    public void TriggerPerformAttackEvent()
    {
        skeleton?.OnPerformAttackTriggered?.Invoke();
    }

    public void TriggerFinishAttackEvent()
    {
        skeleton?.OnFinishAttackTriggered?.Invoke();
    }
}
