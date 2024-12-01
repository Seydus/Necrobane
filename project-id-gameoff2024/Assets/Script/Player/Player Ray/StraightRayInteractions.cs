using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StraightRayInteractions : MonoBehaviour
{
    public int distF;

    public RaycastHit hit;
    public Transform cam;

    public KeyHold kh;

    private InventoryHolder invHold;

    public GameObject UpgradingPanel;

    private PlayerManager playerManager;

    private void Start()
    {
        kh = GetComponent<KeyHold>();
        invHold = GetComponent<InventoryHolder>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    void Update()
    {
        //create ray
        Ray ray = new Ray();
        ray.origin = cam.position;
        ray.direction = cam.forward;

        ////Draw ray
        //Debug.DrawRay(cam.position, cam.forward * 100, Color.red);

        //Rey colides

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UpgradingPanel.active)
            {
                Cursor.lockState = CursorLockMode.Locked;
                playerManager.enabled = true;
                UpgradingPanel.SetActive(false);
            }
        }

        if (Physics.Raycast(ray, out hit))
        {
            Selectable Selected = hit.collider.gameObject.GetComponent<Selectable>();
            if (Input.GetKeyDown(KeyCode.E) && distF <= 5)
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
                    invHold.MoreItem(hit.collider.gameObject);
                    AkSoundEngine.PostEvent("Play_Key_Pickup", gameObject);
                    invHold.MoreItem(hit.collider.gameObject);
                }

                if(hit.collider.tag == "Door")
                {
                    if (Hi.GetComponent<PropAction>().NKey)
                    {
                        for (int i = 0; i < kh.Keys.Count; i++)
                        {
                            if (kh.Keys[i] == hit.collider.GetComponent<PropAction>().NeededKey)
                            {
                                bool open = true;
                                if (hit.collider.GetComponent<PropAction>().anim.GetBool(hit.collider.GetComponent<PropAction>().BoolName) == false && open)
                                {
                                    hit.collider.GetComponent<PropAction>().anim.SetBool(hit.collider.GetComponent<PropAction>().BoolName, true);
                                    open = false;
                                }
                                if (hit.collider.GetComponent<PropAction>().anim.GetBool(hit.collider.GetComponent<PropAction>().BoolName) && open)
                                {
                                    hit.collider.GetComponent<PropAction>().anim.SetBool(hit.collider.GetComponent<PropAction>().BoolName, false);
                                    open = false;
                                }
                            }

                           // for (int j = 0; j < invHold.Items.Count; j++)
                           // {
                           ///     if (invHold.Items[j].Objname == kh.Keys[i])
                           ////    {
                           //             invHold.InventorySlots[j].avatar = null;
                           ///             invHold.InventorySlots[j].Objname = null;
                            //            invHold.Items[j] = null;
                            //    }
                           // }
                        }
                    }

                    if (!hit.collider.GetComponent<PropAction>().NKey)
                    {
                        bool open = true;
                        if(hit.collider.GetComponent<PropAction>().anim.GetBool(hit.collider.GetComponent<PropAction>().BoolName) == false && open) hit.collider.GetComponent<PropAction>().anim.SetBool(hit.collider.GetComponent<PropAction>().BoolName, true); open = false;
                        if(hit.collider.GetComponent<PropAction>().anim.GetBool(hit.collider.GetComponent<PropAction>().BoolName) && open) hit.collider.GetComponent<PropAction>().anim.SetBool(hit.collider.GetComponent<PropAction>().BoolName, false); open = false;
                    }

                }

                if(hit.collider.tag == "Anvil")
                {
                    bool open = true;

                    if (UpgradingPanel.active && open)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        playerManager.enabled = true;
                        UpgradingPanel.SetActive(false);
                        open = false;
                    }

                    if (!UpgradingPanel.active && open)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        playerManager.enabled = false;
                        UpgradingPanel.SetActive(true);
                        open = false;
                    }
                }
            }
        }
        distF = (int)hit.distance;
    }
}
