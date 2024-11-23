using UnityEngine;
using UnityEngine.UI;

public class StraightRayInteractions : MonoBehaviour
{
    public int distF;

    public RaycastHit hit;
    public Transform cam;

    public KeyHold kh;

    private InventoryHolder invHold;

    private void Start()
    {
        kh = GetComponent<KeyHold>();
        invHold = GetComponent<InventoryHolder>();
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
                    invHold.MoreItem(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                }

                if(hit.collider.tag == "Door")
                {
                    if (Hi.GetComponent<PropAction>().NKey)
                    {
                        for (int i = 0; i < kh.Keys.Count; i++)
                        {
                            if (kh.Keys[i] == Hi.GetComponent<PropAction>().NeededKey)
                            {
                                for(int j = 0; j < invHold.Items.Count; j++)
                                {
                                    if (invHold.Items[j].Objname == kh.Keys[i])
                                    {
                                        invHold.Items[j] = null;
                                        invHold.InventorySlots[j].avatar = null;
                                        invHold.InventorySlots[j].Objname = null;
                                        Hi.GetComponent<PropAction>().NKey = false;
                                    }
                                }
                            }
                        }
                    }

                    if (!Hi.GetComponent<PropAction>().NKey)
                    {
                        bool open = true;
                        if(!Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName)) Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, true); open = false;
                        if(Hi.GetComponent<PropAction>().anim.GetBool(Hi.GetComponent<PropAction>().BoolName)) Hi.GetComponent<PropAction>().anim.SetBool(Hi.GetComponent<PropAction>().BoolName, false); open = false;
                    }

                }
            }
        }
        distF = (int)hit.distance;
    }
}
