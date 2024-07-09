using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemShopBox : MonoBehaviour
{
    public Cosmetic item;
    public int itemID;
    public TMP_Text costText;
    public Image itemDisplay;
    

    void Start()
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
            costText.SetText("Owned");
        }
        else
        {
            costText.SetText(item.cost.ToString());
        }
    }

    public void PurchaseItem(ref int userCurrency)
    {
        if (userCurrency >= item.cost && !CosmeticManager.userCosmeticInfo.ContainsKey(itemID))
        {
            userCurrency -= item.cost;
            StartCoroutine(item.PurchaseItem());
            costText.SetText("Owned");
        }
    }
}
