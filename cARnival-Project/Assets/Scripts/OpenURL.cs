using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that is used to handle opening urls outside of the app.
public class OpenURL : MonoBehaviour
{
    // Function specific for opening the signup page.
    public void OpenWebsite(string url = "https://chdr.cs.ucf.edu/elle/signup/")
    {
        Application.OpenURL(url);
    }

    // Generic Function to open a given URL.
    public void OpenGivenWebsite(string url)
    {
        Application.OpenURL(url);
    }
}
