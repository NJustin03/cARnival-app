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

    // Start is called before the first frame update
    void Start()
    {
        questionIDs = new List<int>();
        currentModuleQuestions = new Dictionary<int, Question>();
        currentModuleAnswers = new Dictionary<int, Answer>();
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

        // Retrieve the current module from the API manager.
        
        foreach( QuestionJson q in APIManager.currentQuestions)
        {
            Dictionary<int, Answer> currentQuestionAnswers = new Dictionary<int, Answer>();
            List<int> answerIDList = new List<int>();
            foreach (AnswerJson a in q.answers)
            {
                Answer value = new Answer(a.termID, a.front, a.back, a.type, a.gender, a.language, a.audioLocation, a.imageLocation);
                answerIDList.Add(value.GetTermID());
                currentQuestionAnswers.Add(a.termID, value);
                StartCoroutine(DownloadAudioVisuals(value));
                if (!currentModuleAnswers.ContainsKey(a.termID))
                {
                    currentModuleAnswers.Add(a.termID, value);
                    answerIDs.Add(a.termID);
                }

            }
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

}
