using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModuleStatsJson
{

    public string name;
    public float averageScore;
    public int moduleID;
    public string averageSessionLength;

    public static ModuleStatsJson CreateModuleStatsFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ModuleStatsJson>(jsonString);
    }

    public string CreateJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
