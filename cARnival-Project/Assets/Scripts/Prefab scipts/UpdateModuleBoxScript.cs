using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateModuleBoxScript : MonoBehaviour
{
    public TMP_Text moduleName;
    public TMP_Text moduleLanguage;
    public TMP_Text moduleComplexity;
    public int moduleID;
    private ModuleManager moduleManager;

    public void Start()
    {
        moduleManager = FindAnyObjectByType<ModuleManager>();
    }

    public void UpdateModuleBox(ModulesJson module)
    {
        moduleID = module.moduleID;
        moduleName.SetText(module.name);
        moduleLanguage.SetText("Language: " + module.language);
        moduleComplexity.SetText("Complexity: " + module.complexity.ToString());
    }

    public void LoadModuleOnPress()
    {
        StartCoroutine(LoadModule());
    }

    private IEnumerator LoadModule()
    {
        yield return StartCoroutine(moduleManager.LoadQuestionsAndAnswers(moduleID));
        Debug.Log("Loaded module number: " + moduleID + "\n Number of Terms: " + moduleManager.answerIDs.Count);
        SceneSwapper.SwapToChosenGame();
    }
}
