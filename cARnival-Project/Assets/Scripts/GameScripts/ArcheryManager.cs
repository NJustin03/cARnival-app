using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcheryManager : MonoBehaviour
{
    public List<Target> targets;

    public static ArcheryManager shared;

    private int score = 0;
    private string currentWord = null;
    public Answer newWord = null;


    [SerializeField]
    private ModuleManager module;

    [SerializeField]
    private List<Answer> TermsList;

    public TextPrefabScript questionText;
    public TextPrefabScript scoreText;
    public TimerPrefab timerText;
    public GameObject resultCard;


    private void Awake()
    {
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
        for (int i = 0; i < tempWords.Count; i++)
        {
            targets[i].ResetTarget(tempWords[i].GetBack(), false);
        }
        targets[3].ResetTarget(newWord.GetBack(), true);
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
            scoreText.Text = "Score: " + score;
            PlayNewWord();
        }
        else
        {
            PlayNewWord();
        }

    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        resultCard.SetActive(false);
        SceneSwapper.SwapSceneStatic("ArcheryGame");
    }
}
