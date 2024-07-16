using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class MusicObject : MonoBehaviour
{
    public AudioClip musicFile;
    public string musicName;

    public AudioClip GetMusicFile()
    { return musicFile; }

    public string GetMusicName() 
    { return musicName; }
}
