using UnityEngine;

public class SkeletonAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    public void SkeletonWalking(float value)
    {
        anim.SetFloat("isWalking", value);
    }

    public void SkeletonAttack(bool state)
    {
        anim.SetBool("isAttack", state);
    }

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }

}
