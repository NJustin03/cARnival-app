using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class AdaptiveValuesJson
{
    public int userID;
    public int termID;
    public float activation_val;
    public float decay_val;
    public float alpha_val;
    public string dates;
    public string times;

    public static AdaptiveValuesJson CreateAdaptiveFromJson(string jsonString)
    {
        return JsonUtility.FromJson<AdaptiveValuesJson>(jsonString);
    }

    public string CreateJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
