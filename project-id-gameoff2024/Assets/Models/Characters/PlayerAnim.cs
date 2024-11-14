using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator animator;

    public bool right;
    public bool punch;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
                animator.SetBool("RightPunch", true);
                Invoke("OffPunch", 0.3f);
        }
    }

    public void OffPunch()
    {
        animator.SetBool("LeftPunch", false);
        animator.SetBool("RightPunch", false);
    }
}
