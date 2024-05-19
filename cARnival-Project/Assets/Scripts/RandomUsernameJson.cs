using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomUsernameJson
{
    public string username;
    public static RandomUsernameJson CreateUsernameFromJson(string jsonString)
    {
        return JsonUtility.FromJson<RandomUsernameJson>(jsonString);
    }
}
