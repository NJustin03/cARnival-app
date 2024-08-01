using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class which focuses on swapping between game scenes.
public class SceneSwapper : MonoBehaviour
{
    // Static String which holds the name of the next scene to swap to (used for saving which game was chosen).
    public static string NextChosenScene;

    // Function to swap a scene.
    public void SwapScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    // Function to swap a scene (STATIC).
    public static void SwapSceneStatic(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    // Function to set the next game scene.
    public static void SetNextGameScene(string scene)
    {
        NextChosenScene = scene;
    }

    // Function to swap to the game the user has chosen.
    public static void SwapToChosenGame()
    {
        SceneManager.LoadSceneAsync(NextChosenScene);
    }
}
