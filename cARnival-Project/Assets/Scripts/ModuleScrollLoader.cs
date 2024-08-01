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

    // Function which retrieves all modules from the API Manager.
    private IEnumerator RetrieveModules()
    {
        yield return StartCoroutine(APIManager.GetAllModules());
        AddModulesToList();
    }

    // Function which adds all modules to a list, instantiates a container and populates them properly.
    void AddModulesToList()
    {
        foreach (ModulesJson module in APIManager.ModulesJsonObjects)
        {
            GameObject temp = Instantiate(moduleBoxPrefab, modulesContainer.transform);
            temp.GetComponent<UpdateModuleBoxScript>().UpdateModuleBox(module);
        }
    }
}
