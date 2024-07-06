using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic : MonoBehaviour
{

    public Texture2D icon;
    public bool isPurchased;
    public int cost;
    public string game;
    public int itemID; // Note - You need to get this from the database after creating the item in it.
    public int userItemID; // Specific to the current account - used to identify that user's item in the database.

    public void PurchaseItem(ref int userCurrency)
    {
        if (userCurrency >= cost)
        {
            userCurrency -= cost;
            isPurchased = true;
            StartCoroutine(APIManager.PurchaseItem(itemID, game));
        }
    }
}
