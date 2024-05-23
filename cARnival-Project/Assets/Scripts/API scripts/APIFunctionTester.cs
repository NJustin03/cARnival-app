using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APIFunctionTester : MonoBehaviour
{
    public ModuleManager moduleManager;
    public RawImage image;
    public AudioSource audioSource;

    public void LoginTest()
    {
        StartCoroutine(APIManager.Login("dearjerky8", "123"));
    }

    public void GenerateModuleList()
    {
        if (APIManager.authenticationString != "")
        {
            StartCoroutine(APIManager.GetAllModules());
        }
    }

    public void GetSpecificModule()
    {
        StartCoroutine(moduleManager.LoadQuestionsAndAnswers(1));
    }

    public IEnumerator TestRegister()
    {
        yield return StartCoroutine(APIManager.GenerateUsername());
        StartCoroutine(APIManager.Register("password"));
    }

    public void RetrieveStats()
    {
        StartCoroutine(APIManager.RetrieveAllModuleStats());
    }

    public void TestImageDownload()
    {
        StartCoroutine(DisplayImages());
    }

    private IEnumerator DisplayImages()
    {
        foreach (int i in moduleManager.answerIDs)
        {
            Debug.Log(moduleManager.currentModuleAnswers[i]);
            image.texture = moduleManager.currentModuleAnswers[i].GetImage();
            yield return new WaitForSeconds(3f);
        }
    }

    
    public void TestAudioDownload()
    {
        StartCoroutine(PlayAudio());
    }

    private IEnumerator PlayAudio()
    {
        foreach (int i in moduleManager.answerIDs)
        {
            Debug.Log(moduleManager.currentModuleAnswers[i]);
            audioSource.clip = moduleManager.currentModuleAnswers[i].GetAudio();
            audioSource.Play();
            yield return new WaitForSeconds(3f);
        }
    }
}
