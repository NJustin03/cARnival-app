using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public GameObject background;
    public Button button_active;
    public Button button_inactive;

    public void ActiveBackground()
    {
        background.SetActive(true);
    }

    public void InactiveBackground()
    {
        background.SetActive(false);
    }

}
