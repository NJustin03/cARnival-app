using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcheryManager : MonoBehaviour
{
    public List<Target> targets;

    public static ArcheryManager shared;

    private static int score = 0;
    private string currentWord = null;
    public Answer newWord = null;

    [SerializeField]
    private ModuleManager module;

    [SerializeField]
    private List<Answer> TermsList;

    [SerializeField]
    private TextPrefabScript correctAnswerBox;

    public TextPrefabScript questionText;
    public TextPrefabScript scoreText;
    public TimerPrefab timerText;
    public GameObject resultCard;
    public GameObject settingsCard;
    public StandardizedBow bow;

    public AnimationClip slideIn;
    public AnimationClip slideOut;


    private void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        // Loads the module manager and finds the list of "terms" (current the Answer object).
        // Front is the word in the foreign language (prompt),
        // Back is the word in the native language(answer)
        module = FindAnyObjectByType<ModuleManager>();
        StartCoroutine(APIManager.StartSession(module.currentModuleID));
        TermsList = module.terms;
        Debug.Log(TermsList.Count);
        shared = this;
    }

    private void Start()
    {
        // TODO: Start the game
        PlayNewWord();
    }

    private void Update()
    {
        if (timerText.timeLeft < 0)
        {
            Time.timeScale = 0;
            resultCard.SetActive(true);
        }
    }

    void ResetTargets(List<Answer> tempWords)
    {
        List<Answer> temp = new List<Answer>();
        temp.AddRange(tempWords);

        List<Answer> shuffled = new List<Answer>();

        for (int i = 0; i < tempWords.Count; i++)
        {
            int index = Random.Range(0, temp.Count);
            shuffled.Add(temp[index]);
            temp.RemoveAt(index);
        }

        for (int i = 0; i < shuffled.Count;i++)
        {
            bool isCurrentTermCorrect = false;
            if (shuffled[i].GetTermID() == newWord.GetTermID())
            {
                isCurrentTermCorrect = true;
            }
            targets[i].ResetTarget(shuffled[i], isCurrentTermCorrect);
        }
    }

    private void PlayNewWord()
    {

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
        tempWords.Add(newWord);
        //configure all the targets
        ResetTargets(tempWords);

        //TODO: configure question board
        questionText.Text = newWord.GetFront();

        //Add terms back into main term list
        TermsList.Add(newWord);
        TermsList.Add(tempWords[0]);
        TermsList.Add(tempWords[1]);
        TermsList.Add(tempWords[2]);

    }

    public void ChooseAnswer(bool isAnswerCorrect)
    {
        if (isAnswerCorrect)
        {
            score++;
            StoreManager.AddCoins(1);
            StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), true));
            scoreText.Text = "Score: " + score;
            AdaptiveLearning.CalculateDecayContinuous(newWord, true, 0);
            AdaptiveLearning.CalculateActivationValue(newWord);
            StartCoroutine(SendSingleALToDatabase(newWord));
            PlayNewWord();
        }
        else
        {
            StartCoroutine(OnAnswerIncorrect());
            StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), false));
            AdaptiveLearning.CalculateDecayContinuous(newWord, false, 0);
            AdaptiveLearning.CalculateActivationValue(newWord);
            StartCoroutine(SendSingleALToDatabase(newWord));
            PlayNewWord();
        }

    }

    private IEnumerator OnAnswerIncorrect()
    {
        correctAnswerBox.Text = "The correct answer is: " + newWord.GetBack();
        correctAnswerBox.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        correctAnswerBox.gameObject.SetActive(false);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        resultCard.SetActive(false);
        SceneSwapper.SwapSceneStatic("ArcheryGame");
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        // TODO: Add the summary functionality if needed
        // TODO: Make sure the loading of the scene is the correct scene GameScene?
        StartCoroutine(EndSession());
    }

    private IEnumerator EndSession()
    {
        yield return StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    private IEnumerator SendSingleALToDatabase(Answer currentTerm)
    {
        Debug.Log("Updating term: " + currentTerm.GetFront());
        string times = string.Join(",", currentTerm.GetPresentationTimes());
        yield return StartCoroutine(APIManager.UpdateAdaptiveLearningValue(currentTerm.GetTermID(), currentTerm.GetActivation(), currentTerm.GetDecay(), currentTerm.GetIntercept(), currentTerm.GetInitialTime(), times));
    }

    // Function that sends all Adaptive Learning values at once to the database. 
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
        bow.isPaused = true;
        Time.timeScale = 0;
        settingsCard.SetActive(true);
        settingsCard.GetComponent<Animator>().SetTrigger("SlideIn");

    }

    public void UnPause()
    {
        bow.isPaused = false;
        Time.timeScale = 1;
        StartCoroutine(UnpauseAnimation());
    }

    private IEnumerator UnpauseAnimation()
    {
        settingsCard.GetComponent<Animator>().SetTrigger("SlideOut");
        yield return new WaitForSeconds(1.5f);
        settingsCard.SetActive(false);
    }
}
