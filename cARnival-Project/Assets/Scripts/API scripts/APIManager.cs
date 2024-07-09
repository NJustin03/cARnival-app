using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
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

    //private const string endpointURL = "https://chdr.cs.ucf.edu/elleapi/";
    private const string endpointURL = "https://www.elledevserver.xyz/elleapi/";
    private const string filePrefixURL = "https://www.elledevserver.xyz/elle";
    // private const string filePrefixURL = "https://chdr.cs.ucf.edu/elle";

    public static TokenJson token;

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

    public static AdaptiveValuesJson adaptiveValuesJson;
    public static AdaptiveValuesJson[] listOfALValues;

    public static ItemJson[] cosmeticList;
    public static PurchaseJson purchase;

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

    /// <summary>
    /// Calls the API to retrieve a random username that is compliant with ELLE guidelines.
    /// </summary>
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

    /// <summary>
    /// Registers the user's account in the ELLE database.
    /// </summary>
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
    /// <summary>
    /// Attempts to log in a user with username and password.
    /// </summary>
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
                    token = TokenJson.CreateTokenFromJson(authenticationString);
                    isConnected = true;
                }
            }
        }
    }

    /// <summary>
    /// Attempts to log out the user.
    /// </summary>
    public static IEnumerator Logout()
    {
        // Start with creating the url endpoint and the form.
        string logoutEndpoint = endpointURL + "logout";

        WWWForm form = new WWWForm();


        // Create a webrequest and fire it.

        using (UnityWebRequest attemptLogout = UnityWebRequest.Post(logoutEndpoint, form))
        {
            yield return attemptLogout.SendWebRequest();
            Debug.Log("Server responded: " + attemptLogout.downloadHandler.text);

            // If a connection error is received, print the result.
            if (attemptLogout.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error Logging out");
            }
            else
            {
                // Clear everything upon logging out.
                token = null;
                authenticationString = string.Empty;
                sessionID = null;
                isConnected = false;
                userModules = null;
                currentQuestions = null;
                currentImage = null;
                currentAudio = null;
                ModulesJsonObjects = null;
                moduleStats = null;
                adaptiveValuesJson = null;
                listOfALValues = null;
            }
        }
    }

    /// <summary>
    /// Attempts to send an email to the user with the username associated to that email.
    /// </summary>
    public static IEnumerator ForgotUsername(string email)
    {
        // Start with creating the url endpoint and the form.
        string forgotUsernameEndpoint = endpointURL + "forgotusername";

        WWWForm form = new WWWForm();
        form.AddField("email", email);


        // Create a webrequest and fire it.

        using (UnityWebRequest forgotUsernameRequest = UnityWebRequest.Post(forgotUsernameEndpoint, form))
        {
            yield return forgotUsernameRequest.SendWebRequest();
            Debug.Log("Server responded: " + forgotUsernameRequest.downloadHandler.text);

            // If a connection error is received, print the result.
            if (forgotUsernameRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error with username retrieval.");
            }
        }
    }

    /// <summary>
    /// Attempts to send an email to the user with the password reset.
    /// </summary>
    public static IEnumerator ForgotPassword(string email)
    {
        // Start with creating the url endpoint and the form.
        string forgotPasswordEndpoint = endpointURL + "forgotpassword";

        WWWForm form = new WWWForm();
        form.AddField("email", email);


        // Create a webrequest and fire it.

        using (UnityWebRequest forgotPasswordRequest = UnityWebRequest.Post(forgotPasswordEndpoint, form))
        {
            yield return forgotPasswordRequest.SendWebRequest();
            Debug.Log("Server responded: " + forgotPasswordRequest.downloadHandler.text);

            // If a connection error is received, print the result.
            if (forgotPasswordRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error with sending the password reset.");
            }
        }
    }

    /// <summary>
    /// Retrieves a list of modules associated with a user.
    /// </summary>
    public static IEnumerator GetAllModules()
    {

        string retrieveModulesEndpoint = endpointURL + "modules";
        using (UnityWebRequest listOfModulesRequest = UnityWebRequest.Get(retrieveModulesEndpoint))
        {
            listOfModulesRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return listOfModulesRequest.SendWebRequest();
            Debug.Log("Server responded: " + listOfModulesRequest.downloadHandler.text);
            ModulesJsonObjects = GetJsonArray<ModulesJson>(listOfModulesRequest.downloadHandler.text);
        }
    }

    /// <summary>
    /// Retrieves a list of questions and terms associated with the specified module.
    /// </summary>
    public static IEnumerator GetModule(int ID)
    {
        currentQuestions = null;

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

    /// <summary>
    /// Logs the start of a gameplay session.
    /// </summary>
    public static IEnumerator StartSession(int ID)
    {
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

    /// <summary>
    /// Logs the end of a gameplay session along with the score.
    /// </summary>
    public static IEnumerator EndSession(int points)
    {


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

    /// <summary>
    /// Retrieves the user's stats on every module associated with the account.
    /// </summary>
    public static IEnumerator RetrieveAllModuleStats()
    {


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

    public static IEnumerator LogAnswer(int termID, bool isCorrect)
    {


        string sessionEndpoint = endpointURL + "/loggedanswer";
        WWWForm form = new WWWForm();

        form.AddField("termID", termID);
        form.AddField("sessionID", sessionID);
        form.AddField("correct", isCorrect ? 1 : 0);

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

    /// <summary>
    /// Retrieves an image at the specified URL if it exists.
    /// </summary>
    public static IEnumerator RetrieveImage(string imgURL)
    {
        currentImage = null;
        string imageLink = filePrefixURL + imgURL;


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

    /// <summary>
    /// Retrieves an audio clip at the specified URL if it exists.
    /// </summary>
    public static IEnumerator RetrieveAudio(string audioURL)
    {
        currentAudio = null;

        string audioLink = filePrefixURL + audioURL;


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


    /// <summary>
    /// Retrieves the adaptive learning values associated with a term ID.
    /// </summary>
    public static IEnumerator RetrieveAdaptiveLearningValue(int termID)
    {

        string adaptiveRetrievalEndpoint = endpointURL + "/adaptivelearning/gettermvalue";
        WWWForm form = new WWWForm();
        form.AddField("termID", termID);

        using (UnityWebRequest adaptiveRetrievalRequest = UnityWebRequest.Post(adaptiveRetrievalEndpoint, form))
        {
            adaptiveRetrievalRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return adaptiveRetrievalRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (adaptiveRetrievalRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(adaptiveRetrievalRequest.error);
            }
            else
            {
                // Debug.Log(adaptiveRetrievalRequest.downloadHandler.text);
                adaptiveValuesJson = AdaptiveValuesJson.CreateAdaptiveFromJson(adaptiveRetrievalRequest.downloadHandler.text);
            }
        }
    }


    /// <summary>
    /// Retrieves the adaptive learning values associated with each term in the list.
    /// </summary>
    // Instead of a list of answers/terms, you could replace this with a CSV list of term IDs.
    public static IEnumerator RetrieveAllALValues(List<Answer> terms)
    {

        string adaptiveRetrievalEndpoint = endpointURL + "/adaptivelearning/gettermlistvalues";
        WWWForm form = new WWWForm();
        form.AddField("list_of_termIDs", ConvertTermListToCSV(terms));

        using (UnityWebRequest adaptiveRetrievalRequest = UnityWebRequest.Post(adaptiveRetrievalEndpoint, form))
        {
            adaptiveRetrievalRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return adaptiveRetrievalRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (adaptiveRetrievalRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(adaptiveRetrievalRequest.error);
            }
            else
            {
                listOfALValues = GetJsonArray<AdaptiveValuesJson>(adaptiveRetrievalRequest.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Updates the term's adaptive learning values.
    /// </summary>
    public static IEnumerator UpdateAdaptiveLearningValue(int termID, float activation_val, float decay_val, float alpha_val, string dates, string times)
    {

        string adaptiveUpdateEndpoint = endpointURL + "/adaptivelearning/updatetermvalues";
        WWWForm form = new WWWForm();
        form.AddField("termID", termID);
        form.AddField("activation_val", activation_val.ToString());
        form.AddField("decay_val", decay_val.ToString());
        form.AddField("alpha_val", alpha_val.ToString());
        form.AddField("dates", dates);
        form.AddField("times", times);

        using (UnityWebRequest adaptiveUpdateRequest = UnityWebRequest.Post(adaptiveUpdateEndpoint, form))
        {
            adaptiveUpdateRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return adaptiveUpdateRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (adaptiveUpdateRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(adaptiveUpdateRequest.error);
            }
            else
            {
                Debug.Log(adaptiveUpdateRequest.downloadHandler.text);
            }
        }
    }

    // Store endpoints (need to be tested)
    
    public static IEnumerator RetrieveUserItems(string game)
    {
        string retrieveItemsEndpoint = endpointURL + "/store/user/items?game=" + game + "&userID=" + token.id;
        using (UnityWebRequest itemsRequest = UnityWebRequest.Get(retrieveItemsEndpoint))
        {
            itemsRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return itemsRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (itemsRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(itemsRequest.error);
            }
            else
            {
                // Debug.Log(itemsRequest.downloadHandler.text);
                cosmeticList = GetJsonArray<ItemJson>(itemsRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator WearItem(int userItemID, bool isWearing, bool replaceItem)
    {
        string retrieveItemsEndpoint = endpointURL + "/store/wear";
        WWWForm form = new WWWForm();
        form.AddField("userItemID", userItemID);
        form.AddField("isWearing", isWearing.ToString());
        form.AddField("replaceItem", replaceItem.ToString());



        using (UnityWebRequest itemsRequest = UnityWebRequest.Put(retrieveItemsEndpoint, form.data))
        {
            itemsRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return itemsRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (itemsRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(itemsRequest.error);
            }
            else
            {
                Debug.Log(itemsRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator PurchaseItem(int itemID, string game)
    {
        string purchaseItemsEndpoint = endpointURL + "/store/purchase";
        WWWForm form = new WWWForm();
        form.AddField("itemID", itemID);
        form.AddField("game", game);


        using (UnityWebRequest itemsRequest = UnityWebRequest.Post(purchaseItemsEndpoint, form))
        {
            itemsRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return itemsRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (itemsRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(itemsRequest.error);
            }
            else
            {
                Debug.Log(itemsRequest.downloadHandler.text);
                purchase = PurchaseJson.FromJson(itemsRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator LogItemUse()
    {
        string logItemsEndpoint = endpointURL + "/store/purchase";
        WWWForm form = new WWWForm();
        form.AddField("userID", token.id);
        form.AddField("sessionID", sessionID);


        using (UnityWebRequest itemsRequest = UnityWebRequest.Post(logItemsEndpoint, form))
        {
            itemsRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

            yield return itemsRequest.SendWebRequest();

            // If a connection error is received, print the result.
            if (itemsRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(itemsRequest.error);
            }
            else
            {
                Debug.Log(itemsRequest.downloadHandler.text);
            }
        }
    }

    private static string ConvertTermListToCSV(List<Answer> termList)
    {
        StringBuilder listOfIds = new StringBuilder();
        foreach (Answer answer in termList)
        {
            listOfIds.Append(answer.GetTermID().ToString());
            listOfIds.Append(",");
        }
        listOfIds.Length--;
        return listOfIds.ToString();
    }
}
