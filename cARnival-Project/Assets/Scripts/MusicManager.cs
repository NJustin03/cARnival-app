using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static List<MusicObject> musicFiles;
    public static MusicObject duckMusic;
    public static MusicObject archeryMusic;
    public static MusicObject basketballMusic;
    public static float volume;
    public AudioSource audioSource;

    public string currentGame;
    public TMPro.TMP_Dropdown dropdown;
    private string currentSongTitle;


    // To Do: Determine how/what to set as default music.
    private void Start()
    {
        volume = 1f;

        // On initial startup, the app should load all music files stored in the resources folder.
        if (musicFiles == null)
        {
            var items = Resources.LoadAll<MusicObject>("Music");
            musicFiles = new List<MusicObject>(items);
            archeryMusic = musicFiles[0];
            duckMusic = musicFiles[1];
            basketballMusic = musicFiles[2];
        }

        // This component should only be used in game. Manually assign the audio component and tell which game's audio to play.
        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.loop = true;
            if (currentGame == "duck")
            {
                audioSource.clip = duckMusic.GetMusicFile();
                currentSongTitle = duckMusic.GetMusicName();
            }
            else if (currentGame == "basketball")
            {
                audioSource.clip = basketballMusic.GetMusicFile();
                currentSongTitle = basketballMusic.GetMusicName();
            }
            else if (currentGame == "archery")
            {
                audioSource.clip = archeryMusic.GetMusicFile();
                currentSongTitle = archeryMusic.GetMusicName();
            }
            audioSource.Play();
        }
        
        // If a dropdown is in the scene and assigned, this will populate the dropdown for the settings.
        if (dropdown != null)
        {
            SetDropdown();
        }
    }

    // Function which sets the volume of the audiosource for all music.
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", newVolume);
    }

    // Function to set the current music for the duck game.
    public void SetDuckMusic(MusicObject duckMusicChoice)
    {
        duckMusic = duckMusicChoice;
    }

    // Function to set the current music for the archery game.
    public void SetArcheryMusic(MusicObject archeryMusicChoice)
    {
        archeryMusic = archeryMusicChoice;
    }

    // Function to set the current music for the basketball game.
    public void SetBasketballMusic(MusicObject basketballMusicChoice)
    {
        basketballMusic = basketballMusicChoice;
    }

    // Function to populate a dropdown with the list of currently available music.
    private void SetDropdown()
    {
        dropdown.ClearOptions();
        for (int i = 0; i < musicFiles.Count; i++)
        {
            dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = musicFiles[i].GetMusicName() });
            if (currentSongTitle == musicFiles[i].GetMusicName())
            {
                dropdown.value = i;
            }
        }
    }

    // Function to set the a new song for the audio player.
    public void PlayNewMusic()
    {
        if (currentGame == "duck")
        {
            SetDuckMusic(musicFiles[dropdown.value]);
            audioSource.clip = duckMusic.GetMusicFile();
        }
        else if (currentGame == "basketball")
        {
            SetBasketballMusic(musicFiles[dropdown.value]);
            audioSource.clip = basketballMusic.GetMusicFile();
        }
        else if (currentGame == "archery")
        {
            SetArcheryMusic(musicFiles[dropdown.value]);
            audioSource.clip = archeryMusic.GetMusicFile();
        }
        audioSource.Play();

    }
}

