using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public List<int> questionIDs;
    public List<int> answerIDs;
    public Dictionary<int, Question> currentModuleQuestions;
    public Dictionary<int, Answer> currentModuleAnswers;
    public List<Answer> terms;
    public int currentModuleID;

    // Start is called before the first frame update
    void Start()
    {
        questionIDs = new List<int>();
        answerIDs = new List<int>();
        currentModuleQuestions = new Dictionary<int, Question>();
        currentModuleAnswers = new Dictionary<int, Answer>();
        terms = new List<Answer>();
    }

    private void Awake()
    {
        if (FindObjectsOfType<ModuleManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Function which takes in the ID of the module to retrieve, then calls the API to retrieve all terms associated with the module and populates the respective list.
    public IEnumerator LoadQuestionsAndAnswers(int moduleID)
    {
        currentModuleQuestions.Clear();
        currentModuleAnswers.Clear();
        questionIDs.Clear();
        answerIDs.Clear();
        terms.Clear();
        currentModuleID = moduleID;
        yield return StartCoroutine(APIManager.GetModule(moduleID));
        yield return StartCoroutine(LoadToManager());
        yield return StartCoroutine(APIManager.RetrieveAllALValues(terms));
        AssignAdaptiveLearningValues();
    }

    // Private Helper Function to actually load the values and terms retrieved from the API into lists.
    private IEnumerator LoadToManager()
    {
        // Retrieve the current module from the API manager.

        foreach( QuestionJson q in APIManager.currentQuestions)
        {
            Dictionary<int, Answer> currentQuestionAnswers = new Dictionary<int, Answer>();
            List<int> answerIDList = new List<int>();
            foreach (AnswerJson a in q.answers)
            {
                // Creates an answer object and adds it to the list of terms and to the the question's list of answers.
                Answer value = new Answer(a.termID, a.front, a.back, a.type, a.gender, a.language, a.audioLocation, a.imageLocation);
                answerIDList.Add(value.GetTermID());
                currentQuestionAnswers.Add(a.termID, value);

                // If it's a new term add it to the module manager's database of terms/answers that we haven't seen before.
                if (!currentModuleAnswers.ContainsKey(a.termID))
                {
                    yield return StartCoroutine(DownloadAudioVisuals(value));
                    currentModuleAnswers.Add(a.termID, value);
                    terms.Add(value);
                    answerIDs.Add(a.termID);
                }

            }

            // Create the question object and download/add the important values and information.
            Question newQuestion = new(q.questionID, q.type, q.questionText, answerIDList, currentQuestionAnswers, q.audioLocation, q.imageLocation);
            currentModuleQuestions.Add(q.questionID, newQuestion);
            questionIDs.Add(q.questionID);
        }
        yield return null;
        // ViewAnswers();
    }

    // Function which calls the API to download any images and audio associated with the term.
    private IEnumerator DownloadAudioVisuals(Answer answer)
    {
        if (answer.GetAudioLocation().Length > 0)
        {
            yield return StartCoroutine(APIManager.RetrieveAudio(answer.GetAudioLocation()));
            answer.SetAnswerAudio(APIManager.currentAudio);
        }

        if (answer.GetImageLocation().Length > 0)
        {
            yield return StartCoroutine(APIManager.RetrieveImage(answer.GetImageLocation()));
            answer.SetAnswerImage(APIManager.currentImage);
        }
    }

    // Helper function which assigns the adaptive learning values to their corresponding terms.
    private void AssignAdaptiveLearningValues()
    {
        foreach (Answer a in terms)
        {
            for (int i = 0; i < terms.Count; i++)
            {
                if (APIManager.listOfALValues[i].termID == a.GetTermID())
                {
                    a.SetAdaptiveValues(APIManager.listOfALValues[i]);
                    break;
                }
            }
        }
    }

    public void ClearModules()
    {
        // Clear the current module set upon new load.
        currentModuleQuestions.Clear();
        currentModuleAnswers.Clear();
        questionIDs.Clear();
        answerIDs.Clear();
        terms.Clear();
        currentModuleID = 0;
    }

    private void ViewAnswers()
    {
        foreach (Answer answer in terms)
        {
            Debug.Log(answer.GetFront());
        }
    }
}
