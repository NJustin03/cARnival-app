using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPrefab : MonoBehaviour
{
    public float timeLeft;
    public TextPrefabScript timerText;

    private bool timerOn = false;


    // Start is called before the first frame update
    void Start()
    {
        timerOn = true;
        timeLeft = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                Countdown(timeLeft);

            }
            else
            {
                timeLeft = 0;
                timerOn = false;
            }
        } 
    }

    void Countdown(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.Text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
