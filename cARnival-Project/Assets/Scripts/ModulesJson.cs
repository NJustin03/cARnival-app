using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ModulesJson
{
    public int moduleID;
    public string name;
    public string language;
    public int complexity;
    public int userID;
    public int isPastaModule;

    public static ModulesJson CreateModuleFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ModulesJson>(jsonString);
    }
}
