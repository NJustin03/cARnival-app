using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStatsBoxScript : MonoBehaviour
{
    public PieChartController pieChart;
    public TextPrefabScript moduleName;
    public TextPrefabScript timeCount;

    public void UpdateStatsBox(ModuleStatsJson stats)
    {
        pieChart.UpdateStats(stats.averageScore);
        moduleName.Text = stats.name;
        string[] averageSessionLength = stats.averageSessionLength.Split(':');
        if (averageSessionLength[0].Contains("days"))
        {
            string[] daysAndHours = averageSessionLength[0].Split("days,");
            averageSessionLength[0] = (int.Parse(daysAndHours[0]) * 24 + int.Parse(daysAndHours[1])).ToString();
        }

        timeCount.Text = averageSessionLength[0] + " hours " + averageSessionLength[1] + " minutes";

    }
}
