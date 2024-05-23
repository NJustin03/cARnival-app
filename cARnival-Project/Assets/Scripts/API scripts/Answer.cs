using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{

    private int termID;
    private AudioClip answerAudio;
    private Texture2D answerImage;
    private string front;
    private string back;
    private string type;
    private string gender;
    private string language;
    private string audioLocation;
    private string imageLocation;

    // Various constructors for whether or not there is an image or audio attached to the answer.

    public Answer(int termID, string front, string back, string type, string gender, string language, string audioLocation, string imageLocation)
    {
        this.termID = termID;
        this.answerAudio = null;
        this.answerImage = null;
        this.front = front;
        this.back = back;
        this.type = type;
        this.gender = gender;
        this.language = language;
        this.audioLocation = audioLocation;
        this.imageLocation = imageLocation;

    }

    public void SetAnswerAudio(AudioClip answerAudio)
    { this.answerAudio = answerAudio; }

    public void SetAnswerImage(Texture2D answerImage)
    { this.answerImage = answerImage; }

    public int GetTermID()
    { return termID; }

    public string GetFront() 
    { return front; }

    public string GetBack()
    { return back; }

    public string GetAnswerType()
    { return type; }

    public string GetGender()
    { return gender; }

    public string GetLanguage() 
    { return language; }

    public string GetImageLocation()
    { return imageLocation; }

    public string GetAudioLocation()
    { return audioLocation; }

    public Texture2D GetImage()
    { return answerImage; }

    public AudioClip GetAudio() 
    { return answerAudio; }

    override
    public string ToString()
    {
        return "Term ID: " + termID + "\n Term Front: " + front + "\n Term Back: " + back + "\n";
    }
}
