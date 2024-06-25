using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public GameObject background;
    public Button button_active;
    public Button button_inactive;

    // Start is called before the first frame update
    void Start()
    {
        background.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveBackground()
    {
        background.SetActive(true);
    }

    public void InactiveBackground()
    {
        background.SetActive(false);
    }

}
