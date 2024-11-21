using UnityEngine;

public class PropAction : MonoBehaviour
{
    public LeverActives la;

    public Animator anim;
    public string BoolName;

    public bool NKey;
    public bool NLever;

    public string NeededKey;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (NLever)
        {
            if (la.active)
            {
                anim.SetBool(BoolName, false);
            }
            else
            {
                anim.SetBool(BoolName, true);
            }
        }

    }
}
