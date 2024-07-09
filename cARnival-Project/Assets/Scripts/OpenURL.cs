using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void OpenWebsite(string url = "https://chdr.cs.ucf.edu/elle/signup/")
    {
        Application.OpenURL(url);
    }
}
