using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemJson
{
    public int userItemID;
    public int userID;
    public int itemID;
    public string timeOfPurchase;
    public string game;
    public int isWearing;
    public string color;

    public static ItemJson CreateItem(string jsonString)
    {
        return JsonUtility.FromJson<ItemJson>(jsonString);
    }
}
