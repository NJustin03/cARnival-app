using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static T[] GetJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    private const string endpointURL = "https://chdr.cs.ucf.edu/elleapi/";
    private const string filePrefixURL = "https://chdr.cs.ucf.edu/elle";
    public static string authenticationString;
    public static string randomUsername;
    public static string sessionID;

    public static bool isConnected;

    public static Dictionary<int, string> userModules;
    public static QuestionJson[] currentQuestions;

    public static Texture2D currentImage;
    public static AudioClip currentAudio;

    public static ModulesJson[] ModulesJsonObjects;
    public static ModuleStatsJson[] moduleStats;


    private void Awake()
    {
        if (FindObjectsOfType<APIManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        userModules = new Dictionary<int, string>();
        isConnected = false;
    }

    public static IEnumerator GenerateUsername()
    {
        string usernameEndpoint = endpointURL + "generateusername";
        using (UnityWebRequest attemptRandomUsername = UnityWebRequest.Get(usernameEndpoint))
        {
            yield return attemptRandomUsername.SendWebRequest();
            randomUsername = attemptRandomUsername.downloadHandler.text;
            Debug.Log("Server responded: " + randomUsername);
        }
    }

    public static IEnumerator Register(string password)
    {
        RandomUsernameJson generatedUsername = RandomUsernameJson.CreateUsernameFromJson(randomUsername);
        string registerEndpoint = endpointURL + "register";

        WWWForm form = new WWWForm();
        form.AddField("username", generatedUsername.username);
        form.AddField("password", password);
        form.AddField("password_confirm", password);    // Note - should only work if the frontend ensures password and password_confirm are the same before doing this call.

        using (UnityWebRequest attemptRegister = UnityWebRequest.Post(registerEndpoint, form))
        {
            yield return attemptRegister.SendWebRequest();
            Debug.Log("Server responded: " + attemptRegister.downloadHandler.text);
        }
    }

    // Function to submit a username and password.
    public static IEnumerator Login(string username, string password)
    {
        authenticationString = string.Empty;
        // Start with creating the url endpoint and the form.
        string loginEndpoint = endpointURL + "login";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        // Create a webrequest and fire it.

        using (UnityWebRequest attemptLogin = UnityWebRequest.Post(loginEndpoint, form))
        {
            yield return attemptLogin.SendWebRequest();
            Debug.Log("Server responded: " + attemptLogin.downloadHandler.text);

            // If a connection error is received, print the result.
            if (attemptLogin.result == UnityWebRequest.Result.ConnectionError)
            {
                authenticationString = "Error: " + attemptLogin.error;
                isConnected = false; 
            }
            else 
            {
                authenticationString = attemptLogin.downloadHandler.text;
                if (authenticationString.Contains("Error") || authenticationString.Length < 1)
                {
                    isConnected = false;
                    authenticationString = authenticationString.Replace("\"", "");
                    authenticationString = authenticationString.Replace("\n", "");
                    authenticationString = authenticationString.Replace("{", "");
                    authenticationString = authenticationString.Replace("}", "");
                }
                else
                {
                    isConnected = true;
                }
            }
        }
    }


    // Function to receive a list of modules associated with a user.
    public static IEnumerator GetAllModules()
    {
        Debug.Log(authenticationString);
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);
        string retrieveModulesEndpoint = endpointURL + "modules";
        using (UnityWebRequest listOfModulesRequest = UnityWebRequest.Get(retrieveModulesEndpoint))
        {
            listOfModulesRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return listOfModulesRequest.SendWebRequest();
            Debug.Log("Server responded: " + listOfModulesRequest.downloadHandler.text);
            ModulesJsonObjects = GetJsonArray<ModulesJson>(listOfModulesRequest.downloadHandler.text);
        }
    }

    public static IEnumerator GetModule(int ID)
    {
        currentQuestions = null;
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);
        string moduleEndpoint = endpointURL + "/modulequestions";

        WWWForm form = new WWWForm();
        form.AddField("moduleID", ID);

        using (UnityWebRequest getModuleRequest = UnityWebRequest.Post(moduleEndpoint, form))
        {
            getModuleRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return getModuleRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (getModuleRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(getModuleRequest.error);
            }
            string moduleJsonString = getModuleRequest.downloadHandler.text;

            currentQuestions = GetJsonArray<QuestionJson>(moduleJsonString);
        }
        Debug.Log("Finished retrieval");
    }

    public static IEnumerator StartSession(int ID)
    {
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);

        string sessionEndpoint = endpointURL + "/session";
        WWWForm form = new WWWForm();

        form.AddField("moduleID", ID);
        form.AddField("platform", "mob");

        using (UnityWebRequest startSessionRequest = UnityWebRequest.Post(sessionEndpoint, form))
        {
            startSessionRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return startSessionRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (startSessionRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(startSessionRequest.error);
            }

            string sessionJsonString = startSessionRequest.downloadHandler.text;

            SessionJson session = SessionJson.CreateSessionFromJson(sessionJsonString);
            sessionID = session.sessionID;
        }
    }

    public static IEnumerator EndSession(int points)
    {
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);

        string sessionEndpoint = endpointURL + "/endsession";
        WWWForm form = new WWWForm();

        form.AddField("sessionID", sessionID);
        form.AddField("playerScore", points);

        using (UnityWebRequest endSessionRequest = UnityWebRequest.Post(sessionEndpoint, form))
        {
            endSessionRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return endSessionRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (endSessionRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(endSessionRequest.error);
            }
        }
    }

    public static IEnumerator RetrieveAllModuleStats()
    {
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);

        string statsEndpoint = endpointURL + "/allmodulestats";

        using (UnityWebRequest statsSessionRequest = UnityWebRequest.Get(statsEndpoint))
        {
            statsSessionRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return statsSessionRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (statsSessionRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(statsSessionRequest.error);
            }

            string stats = statsSessionRequest.downloadHandler.text;
            moduleStats = GetJsonArray<ModuleStatsJson>(stats);
        }
    }

    public static IEnumerator RetrieveImage(string imgURL)
    {
        currentImage = null;
        string imageLink = filePrefixURL + imgURL;
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);

        using (UnityWebRequest pictureRequest = UnityWebRequestTexture.GetTexture(imageLink))
        {
            pictureRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return pictureRequest.SendWebRequest();
            // If a connection error is received, print the result.
            if (pictureRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pictureRequest.error);
            }
            else
            {
                currentImage = ((DownloadHandlerTexture)pictureRequest.downloadHandler).texture;
            }
        }
    }

    public static IEnumerator RetrieveAudio(string audioURL)
    {
        currentAudio = null;

        string audioLink = filePrefixURL + audioURL;
        TokenJson token = TokenJson.CreateTokenFromJson(authenticationString);

        using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(audioLink, AudioType.UNKNOWN))
        {
            audioRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return audioRequest.SendWebRequest();
            // If a connection error is received, print the result.
            if (audioRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(audioRequest.error);
            }
            else
            {
                AudioClip myClip = null;
                try
                {
                    myClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                }
                catch (Exception)
                {
                    Debug.Log("No audio to download");
                }

                if (myClip != null)
                {
                    // Debug.Log("Saving myClip: " + "\"" + audioURL + "\"");
                    currentAudio = myClip;
                }
            }
        }
    }

    public QuestionJson[] GetQuestions()
    {
        return currentQuestions;
    }
}
