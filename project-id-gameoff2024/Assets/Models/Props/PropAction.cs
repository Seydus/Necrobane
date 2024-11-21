using UnityEngine;
using UnityEngine.Events;

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
                anim.SetBool(BoolName, true);
            }
            else
            {
                anim.SetBool(BoolName, false);
            }
        }

    }
    private void PlayGateSFX()
    {
        Debug.Log("Gate");
        AkSoundEngine.PostEvent("Play_Open_Gate", gameObject);
    }


}
