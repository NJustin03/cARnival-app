using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PurchaseJson
{

    public string message;
    public int userItemID;

    public static PurchaseJson FromJson(string jsonString)
    {
        return JsonUtility.FromJson<PurchaseJson>(jsonString);
    }
}
