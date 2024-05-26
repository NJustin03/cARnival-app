using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleScrollLoader : MonoBehaviour
{

    public GameObject moduleBoxPrefab;
    public GameObject modulesContainer;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RetrieveModules());
    }

    private IEnumerator RetrieveModules()
    {
        yield return StartCoroutine(APIManager.GetAllModules());
        AddModulesToList();
    }

    void AddModulesToList()
    {
        foreach (ModulesJson module in APIManager.ModulesJsonObjects)
        {
            GameObject temp = Instantiate(moduleBoxPrefab, modulesContainer.transform);
            temp.GetComponent<UpdateModuleBoxScript>().UpdateModuleBox(module);
        }
    }
}
