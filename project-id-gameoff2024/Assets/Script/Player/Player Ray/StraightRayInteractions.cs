using UnityEngine;
using UnityEngine.UI;

public class StraightRayInteractions : MonoBehaviour
{
    public int distF;

    public RaycastHit hit;
    public Transform cam;

    public KeyHold kh;

    private void Start()
    {
        kh = GetComponent<KeyHold>();
    }

    void Update()
    {
        //create ray
        Ray ray = new Ray();
        ray.origin = cam.position;
        ray.direction = cam.forward;

        //Draw ray
        Debug.DrawRay(cam.position, cam.forward * 100, Color.red);

        //Rey colides

        if (Physics.Raycast(ray, out hit))
        {
            Selectable Selected = hit.collider.gameObject.GetComponent<Selectable>();
            if (Input.GetKeyDown(KeyCode.E) && distF <= 30)
            {
                GameObject Hi = hit.collider.gameObject;
                if(hit.collider.tag == "Lever")
                {
                    bool on = true;
                    if(hit.collider.GetComponent<Animator>().GetBool("On") == false && on)
                    {
                        hit.collider.GetComponent<Animator>().SetBool("On", true);
                        on = false;
                    }

                    
                    if (hit.collider.GetComponent<Animator>().GetBool("On") == true && on)
                    {
                        hit.collider.GetComponent<Animator>().SetBool("On", false);
                        on = false;
                    }
                }

                if (hit.collider.tag == "Key")
                {
                    kh.Keys.Add(hit.collider.GetComponent<KeyPurpose>().keyPupose);
                    AkSoundEngine.PostEvent("Play_Key_Pickup", gameObject);
                    Destroy(hit.collider.gameObject);
                }

                if(hit.collider.tag == "Door")
                {
                    if (!Hi.GetComponent<PropAction>().NKey)
                    {
                        bool open = true;
                        if(!Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName)) Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, true); open = false;
                        if(Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName)) Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, false); open = false;
                    }

                    if (Hi.GetComponent<PropAction>().NKey)
                    {
                        for (int i = 0; i < kh.Keys.Count; i++)
                        {
                            if (kh.Keys[i] == Hi.GetComponent<PropAction>().NeededKey)
                            {
                                bool open = true;
                                if (!Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName) && open) 
                                { 
                                    Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, true); 
                                    open = false; 
                                }

                                if (Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName) && open)
                                {
                                    Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, false);
                                    open = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        distF = (int)hit.distance;
    }
}
