using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class FishingGameManager : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public Camera arCamera;

    public static FishingGameManager shared;
    // Likely will need to change this once words actually pulled
    public ArrayList Terms;
    public Material DuckColor;
    //DuckA to be used as correct term
    public DuckPrefab DuckA, DuckB, DuckC, DuckD, duckSelected;
    public FishingGameQuestionBoard QuestionBoard;


    [SerializeField]
    private AudioSource FishingGameAudioSource = null;

    [SerializeField]
    private FishingGameQuestionBoard fishingGameQuestionBoard = null;

    private int score = 0;
    private int numErrors = 0;
    private string currentWord = null;

    private void Awake()
    {
        shared = this;
        
        //Adding a few terms for testing
        Terms.Add("Apple");
        Terms.Add("Bannana");
        Terms.Add("Orange");
        Terms.Add("Kiwi");
    }

    private void Start()
    {
        // TODO: Start the game
        PlayNewWord();
    }

    private void Update()
    {
        // TODO: Perform raycast to see if we are clicking on a duck and determine if we need to select this word?
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider != null && hitObject.collider.gameObject.CompareTag("duck"))
                    {
                        Debug.Log("Duck wCreith collider tapped");

                        duckSelected = hitObject.collider.gameObject.GetComponent<DuckPrefab>();

                        if (duckSelected != null)
                        {
                            SelectWord(duckSelected);
                        }
                    }
                }
            }
        }
    }

    private void PlayNewWord()
    {   
        //TODO:Change type 'string' to word class when created
        string newWord = null;
        numErrors = 0;

        //Get the new word randomly from a term array for now
        var randomIndex = UnityEngine.Random.Range(0, Terms.Count - 1);

        //Do not destroy the value this holds when mofiying code
        newWord = (string)Terms[randomIndex];
        Terms.Remove(newWord);

        // TODO: Add logic to randomize duck colors
        ArrayList tempWords = null;
        for (int i = 0; i >= 2; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, Terms.Count - 1);
            tempWords.Add(Terms[randomIndex]);
            Terms.RemoveAt(randomIndex);
        }
        //configure all the ducks
        DuckA.ConfigureDuck(newWord, DuckColor);
        DuckA.ConfigureDuck((string)tempWords[0], DuckColor);
        DuckA.ConfigureDuck((string)tempWords[1], DuckColor);
        DuckA.ConfigureDuck((string)tempWords[2], DuckColor);
        //TODO: configure question board
        QuestionBoard.ConfigureWithWord(newWord);
        //Add terms back into main term list
        Terms.Add(newWord);
        Terms.Add(tempWords[0]);
        Terms.Add(tempWords[1]);
        Terms.Add(tempWords[2]);
    }

    public void PlayAudioClip(AudioClip clip) => FishingGameAudioSource.PlayOneShot(clip);

    // TODO: See what the duck script is 
    public void SelectWord(DuckPrefab selectedDuck)
    {
        // TODO: Test to see if the word is correct or not
        if (selectedDuck.Text.Text == currentWord)
        {
            score++;
            PlayNewWord();
        }
        else
        {
            if (numErrors == 0)
            {
                selectedDuck.SetActive(false);
                return;
            }
            //TODO: Add logic for giving correct answer after second incorrect guess
            else if (numErrors > 0)
            {
                PlayNewWord();
                return;
            }
        }
    }

    public void QuitGame()
    {
        // TODO: Add the summary functionality if needed
        // TODO: Make sure the loading of the scene is the correct scene GameScene?
        SceneManager.LoadScene("Home");
    }

    public void ShowSettings()
    {
        // TODO: Show the Settings Prefab 
        // Ex: SetActive call on a settings prefab

        // TODO: Pause the game
    }
}
