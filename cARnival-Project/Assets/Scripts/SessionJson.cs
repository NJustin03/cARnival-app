using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SessionJson
{
    public string sessionID;

    public static SessionJson CreateSessionFromJson(string jsonString)
    {
        return JsonUtility.FromJson<SessionJson>(jsonString);
    }
}
