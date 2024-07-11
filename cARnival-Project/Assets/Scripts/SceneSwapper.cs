using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour
{

    public static string NextChosenScene;
    public void SwapScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public static void SwapSceneStatic(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public static void SetNextGameScene(string scene)
    {
        NextChosenScene = scene;
    }

    public static void SwapToChosenGame()
    {
        SceneManager.LoadSceneAsync(NextChosenScene);
    }
}
