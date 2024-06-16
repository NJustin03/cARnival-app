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

    private IEnumerator RetrieveStats()
    {
        yield return StartCoroutine(APIManager.RetrieveAllModuleStats());
        AddStatsToList();
    }

    void AddStatsToList()
    {
        foreach (ModuleStatsJson stats in APIManager.moduleStats)
        {
            GameObject temp = Instantiate(statsBoxPrefab, statsContainer.transform);
            temp.GetComponent<UpdateStatsBoxScript>().UpdateStatsBox(stats);
        }
    }
}
