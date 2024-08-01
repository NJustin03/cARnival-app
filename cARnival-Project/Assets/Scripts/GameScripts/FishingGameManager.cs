using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using System.Collections;

public class FishingGameManager : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public Camera arCamera;

    public static FishingGameManager shared;
    public Material DuckColor;
    //DuckA to be used as correct term
    public DuckPrefab DuckA, DuckB, DuckC, DuckD, duckSelected;
    public Answer newWord = null;


    [SerializeField]
    private AudioSource FishingGameAudioSource = null;

    [SerializeField]
    private FishingGameQuestionBoard fishingGameQuestionBoard = null;

    [SerializeField]
    private GameObject incorrectCard;

    [SerializeField]
    private GameObject correctCard;

    [SerializeField]
    private GameObject incorrectCard2;

    [SerializeField]
    private TextPrefabScript correctAnswerBox;

    public GameObject settingsCard;
    public TextPrefabScript scoreText;
    public TextPrefabScript correctAnswer;
    public GameObject gameScene;

    private int score = 0;
    private int numErrors = 0;
    private string currentWord = null;
    private bool canSelectDuck = true;

    [SerializeField]
    private ModuleManager module;

    [SerializeField]
    private MusicManager musicManager = null;

    [SerializeField]
    private List<Answer> TermsList;

    private void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        // Loads the module manager and finds the list of "terms" (current the Answer object).
        // Front is the word in the foreign language (prompt),
        // Back is the word in the native language(answer)
        module = FindAnyObjectByType<ModuleManager>();
        TermsList = module.terms;
        //Debug.Log(TermsList.Count);
        shared = this;
    }

    private void Start()
    {
        StartCoroutine(APIManager.StartSession(module.currentModuleID));
        // TODO: Start the game
        PlayNewWord();
        scoreText.Text = "Score: " + score;
    }

    private void Update()
    {
        // If the question is playing audio, lower the music audio.
        if (FishingGameAudioSource.isPlaying)
        {
            musicManager.audioSource.volume = 0.1f;
        }
        else
        {
            musicManager.audioSource.volume = 0.8f;
        }
        if (!canSelectDuck) return;
        // Perform raycast to see if we are clicking on a duck and determine if we need to select this word
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                //Debug.Log("Raycasting from screen position: " + touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider != null && hitObject.collider.gameObject.CompareTag("duck"))
                    {
                        //Debug.Log("Duck wCreith collider tapped");

                        duckSelected = hitObject.collider.gameObject.GetComponent<DuckPrefab>();

                        if (duckSelected != null)
                        {
                            SelectWord(duckSelected, newWord);
                            duckSelected.PlaySound();
                        }
                    }
                }
            }
        }
    }

    // Function which decides the next question and randomly assigns terms to the ducks.
    private void PlayNewWord()
    {
        numErrors = 0;

        //Get the new word randomly from a term array for now

        int randomIndex = 0;

        //Do not destroy the value this holds when mofiying code
        newWord = AdaptiveLearning.GetNextAnswer(TermsList);
        TermsList.Remove(newWord);

        List<Answer> tempWords = new List<Answer>();
        for (int i = 0; i <= 2; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, TermsList.Count - 1);
            tempWords.Add(TermsList[randomIndex]);
            TermsList.RemoveAt(randomIndex);
        }
        tempWords.Insert(0, newWord);

        //configure all the ducks
        List<DuckPrefab> ducks = new List<DuckPrefab>
        {
            DuckA, DuckB, DuckC, DuckD
        };

        // Shuffle hoops to assign words randomly
        ShuffleList(tempWords);

        //TODO: configure question board
        fishingGameQuestionBoard.ConfigureWithWord(newWord);
        // Assign words to ducks
        // Add back terms to TermList
        for (int i = 0; i < ducks.Count; i++)
        {
            if (fishingGameQuestionBoard.isImage)
            {
                ducks[i].ConfigureDuck(tempWords[i].GetFront());
                TermsList.Add(tempWords[i]);
            }
            else
            {
                ducks[i].ConfigureDuck(tempWords[i].GetBack());
                TermsList.Add(tempWords[i]);
            }
        }
    }

    // Function which plays the question audio.
    public void PlayAudioClip(AudioClip clip) => FishingGameAudioSource.PlayOneShot(clip);

    // Function that is called when a duck is selected, then plays the logic for the correct/incorrect answer choice.
    public void SelectWord(DuckPrefab selectedDuck, Answer currentAnswer)
    {
        //TODO: Add logic to check for response time.
        var responseTime = 0;
        if (selectedDuck.Text.Text == currentAnswer.GetBack() || selectedDuck.Text.Text == currentAnswer.GetFront())
        {
            score++;
            StoreManager.AddCoins(1);
            StartCoroutine(APIManager.LogAnswer(currentAnswer.GetTermID(), true));
            scoreText.Text = "Score: " + score;
            AdaptiveLearning.CalculateDecayContinuous(currentAnswer, true, responseTime);
            AdaptiveLearning.CalculateActivationValue(currentAnswer);
            StartCoroutine(SendSingleALToDatabase(currentAnswer));
            selectedDuck.GetComponent<SpawnResultText>().AnsweredCorrect(selectedDuck.transform.position);
            PlayNewWord();
        }
        else
        {
            if (numErrors == 0)
            {
                selectedDuck.GetComponent<SpawnResultText>().AnsweredIncorrect(selectedDuck.transform.position);
                selectedDuck.SetActive(false);
                numErrors++;
                return;
            }
            else if (numErrors > 0)
            {
                selectedDuck.GetComponent<SpawnResultText>().AnsweredIncorrect(selectedDuck.transform.position);
                StartCoroutine(OnAnswerIncorrect());
                AdaptiveLearning.CalculateDecayContinuous(currentAnswer, false, responseTime);
                AdaptiveLearning.CalculateActivationValue(currentAnswer);
                StartCoroutine(APIManager.LogAnswer(currentAnswer.GetTermID(), false));
                StartCoroutine(SendSingleALToDatabase(currentAnswer));
                PlayNewWord();
                return;
            }
        }
    }

    // Function which displays the correct answer when an incorrect answer is selected.
    private IEnumerator OnAnswerIncorrect()
    {
        if (fishingGameQuestionBoard.isImage)
        {
            correctAnswerBox.Text = "The correct answer is: " + newWord.GetFront();
        }
        else
        {
            correctAnswerBox.Text = "The correct answer is: " + newWord.GetBack();
        }
        correctAnswerBox.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        correctAnswerBox.gameObject.SetActive(false);
    }

    // Function which ends the game and saves the session.
    public void QuitGame()
    {
        Time.timeScale = 1;
        StartCoroutine(EndSession());
    }

    // Function which ends the session and updates the database.
    private IEnumerator EndSession()
    {
        yield return StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    // Function which updates the adaptive learning value and sends it to the database.
    private IEnumerator SendSingleALToDatabase(Answer currentTerm)
    {
        string times = string.Join(",", currentTerm.GetPresentationTimes());
        yield return StartCoroutine(APIManager.UpdateAdaptiveLearningValue(currentTerm.GetTermID(), currentTerm.GetActivation(), currentTerm.GetDecay(), currentTerm.GetIntercept(), currentTerm.GetInitialTime(), times));
    }

    // Function which displays the settings menu.
    public void ShowSettings()
    {
        // TODO: Show the Settings Prefab
        // Ex: SetActive call on a settings prefab

        // TODO: Pause the game
        canSelectDuck = false;
        Time.timeScale = 0;
        settingsCard.SetActive(true);
        settingsCard.GetComponent<Animator>().SetTrigger("SlideIn");
    }

    // Function which unpauses the game/removes the settings menu.
    public void UnPause()
    {
        Time.timeScale = 1;
        incorrectCard.SetActive(false);
        correctCard.SetActive(false);
        StartCoroutine(UnpauseAnimation());
        canSelectDuck = true;
    }

    // Function which raises the settings menu image out of the screen.
    private IEnumerator UnpauseAnimation()
    {
        settingsCard.GetComponent<Animator>().SetTrigger("SlideOut");
        yield return new WaitForSeconds(1.5f);
        settingsCard.SetActive(false);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    // Deprecated - Functions for showing old correct/incorrect cards.
    private IEnumerator ShowIncorrectCard()
    {
        Time.timeScale = 0;
        incorrectCard.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        incorrectCard.SetActive(false);
        Time.timeScale = 1;
        canSelectDuck = true;
    }

    private IEnumerator ShowIncorrectCard2()
    {
        Time.timeScale = 0;
        incorrectCard2.SetActive(true);
        correctAnswer.Text = "Incorrect\n" + "\nCorrect Answer: \n" + "\n" + newWord.GetBack();
        yield return new WaitForSecondsRealtime(2f);
        incorrectCard2.SetActive(false);
        Time.timeScale = 1;
        canSelectDuck = true;
    }

    private IEnumerator ShowCorrectCard()
    {
        Time.timeScale = 0;
        correctCard.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        correctCard.SetActive(false);
        Time.timeScale = 1;
        canSelectDuck = true;
    }
}
