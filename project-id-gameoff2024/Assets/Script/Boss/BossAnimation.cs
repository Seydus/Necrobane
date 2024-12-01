using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;

    public float GetCurrentAnimationLength()
    {
        return bossAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void TriggerProjectileShootAnimation(bool state)
    {
        bossAnimator.SetBool("isFire", state);
    }
    
    public void TriggerSummonAnimation(bool state)
    {
        bossAnimator.SetBool("isSummon", state);
    }

    public void TriggerBulletHellAnimation(bool state)
    {
        bossAnimator.SetBool("isBulletHell", state);
    }

    public void TriggerSpikeAttackAnimation(bool state)
    {
        bossAnimator.SetBool("isSpike", state);
    }

    public void TriggerSpikeAnimation(Animator spikeAnimator)
    {
        spikeAnimator.SetTrigger("isAttackSpike");
    }
}
