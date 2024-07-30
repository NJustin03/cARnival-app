using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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
    private GameObject prefabInstance;
    private bool hasSpawned = false;

    [SerializeField]
    private ModuleManager module;

    [SerializeField]
    private MusicManager musicManager = null;

    [SerializeField]
    private ARAnchorManager aRAnchorManager;

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
        Debug.Log(TermsList.Count);
        shared = this;
    }

    private void Start()
    {
        StartCoroutine(APIManager.StartSession(module.currentModuleID));
        // TODO: Start the game
        PlayNewWord();
        incorrectCard.SetActive(false);
        correctCard.SetActive(false);
        scoreText.Text = "Score: " + score;
    }

    private void Update()
    {
        if (FishingGameAudioSource.isPlaying)
        {
            musicManager.audioSource.volume = 0.1f;
        }
        else
        {
            musicManager.audioSource.volume = 0.8f;
        }

        if (!hasSpawned)
        {
            Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(ray, hits, TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon))
            {
                hasSpawned = true;
                Pose hitPose = hits[0].pose;
                prefabInstance = Instantiate(gameScene, hitPose.position, hitPose.rotation);
            }
        }

        if (!canSelectDuck) return;
        // TODO: Perform raycast to see if we are clicking on a duck and determine if we need to select this word?
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                Debug.Log("Raycasting from screen position: " + touch.position);
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider != null && hitObject.collider.gameObject.CompareTag("duck"))
                    {
                        Debug.Log("Duck wCreith collider tapped");

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
        //configure all the ducks
        DuckA.ConfigureDuck(newWord.GetBack());
        DuckB.ConfigureDuck(tempWords[0].GetBack());
        DuckC.ConfigureDuck(tempWords[1].GetBack());
        DuckD.ConfigureDuck(tempWords[2].GetBack());
        //TODO: configure question board
        fishingGameQuestionBoard.ConfigureWithWord(newWord);
        //Add terms back into main term list
        TermsList.Add(newWord);
        TermsList.Add(tempWords[0]);
        TermsList.Add(tempWords[1]);
        TermsList.Add(tempWords[2]);
    }

    public void PlayAudioClip(AudioClip clip) => FishingGameAudioSource.PlayOneShot(clip);

    public void SelectWord(DuckPrefab selectedDuck, Answer currentAnswer)
    {
        //TODO: Add logic to check for response time.
        var responseTime = 0;
        if (selectedDuck.Text.Text == currentAnswer.GetBack())
        {
            score++;
            StoreManager.AddCoins(1);
            StartCoroutine(APIManager.LogAnswer(currentAnswer.GetTermID(), true));
            scoreText.Text = "Score: " + score;
            AdaptiveLearning.CalculateDecayContinuous(currentAnswer, true, responseTime);
            AdaptiveLearning.CalculateActivationValue(currentAnswer);
            selectedDuck.GetComponent<SpawnResultText>().AnsweredCorrect(selectedDuck.transform.position);
            // StartCoroutine(ShowCorrectCard());
            PlayNewWord();
        }
        else
        {
            if (numErrors == 0)
            {
                // canSelectDuck = false;
                selectedDuck.GetComponent<SpawnResultText>().AnsweredIncorrect(selectedDuck.transform.position);
                selectedDuck.SetActive(false);
                numErrors++;
                // StartCoroutine(ShowIncorrectCard());
                return;
            }
            //TODO: Add logic for giving correct answer after second incorrect guess
            else if (numErrors > 0)
            {
                // canSelectDuck = false;
                selectedDuck.GetComponent<SpawnResultText>().AnsweredIncorrect(selectedDuck.transform.position);
                StartCoroutine(OnAnswerIncorrect());
                AdaptiveLearning.CalculateDecayContinuous(currentAnswer, false, responseTime);
                AdaptiveLearning.CalculateActivationValue(currentAnswer);
                StartCoroutine(APIManager.LogAnswer(currentAnswer.GetTermID(), false));

                PlayNewWord();
                return;
            }
        }
    }

    private IEnumerator OnAnswerIncorrect()
    {
        correctAnswerBox.Text = "The correct answer is: " + newWord.GetBack();
        correctAnswerBox.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        correctAnswerBox.gameObject.SetActive(false);
    }

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
    public void QuitGame()
    {
        StartCoroutine(SendALToDatabase());
    }

    private IEnumerator SendALToDatabase()
    {
        Time.timeScale = 1;
        foreach (Answer answer in TermsList)
        {
            string times = string.Join(",", answer.GetPresentationTimes());
            yield return StartCoroutine(APIManager.UpdateAdaptiveLearningValue(answer.GetTermID(), answer.GetActivation(), answer.GetDecay(), answer.GetIntercept(), answer.GetInitialTime(), times));
        }
        StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    public void ShowSettings()
    {
        // TODO: Show the Settings Prefab
        // Ex: SetActive call on a settings prefab

        // TODO: Pause the game
        Time.timeScale = 0;
        settingsCard.SetActive(true);
        settingsCard.GetComponent<Animator>().SetTrigger("SlideIn");
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        incorrectCard.SetActive(false);
        correctCard.SetActive(false);
        StartCoroutine(UnpauseAnimation());
        canSelectDuck = true;
    }

    private IEnumerator UnpauseAnimation()
    {
        settingsCard.GetComponent<Animator>().SetTrigger("SlideOut");
        yield return new WaitForSeconds(1.5f);
        settingsCard.SetActive(false);
    }
}
