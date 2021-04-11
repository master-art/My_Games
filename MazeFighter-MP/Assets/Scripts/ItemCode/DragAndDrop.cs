using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;

public class DragAndDrop : MonoBehaviourPunCallbacks 
{
    public Inventory inv;

    GameObject curSlot;
    Item curSlotsItem;

    PlayerController checkPlayer;
    public Image followMouseImage;

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        followMouseImage.transform.position = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject obj = GetObjectUnderMouse();
            if (obj)
            {
                //obj.GetComponent<Slot>().photonView.RPC("DropItem", RpcTarget.All);
                obj.GetComponent<Slot>().DropItem();
            }
        }

        // Debug.Log("In Drop Item Check Health" + " " + checkPlayer.checkHealth);
        try
        {
            if (checkPlayer.checkHealth < 0)
            {
                // GetComponent<Slot>().photonView.RPC("DropItem", RpcTarget.All); 
                GetComponent<Slot>().DropItem();
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("checkHealth not set in the code" + ex);
        }
        

        if (Input.GetMouseButtonDown(0))
        {
            curSlot = GetObjectUnderMouse();
        }
        else if(Input.GetMouseButtonDown(0))
        {
            if(curSlot && curSlot.GetComponent<Slot>().slotItem) 
            { 
                followMouseImage.color = new Color(255, 255, 255, 255);
                followMouseImage.sprite = curSlot.GetComponent<Image>().sprite;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (curSlot && curSlot.GetComponent<Slot>().slotItem)
            {
                curSlotsItem = curSlot.GetComponent<Slot>().slotItem;

                GameObject newObj = GetObjectUnderMouse();
                if(newObj && newObj != curSlot)
                {
                    if(newObj.GetComponent<Slot>().slotItem)
                    {
                        Item ObjectItem = newObj.GetComponent<Slot>().slotItem; 
                        if(ObjectItem.itemID == curSlotsItem.itemID && ObjectItem.amountInStack != ObjectItem.maxStackSize)
                        {
                            curSlotsItem.transform.parent = null;
                            inv.AddItem(curSlotsItem, ObjectItem);
                        }
                        else
                        {
                            ObjectItem.transform.parent = curSlot.transform;
                            curSlotsItem.transform.parent = newObj.transform;
                        }
                    }
                    else
                    {
                        curSlotsItem.transform.parent = newObj.transform;
                    }
                }
            }
        }
        else
        {
            followMouseImage.sprite = null;
            followMouseImage.color = new Color(0, 0, 0, 0);
        }
    }

   GameObject GetObjectUnderMouse()
    {
        GraphicRaycaster rayCaster = GetComponent<GraphicRaycaster>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        rayCaster.Raycast(eventData, results);

        foreach(RaycastResult i in results)
        {
            if(i.gameObject.GetComponent<Slot>())
            {
                return i.gameObject;
            }
        }
        return null;
    }
}
