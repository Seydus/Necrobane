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
            Debug.LogError("Banan");
            if (right && punch)
            {
                animator.SetBool("RightPunch", true);
                Invoke("OffPunch", 0.3f);
                right = false;
                punch = false;
            }

            if (!right && punch)
            {
                animator.SetBool("LeftPunch", true);
                Invoke("OffPunch", 0.3f);
                right = true;
                punch = false;
            }

            punch = true;
        }
    }

    public void OffPunch()
    {
        animator.SetBool("LeftPunch", false);
        animator.SetBool("RightPunch", false);
    }
}
