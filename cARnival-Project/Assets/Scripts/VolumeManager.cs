using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeText;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
        UpdateVolumeText();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
        UpdateVolumeText();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    private void UpdateVolumeText()
    {
        int volumePercentage = Mathf.RoundToInt(volumeSlider.value * 100);
        volumeText.text = volumePercentage.ToString() + "%";
    }
}
