using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateModuleBoxScript : MonoBehaviour
{
    public TMP_Text moduleName;
    public TMP_Text moduleLanguage;
    public TMP_Text moduleComplexity;


    public void UpdateModuleBox(ModulesJson module)
    {
        moduleName.SetText(module.name);
        moduleLanguage.SetText("Language: " + module.language);
        moduleComplexity.SetText("Complexity: " + module.complexity.ToString());
    }
}
