using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieChartController : MonoBehaviour
{
    public Image fill;
    //public float score;
    public TextMeshProUGUI num_percentage;
    // Start is called before the first frame update
    /*void Start()
    {
        setValues(score);
        setPercentage(score);
    }*/

    public void UpdateStats(float score)
    {
        setPercentage(score);
        setValues(score);
    }

    public void setValues(float score)
    {
        fill.fillAmount = score;
    }

    public void setPercentage(float score)
    {
        float percentage = score * 100f;
        num_percentage.text = percentage.ToString("F1") + "%";
    }

}
