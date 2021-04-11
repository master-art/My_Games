using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Inventory : MonoBehaviourPunCallbacks
{
    //private bool inverntoryEnabled;
    public GameObject inventoryObj;

    PhotonView PV;
    public Slot[] slots;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

        void Start()
    {
        if (PV.IsMine)
        {
            foreach (Slot i in slots)
            {
                i.CustomStart();
            }

            Debug.Log("Inventory Start in not PV.isMine");
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            Debug.Log("Inventory Update in not PV.isMine");
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryObj.SetActive(!inventoryObj.activeInHierarchy);
        }

        foreach (Slot i in slots)
        {
            i.CheckForItem();
        }
    }
        
    [PunRPC]
    public void AddItem(Item itemToAdded, Item startingItem = null)
    {
       
        if (!PV.IsMine)
        {
            Debug.Log("AddItem in not PV.isMine");
                return;
        }
            

        int amountInStack = itemToAdded.amountInStack;
        List<Item> stackableItems = new List<Item>();
        List<Slot> emptySlots = new List<Slot>();

        if (startingItem && startingItem.itemID == itemToAdded.itemID && startingItem.amountInStack < startingItem.maxStackSize)
            stackableItems.Add(startingItem);

        foreach (Slot i in slots)
        {
            if (i.slotItem)
            {
                Item objValue = i.slotItem;
                if (objValue.itemID == itemToAdded.itemID && objValue.amountInStack < objValue.maxStackSize && objValue != startingItem)
                    stackableItems.Add(objValue);
            }else
            {
                emptySlots.Add(i);
            }
        }

        foreach(Item i in stackableItems)
        {
            int amountThatCanBeAdded = i.maxStackSize - i.amountInStack;
            if (amountInStack <= amountThatCanBeAdded)
            {
                i.amountInStack += amountInStack;
                Destroy(itemToAdded.gameObject);
                return;
            }
            else
            {
                i.amountInStack = i.maxStackSize;
                amountInStack -= amountThatCanBeAdded;
            }
        }

        itemToAdded.amountInStack = amountInStack;
        if(emptySlots.Count > 0)
        {
            itemToAdded.transform.parent = emptySlots[0].transform;
            itemToAdded.gameObject.SetActive(false);
        }

       /* foreach (Slot i in slots)
        {
            if (!i.slotItem)
            {
                itemToAdded.transform.parent = i.transform;
                return;
            }
        }*/
    }  

    private void OnTriggerEnter(Collider col)
    {
        if(!photonView.IsMine)
        {
            Debug.Log("not mine PV inside trigger");
            return;
        }
            if (col.GetComponent<Item>())
            {

                photonView.RPC("AddItem", RpcTarget.All, col.GetComponent<Item>());
            }
    }

 
}
