using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class InventoryHolder : MonoBehaviour
{
    public List<Item> Items;

    public List<Item> InventorySlots;

    public GameObject InventoryObject;

    public GameObject[] ItemPrefs;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool open = true;

            if(InventoryObject.active && open)
            {
                InventoryObject.SetActive(false);
                open = false;
            } 

            if(!InventoryObject.active && open)
            {
                InventoryObject.SetActive(true);
                open = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Item>() != null)
        {
            MoreItem(other.gameObject);
        }
    }

    public void MoreItem(GameObject others)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] == null)
            {
                for (int j = 0; j < ItemPrefs.Length; j++)
                {
                    if (others.tag == ItemPrefs[j].tag)
                    {
                        Items[i] = ItemPrefs[j].GetComponent<Item>();
                        Items[i].Objname = others.GetComponent<Item>().Objname;
                        Items[i].amount = others.GetComponent<Item>().amount;
                        Items[i].avatar = others.GetComponent<Item>().avatar;

                        InventorySlots[i].amount = Items[i].amount;
                        InventorySlots[i].avatar = Items[i].avatar;
                        InventorySlots[i].Objname = Items[i].Objname;

                        Destroy(others.gameObject);
                    }
                }
                break;
            }

            if (Items[i].Objname == others.gameObject.GetComponent<Item>().Objname)
            {
                Items[i].amount += others.gameObject.GetComponent<Item>().amount;
                InventorySlots[i].amount = Items[i].amount;
                Destroy(others.gameObject);
                break;
            }

        }
    }

}
