using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic : MonoBehaviour
{

    public Sprite icon;
    public int cost;
    public string game;
    public int itemID; // Note - You need to get this from the database after creating the item in it.

    public Cosmetic (Cosmetic c)
    {
        itemID = c.itemID;
        icon = c.icon;
        cost = c.cost;
        game = c.game;
    }
}
