using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }

    public void PeformBasicPunchAttackAnim()
    {
        anim.SetTrigger("LeftPunch");
    }

    public void PerformSuperPunchAttackAnim()
    {
        anim.SetTrigger("SuperPunch");
    }

    public void PeformBasicSwordAttackAnim()
    {
        anim.SetTrigger("LeftSword");
    }

    public void PerformDefendSwordAttackAnim()
    {
        anim.SetBool("isBlockSword", true);
    }

    public void UnPerformDefendSwordAttackAnim()
    {
        anim.SetBool("isBlockSword", false);
    }
}
