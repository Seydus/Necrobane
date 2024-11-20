using UnityEngine;

public class CultistAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void CultistWalking(float value)
    {
        anim.SetFloat("isWalking", value);
    }

    public void CultistAttack(bool state)
    {
        anim.SetBool("isAttack", state);
    }

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }
}
