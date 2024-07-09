using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic : MonoBehaviour
{

    public Sprite icon;
    public bool isPurchased;
    public int cost;
    public string game;
    public int itemID; // Note - You need to get this from the database after creating the item in it.
    public int userItemID; // Specific to the current account - used to identify that user's item in the database.

    public Cosmetic (Cosmetic c)
    {
        itemID = c.itemID;
        icon = c.icon;
        isPurchased = c.isPurchased;
        cost = c.cost;
        game = c.game;
        userItemID = c.userItemID;
    }
    public IEnumerator PurchaseItem()
    {
        yield return StartCoroutine(APIManager.PurchaseItem(itemID, game));
        CosmeticManager.AddToOwned(itemID);
    }
}
