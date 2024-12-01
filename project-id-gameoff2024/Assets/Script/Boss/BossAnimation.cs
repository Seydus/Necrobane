using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;

    public float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Idle"))
        {
            return 0f;
        }

        return stateInfo.length;
    }

    public float GetCurrentFireAnimationLength()
    {
        AnimatorStateInfo stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armature|fire"))
        {
            return stateInfo.length;
        }

        return 0f;
    }

    public float GetCurrentBulletHellAnimationLength()
    {
        AnimatorStateInfo stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armature|BulletHell"))
        {
            return stateInfo.length;
        }

        return 0f;
    }

    public float GetCurrentSummonAnimationLength()
    {
        AnimatorStateInfo stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armature|summon"))
        {
            return stateInfo.length;
        }

        return 0f;
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
