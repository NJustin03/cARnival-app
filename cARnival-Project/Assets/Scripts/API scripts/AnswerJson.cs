using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnswerJson
{
    public int termID;
    public float activation;
    public float decay;
    public float intercept;
    public string audioLocation;
    public string imageLocation;
    public string front;
    public string back;
    public string type;
    public string gender;
    public string language;
    public string presentationTimes;
    public string dateTime;

    

    public static AnswerJson CreateAnswerFromJson(string jsonString)
    {
        return JsonUtility.FromJson<AnswerJson>(jsonString);
    }

    public string CreateJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}