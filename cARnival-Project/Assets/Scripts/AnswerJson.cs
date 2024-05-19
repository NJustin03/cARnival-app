using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnswerJson
{
    public int termID;
    public string audioLocation;
    public string imageLocation;
    public string front;
    public string back;
    public string type;
    public string gender;
    public string language;

    public static AnswerJson CreateAnswerFromJson(string jsonString)
    {
        return JsonUtility.FromJson<AnswerJson>(jsonString);
    }

    public string CreateJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}