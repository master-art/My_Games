using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Slot : MonoBehaviourPunCallbacks
{
    public Item slotItem;
    private Text amountText;
    private Sprite defaultIcon;
  //  private GameObject playerPos;
    PhotonView PV;

    private void Awake()
    {
        //playerPos = GameObject.Find("Player");
        PV = GetComponent<PhotonView>();
    }

    public void CustomStart()
    {
        defaultIcon = GetComponent<Image>().sprite;
        amountText = transform.GetChild(0).GetComponent<Text>();
    }
    
    [PunRPC]
    public void DropItem()
    {

        if(slotItem)
        {
            slotItem.transform.parent = null;
            slotItem.gameObject.SetActive(true);
            slotItem.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(0, 0.5f, 1.8f);
        }
    }

    [PunRPC]
    public void CheckForItem()
    {
       if(transform.childCount > 1)
        {
            slotItem = transform.GetChild(1).GetComponent<Item>();
            GetComponent<Image>().sprite = slotItem.itemIcon;
            if (slotItem.amountInStack > 1)
                amountText.text = slotItem.amountInStack.ToString();
            
        }
        else
        {
            slotItem = null;
            GetComponent<Image>().sprite = defaultIcon;
            amountText.text = " ";
        }
    }
}
