using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemShopBox : MonoBehaviour
{
    public Cosmetic item;
    public int itemID;
    public GameObject costBox;
    public TextPrefabScript costText;

    public GameObject statusBox;
    public TextPrefabScript statusText;

    public Image itemDisplay;
    

    void Awake()
    {
        item = CosmeticManager.RetrieveItem(itemID);
        if (item == null )
        {
            Debug.Log("No Item Found");
            return;
        }
        else
        {
            Debug.Log("Here!");

        }
        itemDisplay.sprite = item.icon;
        if (CosmeticManager.userCosmeticInfo.ContainsKey(itemID))
        {
            costBox.SetActive(false);
            if (CosmeticManager.CheckIfEquipped(itemID))
            {
                statusText.Text = "Equipped";
            }
            else
            {
                statusText.Text = "Owned";
            }
            statusBox.SetActive(true);
        }
        else
        {
            costText.Text = item.cost.ToString();
        }
    }

    public void PurchaseItem(ref int userCurrency)
    {
        if (userCurrency >= item.cost && !CosmeticManager.userCosmeticInfo.ContainsKey(itemID))
        {
            userCurrency -= item.cost;
            StartCoroutine(APIManager.PurchaseItem(item.itemID, item.game, 0));
            CosmeticManager.AddToOwned(item.itemID);
            statusText.Text = "Owned";
            costBox.SetActive(false);
            statusBox.SetActive(true);
        }
    }
}
