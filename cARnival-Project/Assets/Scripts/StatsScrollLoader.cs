using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsScrollLoader : MonoBehaviour
{
    public GameObject statsBoxPrefab;
    public GameObject statsContainer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RetrieveStats());
    }
    // Function which retrieves the stats from the server and adds them to a list.
    private IEnumerator RetrieveStats()
    {
        yield return StartCoroutine(APIManager.RetrieveAllModuleStats());
        AddStatsToList();
    }

    // Function which adds all module stats to a list, instantiates a container and populates them properly.
    void AddStatsToList()
    {
        foreach (ModuleStatsJson stats in APIManager.moduleStats)
        {
            GameObject temp = Instantiate(statsBoxPrefab, statsContainer.transform);
            temp.GetComponent<UpdateStatsBoxScript>().UpdateStatsBox(stats);
        }
    }
}
