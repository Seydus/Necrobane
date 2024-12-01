using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;

    public float GetCurrentAnimationLength()
    {
        return bossAnimator.GetCurrentAnimatorStateInfo(0).length;
    }


    public void TriggerProjectileShootAnimation()
    {
        bossAnimator.SetTrigger("isFire");
    }
    
    public void TriggerSummonAnimation()
    {
        bossAnimator.SetTrigger("isSummon");
    }

    public void TriggerBulletHellAnimation()
    {
        bossAnimator.SetTrigger("isBulletHell");
    }

    public void TriggerSpikeAttackAnimation()
    {
        bossAnimator.SetTrigger("isSpike");
    }

    public void TriggerSpikeAnimation(Animator spikeAnimator)
    {
        spikeAnimator.SetTrigger("isAttackSpike");
    }
}
