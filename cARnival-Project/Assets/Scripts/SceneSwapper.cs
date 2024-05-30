using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour
{
    public void SwapScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    
    public static void SwapSceneStatic(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }
}
