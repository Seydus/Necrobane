using UnityEngine;

public class LeverActives : MonoBehaviour
{
    private Animator anim;
    public bool active;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("On"))
        {
            active = true;
        }
        else if (!anim.GetBool("On"))
        {
            active = false;
        }
    }
}
