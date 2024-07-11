using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    public SceneSwapper swapper;
    public void ReloadAll()
    {
        StoreManager.ResetCoins();
        if (FindAnyObjectByType(typeof(ModuleManager)))
        {
            ModuleManager temp = FindAnyObjectByType<ModuleManager>();
            temp.ClearModules();
        }
        StartCoroutine(APILogOut());
    }

    private IEnumerator APILogOut()
    {
        yield return StartCoroutine(APIManager.Logout());
        swapper.SwapScene("StartPage");
    }
}
