using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private APIManager api;
    [SerializeField] private SceneSwapper sceneSwapper;
    [SerializeField] private string nextScene;
    [SerializeField] private GameObject cosmeticManager;

    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text errorText;


    public void LoginPress()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        yield return StartCoroutine(APIManager.Login(username.text, password.text));

        if (!APIManager.isConnected)
        {
            errorText.SetText(APIManager.authenticationString);
        }
        else
        {
            Debug.Log("Successfully logged in");
            sceneSwapper.SwapScene(nextScene);
        }
        Instantiate(cosmeticManager);
    }
}
