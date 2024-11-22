using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class InventoryHolder : MonoBehaviour
{
    public List<Item> Items;

    public List<TextMeshProUGUI> ItemCount;

    public GameObject InventoryObject;
    private void Awake()
    {
        StartCoroutine(UpdateInv());
    }
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
            for(int i = 0; i < Items.Count; i++)
            {
                if(Items[i].Objname == other.gameObject.GetComponent<Item>().Objname)
                {
                    Items[i].amount += other.gameObject.GetComponent<Item>().amount;
                    Destroy(other.gameObject);
                    break;
                }
            }
        }
    }

    IEnumerator UpdateInv()
    {
        yield return new WaitForSeconds(0.1f);
        for(int i = 0;i < ItemCount.Count; i++)
        {
            if (Items[i] != null)
            {
                ItemCount[i].text = Items[i].amount.ToString();
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateInv());
    }
}
