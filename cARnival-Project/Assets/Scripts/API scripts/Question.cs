using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question
{
    private int questionID;
    private AudioClip questionAudio;
    private Texture2D questionImage;
    private string type;
    private string questionText;
    private List<int> answerIDs;
    private Dictionary<int, Answer> answers;
    private string audioLocation;
    private string imageLocation;

    // Various constructors for whether or not there is an image or audio attached to the question.
    public Question(int questionIDInput, string typeInput, string questionTextInput, List<int> answerIDsInput, Dictionary<int, Answer> answersInput, string audioLocation, string imageLocation)
    {
        questionID = questionIDInput;
        type = typeInput;
        questionText = questionTextInput;
        answers = answersInput;
        answerIDs = answerIDsInput;
        questionAudio = null;
        questionImage = null;
        this.audioLocation = audioLocation;
        this.imageLocation = imageLocation;
    }

    public int GetQuestionID()
    { return questionID; }

    public string GetQuestionType()
    { return type; }

    public string GetQuestionText()
    { return questionText; }

    public Dictionary<int, Answer> GetAnswers()
    { return answers; }

    public AudioClip GetQuestionAudio()
    { return questionAudio; }

    public Texture2D GetQuestionImage() 
    {  return questionImage; }

    public List<int> GetAnswerIDs()
    { return answerIDs; }

    public string GetAudioLocation()
    { return audioLocation; }

    public string GetImageLocation()
    {  return imageLocation; }

    public void SetQuestionAudio(AudioClip questionAudio)
    { this.questionAudio = questionAudio; }

    public void SetQuestionImage(Texture2D questionImage)
    { this.questionImage = questionImage; }

    override
    public string ToString()
    {
        string stringToReturn = "Question ID: " + questionID + "\nQuestion Text: " + questionText + "\nAnswers:\n";
        foreach (int answerID in answerIDs)
        {
            stringToReturn += answers[answerID].ToString() + "\n";
        }
        return stringToReturn;
    }
}
