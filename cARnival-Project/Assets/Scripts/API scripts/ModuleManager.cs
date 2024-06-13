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

    public IEnumerator LoadQuestionsAndAnswers(int moduleID)
    {
        yield return StartCoroutine(APIManager.GetModule(moduleID));
        LoadToManager();
    }

    private void LoadToManager()
    {
        // Clear the current module set upon new load.
        currentModuleQuestions.Clear();
        currentModuleAnswers.Clear();
        questionIDs.Clear();
        answerIDs.Clear();
        terms.Clear();

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

                // Downloads extra information (AudioVisual and Adaptive Values) for the terms/answers.
                StartCoroutine(DownloadAudioVisuals(value));
                StartCoroutine(DownloadAdaptiveLearningValues(value));

                // If it's a new term add it to the module manager's database of terms/answers that we haven't seen before.
                if (!currentModuleAnswers.ContainsKey(a.termID))
                {
                    currentModuleAnswers.Add(a.termID, value);
                    terms.Add(value);
                    answerIDs.Add(a.termID);
                }

            }

            // Create the question object and download/add the important values and information.
            Question newQuestion = new(q.questionID, q.type, q.questionText, answerIDList, currentQuestionAnswers, q.audioLocation, q.imageLocation);
            StartCoroutine(DownloadAudioVisuals(newQuestion));
            currentModuleQuestions.Add(q.questionID, newQuestion);
            questionIDs.Add(q.questionID);

        }
    }

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

    private IEnumerator DownloadAudioVisuals(Question question)
    {
        if (question.GetAudioLocation().Length > 0)
        {
            yield return StartCoroutine(APIManager.RetrieveAudio(question.GetAudioLocation()));
            question.SetQuestionAudio(APIManager.currentAudio);
        }

        if (question.GetImageLocation().Length > 0)
        {
            yield return StartCoroutine(APIManager.RetrieveImage(question.GetImageLocation()));
            question.SetQuestionImage(APIManager.currentImage);
        }
    }

    private IEnumerator DownloadAdaptiveLearningValues(Answer answer)
    {
        yield return StartCoroutine(APIManager.RetrieveAdaptiveLearningValue(answer.GetTermID()));
        answer.SetAdaptiveValues(APIManager.adaptiveValuesJson);
    }

}
