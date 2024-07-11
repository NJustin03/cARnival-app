using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static int coins = 0;    // Currency System for the game.

    public TextPrefabScript coinText;
    public ItemShopBox currentlySelectedItem;

    private void Awake()
    {
        coinText.Text = coins.ToString();
    }
    // Function which adds coins to the user's total in that session.
    public static void AddCoins(int newCoins)
    {
        coins += newCoins;
    }

    // Function which resets coins on game logout.
    public static void ResetCoins()
    {
        coins = 0;
    }

    public void SetCurrentItemBox(ItemShopBox current)
    {
        currentlySelectedItem = current;
    }

    public void PurchaseItem()
    {
        if (coins >= currentlySelectedItem.item.cost && !CosmeticManager.userCosmeticInfo.ContainsKey(currentlySelectedItem.item.itemID))
        {
            coins -= currentlySelectedItem.item.cost;
            StartCoroutine(currentlySelectedItem.PurchaseItem());
            coinText.Text = coins.ToString();
        }

    }
}
