using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    public Color InactiveColor, ActiveColor;
    public TextMeshProUGUI[] ButtonTexts;
    private string[] originalTexts;

    // Start is called before the first frame update
    void Start()
    {
        originalTexts = new string[ButtonTexts.Length];
        for (int i = 0; i < ButtonTexts.Length; i++)
        {
            originalTexts[i] = ButtonTexts[i].text;
        }

        SwitchToTab(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToTab(int TabID)
    {
        foreach (GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        foreach (Image im in TabButtons)
        {
            im.sprite = InactiveTabBG;
        }
        TabButtons[TabID].sprite = ActiveTabBG;

        InactiveColor.a = 1f;
        ActiveColor.a = 1f;
        for (int i = 0; i < ButtonTexts.Length; i++)
        {
            ButtonTexts[i].color = InactiveColor;
            ButtonTexts[i].text = originalTexts[i];
        }
        
        ButtonTexts[TabID].color = ActiveColor;
        ButtonTexts[TabID].text = "<u>" + originalTexts[TabID] + "</u>";
    }
}
