using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }

    public void PeformBasicAttackAnim(bool isRightPunchNext)
    {
        if (isRightPunchNext)
        {
            anim.SetTrigger("RightPunch");
        }
        else
        {
            anim.SetTrigger("LeftPunch");
        }
    }

    public void PerformSuperAttackAnim()
    {
        anim.SetTrigger("SuperPunch");
    }
}
